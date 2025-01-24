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

    [SerializeField]
    GameObject player;

    Vector3 playerPosition;
    void Start()
    {
        
    }

    void Update()
    {
        stageText.text = "Stage" + stageLevel;
        playerPosition = player.transform.position;
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
        stageLevel = 1;
    }

    public Vector3 GetPlayerPosition()
    {
        return playerPosition;
    }
}
