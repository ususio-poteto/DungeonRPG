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
    [Header("迷路の素材")]
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

    [Header("迷路生成アルゴリズム")]
    [SerializeField] 
    MazeBarMethod mazeBarMethod;
    [SerializeField] 
    MazeDigMethod mazeDigMethod;
    [SerializeField] 
    MazeWallMethod mazeWallMethod;

    [Header("迷路探索アルゴリズム")]
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

    //通路の座標を入れるリスト
    List<Vector2Int> pathPosition;

    //プレイヤーのスタート座標(2次元配列の要素数)
    Vector2Int startPosition;

    List<GameObject> enemies = new List<GameObject>();

    const int path = 0;
    const int wall = 1;   
    const int goal = 2;
    const int route = 4;

    int[,] maze;

    Vector2Int goalPosition;

    int stageLevel;//階層を表します

    void Start()
    { 
        CreateStage();
    }

//エディタでのみマップの再生成できるようにする。
#if UNITY_EDITOR
    void Update()
    {
        //迷路の再生成
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
    /// 最短経路探索
    /// </summary>
    /// <param name="playerPosition">"playerの現在の位置(transform.position)"</param>
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
    /// 迷路情報が入った配列の取得
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
    ///棒倒し法を実行するための関数
    ///</summary>
    public void MazeBarMethod(int x,int y)
    {
        maze = mazeBarMethod.GenarateMaze(x, y);
        //迷路の描画
        SetTile(maze);

        pathPosition = GetPathPosition(maze);

        //プレイヤーの生成
        CreatePlayer();

        goalPosition = mazeBarMethod.GetGoalPosition();

    }

    ///<summary>
    ///穴掘り法を実行するための関数
    /// </summary>
    public void MazeDigMethod(int x,int y)
    {
        mazeDigMethod.Initialize(x, y);
        maze = mazeDigMethod.CreateMaze();
        //迷路の描画
        SetTile(maze);

        pathPosition = GetPathPosition(maze);

        //プレイヤーの生成
        CreatePlayer();

        goalPosition = mazeDigMethod.GetGoalPosition();
    }
    
    ///<summary>
    ///壁伸ばし法を実行するための関数
    /// </summary>
    public void MazeWallMethod(int x,int y)
    {
        mazeWallMethod.Initialize(x, y);
        maze = mazeWallMethod.CreateMaze();
        //迷路の描画
        SetTile(maze);

        pathPosition = GetPathPosition(maze);

        //プレイヤーの生成
        CreatePlayer();

        goalPosition = mazeWallMethod.GetGoalPosition();

    }


    public Vector2Int GetPlayerStartPosition()
    {
        return startPosition;
    }
}

