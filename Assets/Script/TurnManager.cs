using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    bool isPlayerTurn = true;

    bool isEnemyTurn = false;

    public void SwitchTurn()
    {
        isPlayerTurn = !isPlayerTurn;
        isEnemyTurn = !isEnemyTurn;
    }

    public bool GetPlayerTurn()
    {
        return isPlayerTurn;
    }
    
    public bool GetEnemyTurn()
    {
        return isEnemyTurn;
    }
}