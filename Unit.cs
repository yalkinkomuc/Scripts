using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Unit : MonoBehaviour
{

    #region Actions

    private MoveAction moveAction;
    private BaseAction[] baseActionArray;

    #endregion

    #region EventHandlers

    public static event EventHandler OnAnyActionPointsChanged;
    
    #endregion

    #region ActionPoints

    private const int ACTION_POINTS_MAX = 500;
    [SerializeField] private int actionPoints = ACTION_POINTS_MAX;

    #endregion

    [FormerlySerializedAs("unitID")] public int teamID;

    [SerializeField] private bool isEnemy;
    
    
    private void Awake()
    {
        moveAction = GetComponent<MoveAction>();
        baseActionArray = GetComponents<BaseAction>();
    }

    private void Start()
    {
        TurnSystem.instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        actionPoints = ACTION_POINTS_MAX;
    }

    #region GetActionField

    public MoveAction GetMoveAction()
    {
        return moveAction;
    }

    public BaseAction[] GetBaseActionArray()
    {
        return baseActionArray;
    }

    #endregion
    
    #region ActionPointManagements

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (CanSpendActionPointsToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointsCost());
            return true;
        }
        else
        {
            
            return false;
        }
    }
    
    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction)
    {

        return actionPoints >= baseAction.GetActionPointsCost();

    }

    private void SpendActionPoints(int amount)
    {
        actionPoints -= amount;
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetActionPoints()
    {
        return actionPoints;
    }

    #endregion

    #region EventHandlers

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if ((IsEnemy() && !TurnSystem.instance.IsPlayerTurn()) || (!IsEnemy() && TurnSystem.instance.IsPlayerTurn()))
        {
            actionPoints = ACTION_POINTS_MAX;
        
            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
        
        
    }

    #endregion

    public bool IsEnemy()
    {
        return isEnemy;
    }

    public void Damage()
    {
        Debug.Log(transform+"Damage");
    }

    public Vector3 GetUnitWorldPosition()
    {
       return transform.position;     
    }
    
}
