using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyCharactor : MonoBehaviour, IDamagable
{
    [SerializeField]
    int maxHitPoint;

    public int hitPoint;

    [SerializeField]
    int EXP;

    TurnManager turnManager;

    MazeManager mazeManager;

    public int num;

    GameObject player;

    bool isDeath = false;//死ねるかどうか

    void Awake()
    {
        hitPoint = maxHitPoint;
        //Debug.Log($"hp:{hitPoint}");
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        mazeManager = GameObject.Find("MazeManager").GetComponent<MazeManager>();
    }

    void Update()
    {
        if (hitPoint <= 0)
        {
            player = GameObject.FindWithTag("Player");
            //ここで確率でランダム効果発動
            if(isDeath)
            {
                GiveBuff();
                Death();
            }
        }
    }

    public void TakeDamage(int damage)
    {
        hitPoint -= damage;
        isDeath = true;
    }

    void Death()
    {
        //Debug.Log("死んだ!");
        var playerCharactor = player.GetComponent<PlayerCharacter>();
        playerCharactor.GetEXP(EXP);
        Destroy(this.gameObject);
        turnManager.RemoveEnemies(num);
    }

    void GiveBuff()
    {
        float probability = Random.Range(0f, 100f);
        if( probability < 30f)
        {
            var randomBuff = Random.Range(1, 4);
            switch (randomBuff)
            {
                case 1:
                    //攻撃力アップ
                    var playerController = player.GetComponent<PlayerController>();
                    playerController.AddAttackValue();
                    break;
                case 2:
                    //HPを一定量即時回復
                    var playerCharactor = player.GetComponent<PlayerCharacter>();
                    playerCharactor.Healing(20);
                    break;
                case 3:
                    //最短経路探索
                    mazeManager.SearchShortestPath();
                    break;

            }
        }
    }

    public void SetNum(int setNum)
    {
        num = setNum;
    }

    public int GetNum()
    {
        return num;
    }

    public void DecrementNum()
    {
        if(num>0)num--; 
    }
}
