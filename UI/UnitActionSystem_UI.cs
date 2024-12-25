using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitActionSystem_UI : MonoBehaviour
{

    [Header("ActionSystemUI")]
    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainerTransform;
    [SerializeField] private TextMeshProUGUI actionPointsText;

    private List<ActionButton_UI> actionButtonUIList;

    private void Awake()
    {
        actionButtonUIList = new List<ActionButton_UI>();
    }

    private void Start()
    {

        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_UI_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_UI_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnActionStarted;
        TurnSystem.instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
        
        UpdateActionPointsText();
        CreateUnitActionButtons();
        UpdateSelectedActionVisual();
    }


    #region EventHandlers

    

   
    
    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPointsText();
    }
    
    
    private void UnitActionSystem_UI_OnSelectedUnitChanged(object sender,EventArgs e)
    {
        CreateUnitActionButtons();
        UpdateSelectedActionVisual();
        UpdateActionPointsText();
    }
    
    private void UnitActionSystem_UI_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateSelectedActionVisual();
        
    }

    private void UnitActionSystem_OnActionStarted(object sender, EventArgs e)
    {
        UpdateActionPointsText();
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateActionPointsText();
    }
    
    #endregion
    private void CreateUnitActionButtons()
    {

        foreach (Transform actionButtonTransform in actionButtonContainerTransform)
        {
            Destroy(actionButtonTransform.gameObject);
        }
        
        actionButtonUIList.Clear();
        
        
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        foreach (BaseAction baseAction in selectedUnit.GetBaseActionArray())
        {
          Transform actionButtonTransform =  Instantiate(actionButtonPrefab, actionButtonContainerTransform);
          ActionButton_UI actionButtonUI = actionButtonTransform.GetComponent<ActionButton_UI>();
          actionButtonUI.SetBaseAction(baseAction);
          
          actionButtonUIList.Add(actionButtonUI);
        }

        
    }

    private void UpdateSelectedActionVisual()
    {
        foreach (ActionButton_UI actionButtonUI in actionButtonUIList)
        {
            actionButtonUI.UpdateSelectedActionVisual();
        }
    }

    private void UpdateActionPointsText()
    {
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        actionPointsText.text = "Action Points: " + selectedUnit.GetActionPoints();
    }

  
}
