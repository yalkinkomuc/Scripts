using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectedVisual_UI : MonoBehaviour
{
    [SerializeField] private Unit unit;
    
    [SerializeField] private BowRangeAction bowRangeAction;
    
    
      
    
    
    
    private MeshRenderer meshRenderer;
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        
        UpdateVisual();
    }

   


    private void UnitActionSystem_OnSelectedUnitChanged(object sender,EventArgs empty)
    {
       UpdateVisual();
    }
    
    public void UpdateVisual()
    {
        if (UnitActionSystem.Instance.GetSelectedUnit() == unit)
        {
            meshRenderer.enabled = true;
        }
        else
        {
            meshRenderer.enabled = false;
        }
    }


   

    private void ShowTargetsInRange()
    {
        List<Unit> targets = bowRangeAction.GetValidTargetListWithSphere(bowRangeAction.bowRange);  // Menzildeki hedefleri al

        foreach (Unit target in targets)
        {
            UnitSelectedVisual_UI targetVisualUI = target.GetComponent<UnitSelectedVisual_UI>();  // Hedefin görselini kontrol et
            if (targetVisualUI != null)
            {
                targetVisualUI.ShowVisual();  // Hedefin görselini aktif et
            }
        }
    }
    
    private void ClearTargetVisuals()
    {
        List<Unit> targets = bowRangeAction.GetValidTargetListWithSphere(bowRangeAction.bowRange);  // Menzildeki hedefleri al

        foreach (Unit target in targets)
        {
            UnitSelectedVisual_UI targetVisualUI = target.GetComponent<UnitSelectedVisual_UI>();
            if (targetVisualUI != null)
            {
                targetVisualUI.HideVisual();  // Hedefin görselini gizle
            }
        }
    }
    
    
    public void ShowVisual()
    {
        meshRenderer.enabled = true;  // Hedefi göster
    }

    public void HideVisual()
    {
        meshRenderer.enabled = false;  // Hedefi gizle
    }
    
}
