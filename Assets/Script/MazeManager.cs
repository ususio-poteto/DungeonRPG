using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using static UnityEngine.ParticleSystem;

public class MazeManager : MonoBehaviour
{
    [Header("���H�̑f��")]
    [SerializeField] 
    Tilemap tilemap;
    [SerializeField] 
    Tile wall_tile;
    [SerializeField] 
    Tile path_tile;
    [SerializeField] 
    Tile goal_tile;
    [SerializeField] 
    Tile shotesetTile;

    [Header("���H�����A���S���Y��")]
    [SerializeField] 
    MazeBarMethod mazeBarMethod;
    [SerializeField] 
    MazeDigMethod mazeDigMethod;
    [SerializeField] 
    MazeWallMethod mazeWallMethod;

    [Header("���H�T���A���S���Y��")]
    [SerializeField]
    BreadthFirstSearch breadthFirstSearch;
    [SerializeField]
    AStarAlgorithm aStarAlgorithm;

    [SerializeField] 
    GameObject player;

    [SerializeField] 
    GameObject enemy;

    [SerializeField] 
    GameManager gameManager;

    //�ʘH�̍��W�����郊�X�g
    List<Vector2Int> pathPosition;

    //�v���C���[�̃X�^�[�g���W(2�����z��̗v�f��)
    Vector2Int startPosition;

    List<GameObject> enemies = new List<GameObject>();

    const int path = 0;
    const int wall = 1;   
    const int goal = 2;
    const int route = 4;

    int[,] maze;

    Vector2Int goalPosition;

    int stageLevel;//�K�w��\���܂�

    void Start()
    { 
        CreateStage();
    }

//�G�f�B�^�ł̂݃}�b�v�̍Đ����ł���悤�ɂ���B
#if UNITY_EDITOR
    void Update()
    {
        //���H�̍Đ���
        if (Input.GetKeyDown(KeyCode.F1))
        {
            RecreateMaze();
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            DestroyEnemy();
        }

    }
#endif

    /// <summary>
    /// �ŒZ�o�H�T��
    /// </summary>
    /// <param name="playerPosition">"player�̌��݂̈ʒu(transform.position)"</param>
    public void SearchShortestPath(Vector3 playerPosition)
    {
        SetTile(maze);
    }

    void SetTile(int[,] setmaze)
    {
        var maze = setmaze;
        for (int y = 0; y < maze.GetLength(0); y++) 
        {
            for (int x = 0; x < maze.GetLength(1); x++)
            {
                if (maze[y, x] == wall) tilemap.SetTile(new Vector3Int(x - (maze.GetLength(1) / 2), y - (maze.GetLength(0) / 2), 0), wall_tile);
                else if (maze[y, x] == path) tilemap.SetTile(new Vector3Int(x - (maze.GetLength(1) / 2), y - (maze.GetLength(0) / 2), 0), path_tile);
                else if (maze[y, x] == route) tilemap.SetTile(new Vector3Int(y - (maze.GetLength(1) / 2), x - (maze.GetLength(0) / 2), 0), shotesetTile);
                else tilemap.SetTile(new Vector3Int(x - (maze.GetLength(1) / 2), y - (maze.GetLength(0) / 2), 0), goal_tile);
            }
        }
    }

    List<Vector2Int> GetPathPosition(int[,] setmaze)
    {
        List<Vector2Int> position=new List<Vector2Int>(); ;
        
        for (int y = 0; y < setmaze.GetLength(1); y++)
        {
            for(int x = 0;x < setmaze.GetLength(0); x++)
            {
                if (setmaze[y, x] == path)
                {
                    position.Add(new Vector2Int(x, y));
                }
            }
        }
        return position;
    }

