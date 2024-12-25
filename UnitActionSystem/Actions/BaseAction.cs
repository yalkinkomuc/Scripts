using System;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    protected Unit unit;
    protected bool isActive;
    protected Action onActionComplete;

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
    
}
