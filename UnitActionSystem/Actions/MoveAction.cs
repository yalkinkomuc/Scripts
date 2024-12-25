using System;
using UnityEngine;

public class MoveAction : BaseAction
{
    #region Components

    [SerializeField] private Animator animator;

    #endregion


    #region MovementDetails

    [Header("Movement Details")] private float stoppingDistance = .1f;
    private Vector3 targetPosition;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    #endregion
    
    #region MovementLimitDetails

    [Header("Movement Points")] [SerializeField]
    private int maxMovementPoints = 5; // Maksimum hareket puanı
    [SerializeField] private int currentMovementPoints;
    private float movementCostPerUnit = 100f; // 1 birim mesafe için harcanacak puan

    #endregion
    protected override void Awake()
    {
        base.Awake();
        targetPosition = transform.position;
        ResetMovementPoints(); // İlk başta hareket puanlarını sıfırla
    }


    private void Start()
    {
        TurnSystem.instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }
        
        MoveToTarget();
    }

    #region EventHandlers

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        ResetMovementPoints();
    }

    #endregion
    
    #region MovementManagement

    private void MoveToTarget()
    {
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        // Hedefe ulaşmamış ve yeterli hareket puanı varsa
        if (distanceToTarget > stoppingDistance && currentMovementPoints > 0)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;

            // Hareket adımı: Hız ile hareket ediyoruz, ama kalan puanı kontrol ediyoruz
            float moveStep = Mathf.Min(moveSpeed * Time.deltaTime, currentMovementPoints / movementCostPerUnit);

            transform.position += moveDirection * moveStep; // Hareket ettir

            // Harcanan puanı hesapla ve düş
            currentMovementPoints -= Mathf.CeilToInt(moveStep * movementCostPerUnit);

            // Dönüşü uygula
            transform.forward = Vector3.Slerp(transform.forward, moveDirection, rotationSpeed * Time.deltaTime);

            animator.SetBool("isMoving", true);
        }
        else
        {
            StopMoving();
        }
     
    }

    private void StopMoving()
    {
        isActive = false;
        animator.SetBool("isMoving", false);
        onActionComplete(); // aksiyon bittiğinde burdan ateşle
        Debug.Log("Movement stopped. Remaining movement points: " + currentMovementPoints / movementCostPerUnit);
    }
    
    public void ResetMovementPoints()
    {
        currentMovementPoints = maxMovementPoints;
        Debug.Log("Movement points reset!");
    }

    #endregion

    #region ActionManagement

    public override void TakeAction(Vector3 newTargetPosition,Action onActionComplete)
    {
        this.onActionComplete = onActionComplete;
        targetPosition = newTargetPosition;
        isActive = true;
    }
    public override string GetActionName()
    {
        return "Move";
    }

    #endregion
    
    
}