    /*
    void CreatePlayer()
    {
        int x = Random.Range(1, maze.GetLength(1));
        int y = Random.Range(1, maze.GetLength(0));
        if (maze[x, y] == path)
        {
            Instantiate(player, new Vector3Int(x, y, 0), Quaternion.identity);
        }
        else
        {
            CreatePlayer();
        }
    }
    */

    
    void CreatePlayer()
    {
        var rnd = Random.Range(0, pathPosition.Count);
        
        Vector2Int randomPosition = pathPosition[rnd];
        startPosition = pathPosition[rnd];

        Vector3 worldPosition = tilemap.GetCellCenterLocal(new Vector3Int(randomPosition.x, randomPosition.y, 0));
        Debug.Log("position" + randomPosition.x + "," + randomPosition.y);
        //Debug.Log(maze[randomPosition.x, randomPosition.y]);
        Instantiate(player, worldPosition, Quaternion.identity);
        pathPosition.RemoveAt(rnd);
    }
    

    public int[,] GetMaze()
    {
        return maze;
    }

    void CreateEnemy(int enemyNum)
    {
        for(int i = 0; i < enemyNum; i++)
        {
            var rnd = Random.Range(0, pathPosition.Count);

            Vector2Int randomPosition = pathPosition[rnd];

            Vector3 worldPosition = tilemap.GetCellCenterLocal(new Vector3Int(randomPosition.x, randomPosition.y, 0));
            var createEnemy = Instantiate(enemy, worldPosition, Quaternion.identity);
            enemies.Add(createEnemy);
            pathPosition.RemoveAt(rnd);
        }

    }

    void DestroyEnemy()
    {
        foreach (var element in enemies)
        {
            Destroy(element.gameObject);
        }
        enemies.Clear();
    }

    /// <summary>
    /// ���H��񂪓������z��̎擾
    /// </summary>
    public int[,] Getmaze()
    {
        return maze;
    }

    public void RecreateMaze()
    {
        Destroy(GameObject.FindWithTag("Player"));
        DestroyEnemy();
        pathPosition.Clear();

        CreateStage();
    }

    void CreateStage()
    {
        if (stageLevel <= 10)
        {
            //Debug.Log("StageManager:MazeBarMethod");
            MazeBarMethod(30, 30);
            CreateEnemy(stageLevel+5);
        }

        else if (stageLevel >= 11 && stageLevel < 20)
        {
            //Debug.Log("StageManager:MazeDigMethod");
            MazeDigMethod(30, 30);
            CreateEnemy(stageLevel + 5);
        }

        else if (stageLevel >= 21 && stageLevel <= 30)
        {
            //Debug.Log("StageManager:MazeWallMethod");
            MazeWallMethod(30,30);
            CreateEnemy(stageLevel + 5);
        }
    }

    ///<summary>
    ///�_�|���@�����s���邽�߂̊֐�
    ///</summary>
    public void MazeBarMethod(int x,int y)
    {
        maze = mazeBarMethod.GenarateMaze(x, y);
        //���H�̕`��
        SetTile(maze);

        pathPosition = GetPathPosition(maze);

        //�v���C���[�̐���
        CreatePlayer();

        goalPosition = mazeBarMethod.GetGoalPosition();

    }

    ///<summary>
    ///���@��@�����s���邽�߂̊֐�
    /// </summary>
    public void MazeDigMethod(int x,int y)
    {
        mazeDigMethod.Initialize(x, y);
        maze = mazeDigMethod.CreateMaze();
        //���H�̕`��
        SetTile(maze);

        pathPosition = GetPathPosition(maze);

        //�v���C���[�̐���
        CreatePlayer();

        goalPosition = mazeDigMethod.GetGoalPosition();
    }
    
    ///<summary>
    ///�ǐL�΂��@�����s���邽�߂̊֐�
    /// </summary>
    public void MazeWallMethod(int x,int y)
    {
        mazeWallMethod.Initialize(x, y);
        maze = mazeWallMethod.CreateMaze();
        //���H�̕`��
        SetTile(maze);

        pathPosition = GetPathPosition(maze);

        //�v���C���[�̐���
        CreatePlayer();

        goalPosition = mazeWallMethod.GetGoalPosition();

    }


    public Vector2Int GetPlayerStartPosition()
    {
        return startPosition;
    }
}

