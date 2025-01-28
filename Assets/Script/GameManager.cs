using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    int stageLevel;

    [SerializeField]
    TextMeshProUGUI stageText;

    int level;

    int HP;

    [SerializeField]
    MazeManager mazeManager;

    Vector3 playerPosition;
    void Start()
    {
        
    }

    void Update()
    {
        stageText.text = "Stage" + stageLevel; 
    }

    public int GetStageLevel()
    {
        return stageLevel;
    }

    public void isGoal()
    {
        stageLevel++;
    }

    public void isDead()
    {
        mazeManager.RecreateMaze();
        level = 1;
        stageLevel = 1;
    }

    public Vector3 GetPlayerPosition()
    {
        return playerPosition;
    }

    public int GetPlayerLevel()
    {
        return level;
    }

    public int GetPlayerHP() 
    {
        return HP; 
    }

    public void SetPlayerLevel(int setLevel)
    {
        level = setLevel;
    }

    public void SetPlayerHP(int setHP)
    {
        HP = setHP;
    }
}
