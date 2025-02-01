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
            if (enemies != null)
            {
                var enemyController = enemy.GetComponent<EnemyController>();
                enemyController.MyTurn();
            }
        }
        SwitchTurn();
    }

    public void RemoveEnemies(int num)
    {
        enemies.RemoveAt(num);
        Debug.Log($"num:{num}");
        Debug.Log($"EnemiseCount:{enemies.Count}");

        //foreach(var enemy in enemies)
        //{
        //    var enemyCharactor = enemy.GetComponent<EnemyCharactor>();
        //    enemyCharactor.DecrementNum();
        //}  

        for (int i = num; i < enemies.Count; i++)
        {
            var enemyCharactor = enemies[i].GetComponent<EnemyCharactor>();
            enemyCharactor.DecrementNum();
        }
    }
}