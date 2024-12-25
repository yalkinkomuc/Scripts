using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ActionButton_UI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private GameObject selectedGameObject;
    
    private BaseAction baseAction;
    
    public void SetBaseAction(BaseAction baseAction)
    {
        this.baseAction = baseAction;
        textMeshPro.text = baseAction.GetActionName().ToUpper();
        button.onClick.AddListener(() =>
        {
            UnitActionSystem.Instance.SetSelectedAction(baseAction);
        });
    }

    public void UpdateSelectedActionVisual()
    {
        BaseAction selectedBaseAction = UnitActionSystem.Instance.GetSelectedAction();
        selectedGameObject.SetActive(selectedBaseAction ==baseAction);
    }

  
    
}
