using UnityEngine;
using System;

public class ArrowProjectile : MonoBehaviour
{
    private Vector3 targetPosition;
    private Unit targetUnit;
    [SerializeField] private float movementSpeed = 10f;

    public event EventHandler<OnArrowHitEventArgs> OnArrowHit;

    public class OnArrowHitEventArgs : EventArgs
    {
        public Unit targetUnit;
    }

    public void Setup(Vector3 targetPosition, Unit targetUnit)
    {
        this.targetPosition = targetPosition;
        this.targetUnit = targetUnit;
    }

    private void Update()
    {
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        transform.forward = moveDirection;
        
        float distanceBeforeMoving = Vector3.Distance(transform.position, targetPosition);
        transform.position += moveDirection * movementSpeed * Time.deltaTime;
        float distanceAfterMoving = Vector3.Distance(transform.position, targetPosition);

        if (distanceBeforeMoving < distanceAfterMoving)
        {
            // Hedefe ulaştık
            OnArrowHit?.Invoke(this, new OnArrowHitEventArgs { 
                targetUnit = targetUnit 
            });
            
            Destroy(gameObject);
        }
    }
}