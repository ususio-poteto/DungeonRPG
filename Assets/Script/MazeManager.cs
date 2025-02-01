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
    Tile route_tile;

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

    string callClass;

    public bool searchRoute = true;

    [SerializeField]
    List<GameObject> lowFloor;

    [SerializeField]
    List<GameObject> middleFloor;

    [SerializeField]
    List<GameObject> highFloor;

    void Start()
    { 
        CreateMaze();
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

        //Debug.Log($"stageLevel:{stageLevel}");
    }
#endif

    /// <summary>
    /// �ŒZ�o�H�T��
    /// </summary>
    /// <param name="playerPosition">"player�̌��݂̈ʒu(transform.position)"</param>
    public void SearchShortestPath()
    {
        System.Diagnostics.StackFrame caller = new System.Diagnostics.StackFrame(1);
        callClass = caller.GetMethod().ReflectedType.Name;
        if (searchRoute)
        {
            var playerPosition = GameObject.FindWithTag("Player").transform.position;
            Vector2Int start = new Vector2Int(Mathf.FloorToInt(playerPosition.x), Mathf.FloorToInt(playerPosition.y));
            List<Vector2Int> path = aStarAlgorithm.FindPath(maze, start, goalPosition);
            foreach(var pathItem in path)
            {
                maze[pathItem.x, pathItem.y] = route;
            }
            maze[goalPosition.x, goalPosition.y] = goal;
            SetTile(maze);
        }

        if (callClass == "EnemyCharactor")
        {
            searchRoute = false;
        }
    }

    void SetTile(int[,] setmaze)
    {
        var maze = setmaze;
        for(int row = 0; row < maze.GetLength(1); row++)
        {
            for(int col = 0; col < maze.GetLength(0); col++)
            {
                if (maze[row, col] == path) tilemap.SetTile(new Vector3Int(row - (maze.GetLength(1) / 2), col - (maze.GetLength(0) / 2), 0), path_tile);
                if (maze[row, col] == wall) tilemap.SetTile(new Vector3Int(row - (maze.GetLength(1) / 2), col - (maze.GetLength(0) / 2), 0), wall_tile);
                if (maze[row, col] == route) tilemap.SetTile(new Vector3Int(row - (maze.GetLength(1) / 2), col - (maze.GetLength(0) / 2), 0), route_tile);
                if (maze[row, col] == goal) tilemap.SetTile(new Vector3Int(row - (maze.GetLength(1) / 2), col - (maze.GetLength(0) / 2), 0), goal_tile);
            }
        }
    }

    List<Vector2Int> GetPathPosition(int[,] maze)
    {
        List<Vector2Int> position = new List<Vector2Int>();

        //Debug.Log($"MazeSize{maze.GetLength(0)}*{maze.GetLength(1)}");

        for (int row = 0; row < maze.GetLength(1); row++)
        {
            for(int col = 0; col < maze.GetLength(0); col++)
            {
                if (maze[col, row] == path)
                {
                    position.Add(new Vector2Int(col, row));
                }
            }
        }
        return position;
    }
    
    void CreatePlayer()
    {
        var rnd = Random.Range(0, pathPosition.Count);
        
        Vector2Int randomPosition = pathPosition[rnd];
        startPosition = pathPosition[rnd];
        Vector3 worldPosition = tilemap.GetCellCenterLocal(new Vector3Int(randomPosition.x, randomPosition.y, 0));
        Instantiate(player, worldPosition, Quaternion.identity);
        pathPosition.RemoveAt(rnd);
    }

    void CreateEnemy(int enemyNum)
    {
        stageLevel = gameManager.GetStageLevel();
        for(int i = 0; i < enemyNum; i++)
        {
            var rnd = Random.Range(0, pathPosition.Count);

            Vector2Int randomPosition = pathPosition[rnd];

            Vector3 worldPosition = tilemap.GetCellCenterLocal(new Vector3Int(randomPosition.x, randomPosition.y, 0));
            if (stageLevel <= 3)
            {
                var index = Random.Range(0, lowFloor.Count);
                enemy = lowFloor[index];
            }

            else if (stageLevel >= 4 && stageLevel < 9)
            {
                var index = Random.Range(0, middleFloor.Count);
                enemy = middleFloor[index];
            }

            else if (stageLevel >= 10 && stageLevel <= 15)
            {
                var index = Random.Range(0, highFloor.Count);
                enemy = highFloor[index];
            }
            var createEnemy = Instantiate(enemy, worldPosition, Quaternion.identity);
            var enemyCharactor = createEnemy.GetComponent<EnemyCharactor>();
            enemies.Add(createEnemy);
            enemyCharactor.SetNum(i);
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

    void CreateGoal()
    {
        var row = Random.Range(0, maze.GetLength(1));
        var col = Random.Range(0, maze.GetLength(0));

        if (maze[row, col] == path)
        {
            maze[row, col] = goal;
            goalPosition = new Vector2Int(row, col);
        }

        else CreateGoal();
    }

    /// <summary>
    /// ���H��񂪓������z��̎擾
    /// </summary>
    public int[,] GetMaze()
    {
        return maze;
    }

    public void RecreateMaze()
    {
        Destroy(GameObject.FindWithTag("Player"));
        DestroyEnemy();
        pathPosition.Clear();

        CreateMaze();
    }

    void CreateMaze()
    {
        stageLevel = gameManager.GetStageLevel();
        searchRoute = true;

        MazeBarMethod(20, 20);
        CreateEnemy(6);

        //if (stageLevel <= 16)
        //{
        //    //Debug.Log("StageManager:MazeBarMethod");
        //    MazeBarMethod(20, 20);
        //    CreateEnemy(1);
        //}

        //else if (stageLevel >= 17 && stageLevel < 18)
        //{
        //    //Debug.Log("StageManager:MazeDigMethod");
        //    MazeDigMethod(20, 20);
        //    CreateEnemy(1);
        //}

        //else if (stageLevel >= 17 && stageLevel <= 18)
        //{
        //    //Debug.Log("StageManager:MazeWallMethod");
        //    MazeWallMethod(20, 20);
        //    CreateEnemy(1);
        //}
    }

    ///<summary>
    ///�_�|���@�����s���邽�߂̊֐�
    ///</summary>
    public void MazeBarMethod(int row,int col)
    {
        maze = mazeBarMethod.GenarateMaze(row, col);

        CreateGoal();

        //���H�̕`��
        SetTile(maze);

        pathPosition = GetPathPosition(maze);

        //�v���C���[�̐���
        CreatePlayer();
    }

    ///<summary>
    ///���@��@�����s���邽�߂̊֐�
    /// </summary>
    public void MazeDigMethod(int row,int col)
    {
        mazeDigMethod.Initialize(row, col);
        maze = mazeDigMethod.CreateMaze();

        CreateGoal();

        //���H�̕`��
        SetTile(maze);

        pathPosition = GetPathPosition(maze);

        //�v���C���[�̐���
        CreatePlayer();
    }
    
    ///<summary>
    ///�ǐL�΂��@�����s���邽�߂̊֐�
    /// </summary>
    public void MazeWallMethod(int row,int col)
    {
        mazeWallMethod.Initialize(row, col);
        maze = mazeWallMethod.CreateMaze();

        CreateGoal();

        //���H�̕`��
        SetTile(maze);

        pathPosition = GetPathPosition(maze);

        //�v���C���[�̐���
        CreatePlayer();
    }


    public Vector2Int GetPlayerStartPosition()
    {
        return startPosition;
    }
    
    public List<GameObject> GetEnemiesList()
    {
        return enemies;
    }
}

