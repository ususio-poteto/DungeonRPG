using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField]
    MazeManager mazeManager;

    List<GameObject> enemies = new List<GameObject>();

    bool isPlayerTurn = true;

    bool isEnemyTurn = false;

    void Start()
    {
        enemies = mazeManager.GetEnemiesList();
    }
    void Update()
    {
        if (isEnemyTurn)
        {
            MoveEnemy();
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F2))
        {
            MoveEnemy();
        }
#endif
    }

    public void SwitchTurn()
    {
        isPlayerTurn = !isPlayerTurn;
        isEnemyTurn = !isEnemyTurn;
    }

    public bool GetPlayerTurn()
    {
        return isPlayerTurn;
    }

    void MoveEnemy()
    {
        foreach (var enemy in enemies)
        {
            var enemyController = enemy.GetComponent<EnemyController>();
            enemyController.MyTurn();
        }
    }
}