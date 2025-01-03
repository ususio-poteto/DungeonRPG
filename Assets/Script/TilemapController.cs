using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class TilemapController : MonoBehaviour
{
    [Header("���H�̑f��")]
    [SerializeField] Tilemap tilemap;
    [SerializeField] Tile wall_tile;
    [SerializeField] Tile path_tile;
    [SerializeField] Tile goal_tile;

    [Header("���H�����A���S���Y��")]
    [SerializeField] MazeBarMethod mazeBarMethod;
    [SerializeField] MazeDigMethod mazeDigMethod;
    [SerializeField] MazeWallMethod mazeWallMethod;

    [Header("�g�p������H�����A���S���Y��")]
    [SerializeField] bool isMazeBarMethod = false;
    [SerializeField] bool isMazeDigMethod = false;
    [SerializeField] bool isMazeWallMethod = false;

    [SerializeField] GameObject player;

    //�ʘH�̍��W�����郊�X�g
    List<Vector2Int> pathPosition;

    const int wall = 1;    
    const int path = 0;

    int[,] maze;

    void Start()
    {
        //�_�|���@
        if (isMazeBarMethod)
        {
            maze = mazeBarMethod.GenarateMaze(30, 30);
        }

        //���@��@
        if (isMazeDigMethod)
        {
            mazeDigMethod.Initialize(30, 30);
            maze = mazeDigMethod.CreateMaze();
        }

        //�ǐL�΂��@
        if (isMazeWallMethod)
        {
            mazeWallMethod.Initialize(30, 30);
            maze = mazeWallMethod.CreateMaze();
        }
        
        //���H�̕`��
        SetTile(maze);

        pathPosition=GetPathPosition(maze);
        
        //�v���C���[�̐���
        CreatePlayer();
    }

//�G�f�B�^�ł̂݃}�b�v�̍Đ����ł���悤�ɂ���B
#if UNITY_EDITOR
    void Update()
    {
        //���H�̍Đ���
        if (Input.GetKeyDown(KeyCode.R))
        {
            RecreateMaze();
        }

        //�_�|���@��L����
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            isMazeBarMethod = true;
            isMazeDigMethod = false;
            isMazeDigMethod = false;
        }

        //�_�|���@��L����
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            isMazeBarMethod = false;
            isMazeDigMethod = true;
            isMazeDigMethod = false;
        }

        //�_�|���@��L����
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            isMazeBarMethod = false;
            isMazeDigMethod = false;
            isMazeDigMethod = true;
        }
    }
#endif

    void SetTile(int[,] setmaze)
    {
        var maze = setmaze;
        for (int y = 0; y < maze.GetLength(0); y++)
        {
            for (int x = 0; x < maze.GetLength(1); x++)
            {
                if (maze[y, x] == wall) tilemap.SetTile(new Vector3Int(x - (maze.GetLength(1) / 2), y - (maze.GetLength(0) / 2), 0), wall_tile);
                else if(maze[y, x] == path) tilemap.SetTile(new Vector3Int(x - (maze.GetLength(1)/2), y - (maze.GetLength(0)/2), 0), path_tile);
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

    void CreatePlayer()
    {
        var rnd = Random.Range(0, pathPosition.Count);
        
        Vector2Int randomPosition = pathPosition[rnd];

        Vector3 worldPosition = tilemap.GetCellCenterLocal(new Vector3Int(randomPosition.x, randomPosition.y, 0));
        Debug.Log(worldPosition);
        Instantiate(player, worldPosition, Quaternion.identity);
    }

    public int[,] Getmaze()
    {
        return maze;
    }

    public void RecreateMaze()
    {
        Destroy(GameObject.Find("Player(Clone)"));
        pathPosition.Clear();

        //�_�|���@
        if (isMazeBarMethod)
        {
            maze = mazeBarMethod.GenarateMaze(30, 30);
        }

        //���@��@
        if (isMazeDigMethod)
        {
            mazeDigMethod.Initialize(30, 30);
            maze = mazeDigMethod.CreateMaze();
        }

        //�ǐL�΂��@
        if (isMazeWallMethod)
        {
            mazeWallMethod.Initialize(30, 30);
            maze = mazeWallMethod.CreateMaze();
        }

        pathPosition = GetPathPosition(maze);
        Debug.Log(pathPosition.Count);
        //���H�̕`��
        SetTile(maze);
        //�v���C���[�̐���
        CreatePlayer();
    }
}

