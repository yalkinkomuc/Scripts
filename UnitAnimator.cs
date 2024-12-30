using System;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform arrowProjectilePrefab;
    [SerializeField] private Transform shootPointTransform;

    private void Awake()
    {
        if (TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
        }

        if (TryGetComponent<BowRangeAction>(out BowRangeAction bowRangeAction))
        {
            bowRangeAction.OnShootAnimStarted += BowRangeAction_OnShootAnimStarted;
            bowRangeAction.OnShootCompleted += BowRangeAction_OnShootCompleted;
            bowRangeAction.OnArrowFired += BowRangeAction_OnArrowFired;
        }
    }

  


    #region Bow Range Action
    
    private void BowRangeAction_OnShootAnimStarted(object sender, EventArgs e)
    {
        animator.SetTrigger("Shoot");
    }
    
    private void BowRangeAction_OnArrowFired(object sender, BowRangeAction.OnArrowFiredEventArgs e)
    {
        animator.SetTrigger("Shoot");

        Transform arrowProjectileTransform = Instantiate(arrowProjectilePrefab, shootPointTransform.position, Quaternion.identity);
        ArrowProjectile arrowProjectile = arrowProjectileTransform.GetComponent<ArrowProjectile>();

        Vector3 targetUnitShootAtPosition = e.targetUnit.GetUnitWorldPosition();
        targetUnitShootAtPosition.y = shootPointTransform.position.y;
    
        arrowProjectile.Setup(targetUnitShootAtPosition, e.targetUnit);
    
        // BowRangeAction'dan damage değerini al
        BowRangeAction bowRangeAction = sender as BowRangeAction;
        if (bowRangeAction != null)
        {
            arrowProjectile.OnArrowHit += (s, args) => {
                if (args.targetUnit != null)
                {
                    args.targetUnit.Damage(bowRangeAction.GetDamageAmount());
                }
            };
        }
    }
    
    private void BowRangeAction_OnShootCompleted(object sender, EventArgs e)
    {
        animator.ResetTrigger("Shoot");
    }

#endregion
   

    #region  Move Anim Region

    private void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        animator.SetBool("isMoving", true);
    }

    private void MoveAction_OnStopMoving(object sender, EventArgs e)
    {
        animator.SetBool("isMoving", false);
    }

    #endregion
    
    
}