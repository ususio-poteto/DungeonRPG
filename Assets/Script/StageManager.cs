using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField]
    TilemapController tilemapController;

    [SerializeField]
    GameManager gameManager;
    int stageLevel;//ŠK‘w‚ð•\‚µ‚Ü‚·

    // Start is called before the first frame update
    public void CreateStage()
    {
        stageLevel=gameManager.GetStageLevel();
        Debug.Log(stageLevel);
        if (stageLevel <= 5)
        {
            Debug.Log("StageManager:MazeBarMethod");
            tilemapController.MazeBarMethod(25, 25);
        }

        else if (stageLevel >= 6 && stageLevel <= 10)
        {
            Debug.Log("StageManager:MazeBarMethod");
            tilemapController.MazeBarMethod(30, 30);
        }

        else if (stageLevel >= 11 && stageLevel < 15)
        {
            Debug.Log("StageManager:MazeDigMethod");
            tilemapController.MazeDigMethod(25, 25);
        }

        else if (stageLevel >= 16 && stageLevel < 20)
        {
            Debug.Log("StageManager:MazeDigMethod");
            tilemapController.MazeDigMethod(30, 30);
        }

        else if (stageLevel >= 21 && stageLevel <= 25)
        {
            Debug.Log("StageManager:MazeWallMethod");
            tilemapController.MazeWallMethod(25, 25);
        }

        else if (stageLevel >= 26 && stageLevel <= 30)
        {
            Debug.Log("StageManager:MazeWallMethod");
            tilemapController.MazeWallMethod(30, 30);
        }
    }
}
