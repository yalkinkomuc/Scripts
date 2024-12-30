using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class MoveAction : BaseAction
{
    [SerializeField] private PathVisualizer pathVisualizer;
    private NavMeshAgent agent;

    [Header("Movement Details")] 
    private float stoppingDistance = .1f;
    private Vector3 targetPosition;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 120f;

    [Header("Movement Points")] 
    [SerializeField] private int maxMovementPoints = 5;
    [SerializeField] private int currentMovementPoints;
    private float movementCostPerUnit = 100f;

    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;

    private float turnSmoothVelocity;

    protected override void Awake()
    {
        base.Awake();
        currentMovementPoints = maxMovementPoints;

        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.angularSpeed = rotationSpeed;
            agent.stoppingDistance = stoppingDistance;
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            agent.autoTraverseOffMeshLink = false;
            agent.updatePosition = true;
            agent.updateRotation = false;
        }

        if (pathVisualizer != null)
        {
            pathVisualizer.Setup(agent, currentMovementPoints / movementCostPerUnit);
        }
    }

    public override void TakeAction(Vector3 targetPoint, Action onActionComplete)
    {
        if (agent == null) return;

        ActionStart(onActionComplete);

        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(targetPoint, path))
        {
            float pathLength = CalculatePathLength(path.corners);
            float maxPossibleDistance = currentMovementPoints / movementCostPerUnit;

            if (pathLength > maxPossibleDistance)
            {
                Vector3 limitedTarget = FindPointOnPath(path.corners, maxPossibleDistance);
                targetPosition = limitedTarget;
            }
            else
            {
                targetPosition = targetPoint;
            }

            agent.SetDestination(targetPosition);
            OnStartMoving?.Invoke(this, EventArgs.Empty);
        }

        HidePath();
    }

    private void Update()
    {
        if (!isActive) return;

        if (agent != null && agent.hasPath)
        {
            // Hareket mesafesini hesapla ve movement points'i güncelle
            float distanceMoved = agent.velocity.magnitude * Time.deltaTime;
            currentMovementPoints -= Mathf.CeilToInt(distanceMoved * movementCostPerUnit);

            // Karakterin yönünü hareket yönüne doğru çevir
            if (agent.velocity.sqrMagnitude > 0.1f)
            {
                Vector3 moveDirection = agent.velocity.normalized;
                float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
                float angle = Mathf.SmoothDampAngle(
                    transform.eulerAngles.y,
                    targetAngle,
                    ref turnSmoothVelocity,
                    1f / rotationSpeed
                );
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
            }

            if (pathVisualizer != null)
            {
                pathVisualizer.UpdateMaxDistance(currentMovementPoints / movementCostPerUnit);
            }

            // Hedefe ulaşıldı mı veya hareket puanı bitti mi kontrol et
            if (currentMovementPoints <= 0 || 
                (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance))
            {
                StopMoving();
            }
        }
    }

    private void StopMoving()
    {
        if (agent != null)
        {
            agent.ResetPath();
        }
        
        OnStopMoving?.Invoke(this, EventArgs.Empty);
        ActionComplete();
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        currentMovementPoints = maxMovementPoints;
        if (pathVisualizer != null)
        {
            pathVisualizer.UpdateMaxDistance(currentMovementPoints / movementCostPerUnit);
        }
    }

    private void Start()
    {
        TurnSystem.instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    public void ShowPath(Vector3 targetPoint)
    {
        if (pathVisualizer != null)
        {
            pathVisualizer.UpdatePath(targetPoint);
        }
    }

    public void HidePath()
    {
        if (pathVisualizer != null)
        {
            pathVisualizer.HidePath();
        }
    }

    // Yardımcı methodlar
    private float CalculatePathLength(Vector3[] pathPoints)
    {
        float length = 0;
        for (int i = 0; i < pathPoints.Length - 1; i++)
        {
            length += Vector3.Distance(pathPoints[i], pathPoints[i + 1]);
        }
        return length;
    }

    private Vector3 FindPointOnPath(Vector3[] pathPoints, float maxDistance)
    {
        float currentLength = 0;
        
        for (int i = 0; i < pathPoints.Length - 1; i++)
        {
            float segmentLength = Vector3.Distance(pathPoints[i], pathPoints[i + 1]);
            
            if (currentLength + segmentLength > maxDistance)
            {
                float remainingDistance = maxDistance - currentLength;
                Vector3 direction = (pathPoints[i + 1] - pathPoints[i]).normalized;
                return pathPoints[i] + direction * remainingDistance;
            }
            
            currentLength += segmentLength;
        }
        
        return pathPoints[pathPoints.Length - 1];
    }

    public override string GetActionName()
    {
        return "Move";
    }

    public float GetMovementCostPerUnit()
    {
        return movementCostPerUnit;
    }

    public float GetMaxMovementPoints()
    {
        return maxMovementPoints;
    }

    public float GetCurrentMovementPoints()
    {
        return currentMovementPoints;
    }
}
