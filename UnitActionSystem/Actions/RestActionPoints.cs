using System;
using UnityEngine;

public class RestActionPoints : BaseAction
{
    [SerializeField] private int actionPointCost = 2;

    public override string GetActionName()
    {
        return "Rest";
    }

    public override void TakeAction(Vector3 targetPosition, Action onActionComplete)
    {
        ActionStart(onActionComplete);

        if (unit != null)
        {
            unit.ResetActionPoints();
        }

        ActionComplete();
    }

    public override int GetActionPointsCost()
    {
        return actionPointCost;
    }
} 