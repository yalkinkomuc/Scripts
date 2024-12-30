using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class BowRangeAction : BaseAction, ITargetVisualAction
{
    [Header("Bow Shoot Information")]
    [SerializeField] public int bowRange;
    [SerializeField] private Transform shootPosition;
    [SerializeField] private float aimDuration = 1;
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private GameObject arrowPrefab;

    private State state;
    private float stateTimer;
    private Unit targetUnit;
    private bool canShootArrow;
    private float rotateSpeed = 10f;

    public event EventHandler OnShootAnimStarted;
    public event EventHandler<OnArrowFiredEventArgs> OnArrowFired;
    public event EventHandler OnShootCompleted;
    
    

    public class OnArrowFiredEventArgs : EventArgs
    {
        public Unit shootingUnit;
        public Unit targetUnit;
    }

    private enum State
    {
        Aiming,
        Shooting,
        Cooloff,
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public override string GetActionName() => "Range Attack";

    private void Update()
    {
        
        
        if (!isActive)
        {
            return;
        }
        

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.Aiming:
                if (targetUnit != null)
                {
                    Vector3 targetAimDir = (targetUnit.GetUnitWorldPosition() - unit.GetUnitWorldPosition()).normalized;
                    transform.forward = Vector3.Lerp(transform.forward, targetAimDir, Time.deltaTime * rotateSpeed);
                    OnShootAnimStarted?.Invoke(this, EventArgs.Empty);
                }
               
                break;

            case State.Shooting:
                if (canShootArrow)
                {
                    ShootArrow();
                    canShootArrow = false;
                    OnShootCompleted?.Invoke(this, EventArgs.Empty);
                }
                break;

            case State.Cooloff:
                break;
        }

        if (stateTimer <= 0f)
        {
            NextState();
        }
    }

    private void NextState()
    {
        
        
        switch (state)
        {
            case State.Aiming:
                state = State.Shooting;
                stateTimer = aimDuration;
                break;
            case State.Shooting:
                state = State.Cooloff;
                stateTimer = 0.35f; // değişebilir 0.5f ti normalde
                break;
            case State.Cooloff:
                ActionComplete();
                break;
        }
    }

    private void ShootArrow()
    {
        OnArrowFired?.Invoke(this, new OnArrowFiredEventArgs
        {
            shootingUnit = unit,
            targetUnit = targetUnit
        });
    }

    private void ArrowProjectile_OnHit(object sender, ArrowProjectile.OnArrowHitEventArgs e)
    {
        if (e.targetUnit != null)
        {
            e.targetUnit.Damage(damageAmount);
            Debug.Log(" VURDUMM");
        }
        
        // Event listener'ı temizle
        ((ArrowProjectile)sender).OnArrowHit -= ArrowProjectile_OnHit;
    }

    public override void TakeAction(Vector3 worldPosition, Action onActionComplete)
    {
        targetUnit = GetValidTarget(bowRange);
        
        // Önce ActionStart'ı çağır, sonra kontrolleri yap
        ActionStart(onActionComplete);

        if (targetUnit == null || targetUnit == unit)
        {
            // Hedef yoksa veya kendisiyse direkt bitir
            ActionComplete();
            return;
        }
        
        state = State.Aiming;
        stateTimer = aimDuration;
        canShootArrow = true;
    }


    public override int GetActionPointsCost()
    {

        if (GetValidTarget(bowRange) == null)
        {
            return 0; 
        }
        
        return 3;
    }

    public override List<Unit> GetValidTargetListWithSphere(float radius)
    {
        return base.GetValidTargetListWithSphere(bowRange);
    }


    // private void OnDrawGizmos()
    // {
    //     // Sphere'ı sahnede çizmek için Gizmos kullanıyoruz
    //     Gizmos.color = Color.red;  // Sphere'ın rengini belirleyelim
    //     Gizmos.DrawWireSphere(transform.position, bowRange);  // Sphere çiz
    //
    //     // Menzildeki tüm birimleri alalım ve konsola yazdıralım
    //     List<Unit> validTargets = GetValidTargetListWithSphere(bowRange);
    //     foreach (Unit target in validTargets)
    //     {
    //         // Her hedefin adını konsola yazdır
    //         Debug.Log($"Menzildeki hedef: {target.name}");
    //     }
    // }

    // Geçerli hedefi almak için bir fonksiyon
    // private Unit GetValidTarget(float attackRange)
    // {
    //     // Hedefin geçerli olup olmadığını kontrol et
    //     return base.GetValidTarget(bowRange);
    // }

    public int GetDamageAmount()
    {
        return damageAmount;
    }

    public int GetMaxShootDistance()
    {
        return bowRange;
    }

    public bool ShouldShowTargetVisual(Unit targetUnit)
    {
        // GetValidTargetList yerine GetValidTargetListWithSphere kullanıyoruz
        List<Unit> validTargets = GetValidTargetListWithSphere(bowRange);
        return validTargets.Contains(targetUnit);
    }
}
