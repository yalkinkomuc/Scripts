using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class PathVisualizer : MonoBehaviour
{
    [SerializeField] private LineRenderer validPathRenderer;
    [SerializeField] private LineRenderer invalidPathRenderer;
    [SerializeField] private Color validPathColor = Color.blue;
    [SerializeField] private Color invalidPathColor = Color.red;
    [SerializeField] private int smoothingSegments = 3;

    [Header("Pulse Effect")]
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float minAlpha = 0.3f;
    [SerializeField] private float maxAlpha = 1f;
    
    private float pulseTimer;
    private bool isPathVisible;

    private NavMeshAgent targetAgent;
    private float maxDistance;

    [SerializeField] private float pathHeight = 0.3f;

    private void Awake()
    {
        SetupPathRenderers();
        HidePath();
    }

    public void Setup(NavMeshAgent agent, float maxMovementDistance)
    {
        targetAgent = agent;
        maxDistance = maxMovementDistance;
        SetupPathRenderers();
    }

    private void SetupPathRenderers()
    {
        if (validPathRenderer != null)
        {
            SetupLineRenderer(validPathRenderer, validPathColor);
        }

        if (invalidPathRenderer != null)
        {
            SetupLineRenderer(invalidPathRenderer, invalidPathColor);
        }
    }

    private void SetupLineRenderer(LineRenderer renderer, Color color)
    {
        renderer.startWidth = 0.2f;
        renderer.endWidth = 0.2f;
        renderer.material = new Material(Shader.Find("Sprites/Default"));
        renderer.material.color = color;
        renderer.sortingOrder = 1;
        renderer.alignment = LineAlignment.View;
    }

    public void UpdatePath(Vector3 targetPoint)
    {
        if (targetAgent == null || !targetAgent.enabled) return;

        NavMeshPath path = new NavMeshPath();
        if (targetAgent.CalculatePath(targetPoint, path))
        {
            List<Vector3> pathPoints = new List<Vector3>(path.corners);
            pathPoints = FixPathHeight(pathPoints);
            float totalDistance = CalculatePathLength(pathPoints);

            if (totalDistance <= maxDistance)
            {
                ShowValidPath(pathPoints);
            }
            else
            {
                SplitAndShowPath(pathPoints, maxDistance);
            }
            
            isPathVisible = true;
        }
    }

    public void UpdateMaxDistance(float newMaxDistance)
    {
        maxDistance = newMaxDistance;
    }

    private void ShowValidPath(List<Vector3> pathPoints)
    {
        List<Vector3> smoothedPath = SmoothPath(pathPoints);
        validPathRenderer.positionCount = smoothedPath.Count;
        validPathRenderer.SetPositions(smoothedPath.ToArray());
        invalidPathRenderer.positionCount = 0;
    }

    private void SplitAndShowPath(List<Vector3> pathPoints, float maxDistance)
    {
        List<Vector3> validPath = new List<Vector3>();
        List<Vector3> invalidPath = new List<Vector3>();
        float currentLength = 0;

        validPath.Add(pathPoints[0]);
        for (int i = 1; i < pathPoints.Count; i++)
        {
            float segmentLength = Vector3.Distance(pathPoints[i-1], pathPoints[i]);
            
            if (currentLength + segmentLength <= maxDistance)
            {
                validPath.Add(pathPoints[i]);
                currentLength += segmentLength;
            }
            else
            {
                Vector3 direction = (pathPoints[i] - pathPoints[i-1]).normalized;
                Vector3 intersectionPoint = pathPoints[i-1] + direction * (maxDistance - currentLength);
                
                if (NavMesh.SamplePosition(intersectionPoint, out NavMeshHit hit, 1f, NavMesh.AllAreas))
                {
                    intersectionPoint = hit.position + Vector3.up * pathHeight;
                }
                
                validPath.Add(intersectionPoint);
                invalidPath.Add(intersectionPoint);

                NavMeshPath remainingPath = new NavMeshPath();
                if (NavMesh.CalculatePath(intersectionPoint, pathPoints[i], NavMesh.AllAreas, remainingPath))
                {
                    invalidPath.AddRange(new List<Vector3>(remainingPath.corners));
                }
                else
                {
                    invalidPath.Add(pathPoints[i]);
                }
                
                for (int j = i + 1; j < pathPoints.Count; j++)
                {
                    invalidPath.Add(pathPoints[j]);
                }
                break;
            }
        }

        List<Vector3> smoothedValidPath = SmoothPath(validPath);
        List<Vector3> smoothedInvalidPath = SmoothPath(invalidPath);

        validPathRenderer.positionCount = smoothedValidPath.Count;
        validPathRenderer.SetPositions(smoothedValidPath.ToArray());
        
        invalidPathRenderer.positionCount = smoothedInvalidPath.Count;
        invalidPathRenderer.SetPositions(smoothedInvalidPath.ToArray());
    }

    public void HidePath()
    {
        if (validPathRenderer != null)
        {
            validPathRenderer.positionCount = 0;
            validPathRenderer.SetPositions(new Vector3[0]);
        }
        if (invalidPathRenderer != null)
        {
            invalidPathRenderer.positionCount = 0;
            invalidPathRenderer.SetPositions(new Vector3[0]);
        }
        
        isPathVisible = false;
    }

    private float CalculatePathLength(List<Vector3> pathPoints)
    {
        float length = 0;
        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            length += Vector3.Distance(pathPoints[i], pathPoints[i + 1]);
        }
        return length;
    }

    private List<Vector3> SmoothPath(List<Vector3> path)
    {
        if (path.Count < 3) return path;

        List<Vector3> smoothedPath = new List<Vector3>();
        smoothedPath.Add(path[0]);

        for (int i = 1; i < path.Count - 1; i++)
        {
            Vector3 p0 = path[i - 1];
            Vector3 p1 = path[i];
            Vector3 p2 = path[i + 1];

            float angle = Vector3.Angle(p1 - p0, p2 - p1);
            if (angle > 30f)
            {
                Vector3 control1 = p1 - (p1 - p0).normalized * (Vector3.Distance(p1, p0) * 0.25f);
                Vector3 control2 = p1 + (p2 - p1).normalized * (Vector3.Distance(p1, p2) * 0.25f);

                for (int j = 0; j < smoothingSegments; j++)
                {
                    float t = j / (float)smoothingSegments;
                    Vector3 point = CalculateBezierPoint(control1, p1, control2, t);
                    smoothedPath.Add(point);
                }
            }
            else
            {
                smoothedPath.Add(p1);
            }
        }

        smoothedPath.Add(path[path.Count - 1]);
        return smoothedPath;
    }

    private Vector3 CalculateBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 point = uu * p0;
        point += 2 * u * t * p1;
        point += tt * p2;

        return FixHeight(point);
    }

    private Vector3 FixHeight(Vector3 position)
    {
        float searchHeight = 10f;
        Vector3 searchPoint = new Vector3(position.x, searchHeight, position.z);
        
        if (NavMesh.SamplePosition(searchPoint, out NavMeshHit hit, searchHeight * 2f, NavMesh.AllAreas))
        {
            return new Vector3(position.x, hit.position.y + pathHeight, position.z);
        }
        return position;
    }

    private List<Vector3> FixPathHeight(List<Vector3> pathPoints)
    {
        List<Vector3> fixedPoints = new List<Vector3>();
        
        for (int i = 0; i < pathPoints.Count; i++)
        {
            Vector3 fixedPoint = FixHeight(pathPoints[i]);
            fixedPoints.Add(fixedPoint);
        }
        
        return fixedPoints;
    }

    private void Update()
    {
        if (!isPathVisible) return;

        pulseTimer += Time.deltaTime * pulseSpeed;
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(pulseTimer) + 1f) * 0.5f);

        if (validPathRenderer != null)
        {
            Color validColor = validPathRenderer.material.color;
            validColor.a = alpha;
            validPathRenderer.material.color = validColor;
        }

        if (invalidPathRenderer != null)
        {
            Color invalidColor = invalidPathRenderer.material.color;
            invalidColor.a = alpha;
            invalidPathRenderer.material.color = invalidColor;
        }
    }
} 