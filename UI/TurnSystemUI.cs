using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField] private Button turnButton;
    [SerializeField] private TextMeshProUGUI turnNumberText;
    [SerializeField] private GameObject enemyTurnVisualGameObject;
    private void Start()
    {
        turnButton.onClick.AddListener(() =>
        {
            TurnSystem.instance.NextTurn();
        });

        TurnSystem.instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        
        UpdateTurnText();
        UpdateEnemyTurnViusal();
        UpdateEndTurnButtonVisibility();

    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateTurnText();
        UpdateEnemyTurnViusal();
        UpdateEndTurnButtonVisibility();
    }
    
    private void UpdateTurnText()
    {
        turnNumberText.text = "TURN " + TurnSystem.instance.GetTurnNumber();
    }

    private void UpdateEnemyTurnViusal()
    {
        enemyTurnVisualGameObject.SetActive(!TurnSystem.instance.IsPlayerTurn());
    }

    private void UpdateEndTurnButtonVisibility()
    {
        turnButton.gameObject.SetActive(TurnSystem.instance.IsPlayerTurn());
    }
}
