using System;
using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System.Linq;

public class Unit : MonoBehaviour
{

    #region Actions

    private MoveAction moveAction;
    private BaseAction[] baseActionArray;

    #endregion

    #region Health

    [SerializeField] private HealthSystem healthSystem;

    #endregion

    #region EventHandlers

    public static event EventHandler OnAnyActionPointsChanged;
    
    #endregion

    #region ActionPoints

    private const int ACTION_POINTS_MAX = 5;
    [SerializeField] private int actionPoints = ACTION_POINTS_MAX;

    #endregion

    [FormerlySerializedAs("unitID")] public int teamID;

    [SerializeField] private bool isEnemy;
    
    [SerializeField] private int maxActionPoints = 5;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        moveAction = GetComponent<MoveAction>();
        baseActionArray = GetComponents<BaseAction>(); 
    }

    private void Start()
    {
        TurnSystem.instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        actionPoints = maxActionPoints;
        healthSystem.OnDead += HealthSystem_OnDead;
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

    public void ResetActionPoints()
    {
        actionPoints = maxActionPoints;

        Debug.Log("Action Points Reset");
    }

    public BaseAction[] GetActions()
    {
        return GetComponents<BaseAction>();
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

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        Destroy(gameObject);
    }

    #endregion

    public bool IsEnemy()
    {
        return isEnemy;
    }

    public void Damage(int damageAmount)
    {
        healthSystem.Damage(damageAmount);
        //Debug.Log(transform+"Damage");
        
    }

    public Vector3 GetUnitWorldPosition()
    {
       return transform.position;     
    }
    
}
