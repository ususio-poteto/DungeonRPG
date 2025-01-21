using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharactor : MonoBehaviour, IDamagable
{
    [SerializeField]
    int maxHitPoint;

    int hitPoint;

    [SerializeField]
    int EXP;

    void Start()
    {
        hitPoint = maxHitPoint;
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

    }

    public void Death()
    {
        var player=GameObject.Find("Player").GetComponent<PlayerCharacter>();
        player.GetEXP(EXP);
        Destroy(this.gameObject);
    }
}
