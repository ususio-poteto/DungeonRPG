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
        stageLevel = 1;
    }
}
