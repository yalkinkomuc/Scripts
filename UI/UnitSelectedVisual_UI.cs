using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectedVisual_UI : MonoBehaviour
{
    [SerializeField] private Unit unit;
    public MeshRenderer selectedCircleMeshRenderer;

    private void Awake()
    {
        // Ensure we have the MeshRenderer
        if (selectedCircleMeshRenderer == null)
        {
            selectedCircleMeshRenderer = GetComponent<MeshRenderer>();
        }
        
        // Başlangıçta kesinlikle gizli olsun
        if (selectedCircleMeshRenderer != null)
        {
            selectedCircleMeshRenderer.enabled = false;
        }
    }

    private void Start()
    {
        // Event'leri bağlamadan önce visual'ı gizle
        HideVisual();
        
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        
        // İlk update'i bir frame geciktir
        Invoke("UpdateVisual", 0.1f);
    }

    private void Update()
    {
        // Eğer BowAction seçiliyse her frame güncelle
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        if (selectedAction is BowRangeAction)
        {
            UpdateVisual();
        }
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();

        // Önce circle'ı gizle
        HideVisual();

        // Eğer bu unit seçili unit ise her zaman göster
        if (unit == selectedUnit)
        {
            ShowVisual();
            return;
        }

        // Eğer BowAction seçili ve bu unit hedef olabilecek bir düşmansa göster
        if (selectedAction is BowRangeAction bowAction)
        {
            List<Unit> validTargets = bowAction.GetValidTargetListWithSphere(bowAction.GetMaxShootDistance());
            if (validTargets.Contains(unit))
            {
                ShowVisual();
            }
        }
    }

    private void ShowVisual()
    {
        if (selectedCircleMeshRenderer != null)
        {
            selectedCircleMeshRenderer.enabled = true;
            //selectedCircleMeshRenderer.material.color = Color.green; 
        }
    }

    public void HideVisual()
    {
        if (selectedCircleMeshRenderer != null)
        {
            selectedCircleMeshRenderer.enabled = false; 
            
        }
    }

    private void OnDestroy()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged -= UnitActionSystem_OnSelectedActionChanged;
    }
}
