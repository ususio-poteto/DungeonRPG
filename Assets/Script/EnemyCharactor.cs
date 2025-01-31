using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyCharactor : MonoBehaviour, IDamagable
{
    [SerializeField]
    int maxHitPoint;

    int hitPoint;

    [SerializeField]
    int EXP;

    TurnManager turnManager;

    MazeManager mazeManager;

    public int num;

    GameObject player;

    bool isInitialized=true;

    void Start()
    {
        isInitialized = false;
        hitPoint = maxHitPoint;
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        mazeManager = GameObject.Find("MazeManager").GetComponent<MazeManager>();
    }

    void Update()
    {
        if (!isInitialized) return;
        Debug.Log($"hitpoint:{hitPoint}");
        if (hitPoint <= 0)
        {
            player = GameObject.FindWithTag("Player");
            //�����Ŋm���Ń����_�����ʔ���
            GiveBuff();
            Death();
        }
    }

    public void TakeDamage(int damage)
    {
        hitPoint -= damage;
    }

    void Death()
    {
        Debug.Log("����!");
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
                    //�U���̓A�b�v
                    var playerController = player.GetComponent<PlayerController>();
                    playerController.AddAttackValue();
                    break;
                case 2:
                    //HP�����ʑ�����
                    var playerCharactor = player.GetComponent<PlayerCharacter>();
                    playerCharactor.Healing();
                    break;
                case 3:
                    //�ŒZ�o�H�T��
                    mazeManager.SearchShortestPath();
                    break;

            }
        }
    }

    public void SetNum(int setNum)
    {
        num = setNum;
    }

    public void DecrementNum()
    {
        if(num>0)num--; 
    }
}
