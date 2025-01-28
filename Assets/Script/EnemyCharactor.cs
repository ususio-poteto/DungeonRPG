using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class EnemyCharactor : MonoBehaviour, IDamagable
{
    [SerializeField]
    int maxHitPoint;

    int hitPoint;

    [SerializeField]
    int EXP;

    TurnManager turnManager;

    public int num;

    void Start()
    {
        hitPoint = maxHitPoint;
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
    }

    void Update()
    {
        if (hitPoint <= 0)
        {
            Death();
        }
    }

    public void TakeDamage(int damage)
    {
        hitPoint -= damage;
    }

    public void Death()
    {
        var player=GameObject.FindWithTag("Player").GetComponent<PlayerCharacter>();
        player.GetEXP(EXP);
        turnManager.RemoveEnemies(0);
        Destroy(this.gameObject);
    }

    public void SetNum(int setNum)
    {
        num = setNum;
    }

    public int GetNum()
    {
        return num;
    }
}
