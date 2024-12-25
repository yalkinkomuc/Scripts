using System;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private float timer;


    private void Start()
    {
        TurnSystem.instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    // Update is called once per frame
    void Update()
    {
        if (TurnSystem.instance.IsPlayerTurn())
        {
            return;
        }
        
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            TurnSystem.instance.NextTurn();
        }
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        timer = 1f;
    }
}
