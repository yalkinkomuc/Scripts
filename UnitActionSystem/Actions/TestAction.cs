using System;
using UnityEngine;

public class TestAction : BaseAction
{
    public override string GetActionName()
    {
        return "Test Action";
    }

    public override void TakeAction(Vector3 worldPosition, Action onActionComplete)
    {
        Debug.Log("take action");
    }

    public override int GetActionPointsCost()
    {
        return 1;
    }
}
