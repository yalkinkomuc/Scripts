using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    protected Unit unit;
    protected bool isActive;
    protected Action onActionComplete;

    [SerializeField] protected LayerMask whatIsUnit;
    
    protected virtual void Awake()
    {
        unit = GetComponent<Unit>();
    }

    

    public abstract string GetActionName();

    public virtual int GetActionPointsCost()
    {
        return 0;
    }

    public abstract void TakeAction(Vector3 worldPosition,Action onActionComplete);

    protected void ActionStart(Action onActionComplete)
    {
        isActive = true;
        this.onActionComplete = onActionComplete;
    }

    protected void ActionComplete()
    {
        isActive = false;
        onActionComplete();
    }


    public virtual Unit GetValidTarget(float attackRange)
    {
        Vector3 mouseWorldPosition = MouseWorld.GetMouseWorldPosition();
        Vector3 rayOrigin = transform.position + Vector3.up * 1.5f;

        // Fare pozisyonuna doğru bir yön oluştur
        Vector3 rayDirection = (mouseWorldPosition - rayOrigin).normalized;

        // Ray'i çizmeye başla
        Ray ray = new Ray(rayOrigin, rayDirection);

        Debug.DrawRay(ray.origin, rayDirection * attackRange, Color.red, 10f);

        if (Physics.Raycast(ray, out RaycastHit hit, attackRange))
        {
            Unit targetUnit = hit.collider.GetComponent<Unit>();

            if (targetUnit != null && unit.teamID != targetUnit.teamID)
            {
                Debug.Log($"Geçerli hedef bulundu: {targetUnit.name}");
                return targetUnit;
            }
            else
            {
                Debug.Log("Ray hedefe çarptı ancak geçerli bir hedef yok.");
            }
        }
        // else
        // {
        //     Debug.Log("Ray herhangi bir objeye çarpmadı.");
        // }

        return null;
    }


    
    public virtual List<Unit> GetValidTargetListWithSphere(float radius)
    {
        List<Unit> validTargets = new List<Unit>();
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius,whatIsUnit);

        foreach (Collider unitHitColliders in colliders)
        {
            Unit targetUnit = unitHitColliders.GetComponent<Unit>();

            if (targetUnit != null && unit.teamID != targetUnit.teamID)
            {
                validTargets.Add(targetUnit); // Geçerli hedefi listeye ekle
                Debug.Log($"Geçerli hedef (Sphere) bulundu: {targetUnit.name}");
            }
        }

        return validTargets;
    }

    
    
    
    
    
}
