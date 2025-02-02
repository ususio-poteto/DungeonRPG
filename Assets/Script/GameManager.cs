using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;


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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F10))
        {
            SceneManager.LoadScene("GameOverScene");
        }

        if (Input.GetKeyDown(KeyCode.F11))
        {
            SceneManager.LoadScene("ClearScene");
        }
#endif
    }

    public int GetStageLevel()
    {
        return stageLevel;
    }

    public void isGoal()
    {
        
        if (stageLevel == 15)
        {
            SceneManager.LoadScene("ClearScene");
        }
        stageLevel++;
        mazeManager.RecreateMaze();
    }

    public void isDead()
    {   level = 1;
        stageLevel = 1;
        mazeManager.RecreateMaze();      
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
