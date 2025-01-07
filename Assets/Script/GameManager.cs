using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    int stageLevel;
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
