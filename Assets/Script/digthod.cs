using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class digMethod : MonoBehaviour
{
    const int path = 0;
    const int wall = 1;
    const int goal = 2;

    [Header("���H�̑f��")]
    [SerializeField] Tilemap tilemap;
    [SerializeField] Tile wall_tile;
    [SerializeField] Tile path_tile;
    [SerializeField] Tile goal_tile;

    [SerializeField] float DrawDelay;

    int[,] maze;

    int width;
    int height;

    List<Vector2Int> StartCells;

    enum direction
    {
        up = 0,
        right = 1,
        down = 2,
        left = 3
    }

    public void Initialize(int setwidth, int setheight)
    {
        if (setwidth % 2 == 0) setwidth++;
        if (setheight % 2 == 0) setheight++;

        width = setwidth;
        height = setheight;

        maze = new int[width, height];

        StartCells = new List<Vector2Int>();
    }

    public void SatartMazeGeneration()
    {
        StartCoroutine(CreateMazeCoroutine());
    }

    IEnumerator CreateMazeCoroutine()
    {
        //���ׂĕǂɂ���
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //�O���͔���̂��߂ɒʘH�ɂ��Ă���
                if (x == 0 || y == 0 || x == width - 1 || y == height - 1) maze[x, y] = path;
                else maze[x, y] = wall;
            }
        }
        yield return StartCoroutine(DigCoroutine(1, 1));

        //�O����ǂɖ߂�
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (x == 0 || y == 0 || x == width - 1 || y == height - 1) maze[x, y] = wall;
            }
        }
        CreateGoal();
        DrawDig();
        //return maze;
    }

    IEnumerator DigCoroutine(int x, int y)
    {
        System.Random rnd = new System.Random();

        while (true)
        {
            var directions = new List<direction>();
            if (maze[x, y - 1] == wall && maze[x, y - 2] == wall) directions.Add(direction.up);
            if (maze[x + 1, y] == wall && maze[x + 2, y] == wall) directions.Add(direction.right);
            if (maze[x, y + 1] == wall && maze[x, y + 2] == wall) directions.Add(direction.down);
            if (maze[x - 1, y] == wall && maze[x - 2, y] == wall) directions.Add(direction.left);

            //�@��i�߂��Ȃ��ꍇ�A���[�v�𔲂���
            if (directions.Count == 0) break;

            //�w����W��ʘH�Ƃ����@���₩��폜����
            SetPath(x, y);

            var dirIndex = directions[rnd.Next(directions.Count)];

            switch (dirIndex)
            {
                case direction.up:
                    SetPath(x, --y);
                    SetPath(x, --y);
                    break;
                case direction.right:
                    SetPath(++x, y);
                    SetPath(++x, y);
                    break;
                case direction.down:
                    SetPath(x, ++y);
                    SetPath(x, ++y);
                    break;
                case direction.left:
                    SetPath(--x, y);
                    SetPath(--x, y);
                    break;
            }
            DrawDig();
            yield return new WaitForSeconds(DrawDelay);
        }

        //�ǂ��ɂ����i�߂��Ȃ��ꍇ�A���@��J�n��₩��@�蒼��
        //�����W�����݂��Ȃ��Ƃ��A���@�芮��
        Vector2Int? cell = GetStartCell();
        //���������ōċA
        if (cell.HasValue) yield return StartCoroutine(DigCoroutine(cell.Value.x, cell.Value.y));
    }

    void SetPath(int x, int y)
    {
        maze[x, y] = path;
        if (x % 2 == 1 && y % 2 == 1) StartCells.Add(new Vector2Int(x, y));
    }

    Vector2Int? GetStartCell()
    {
        if (StartCells.Count == 0) return null;

        System.Random rnd = new System.Random();
        var index = rnd.Next(StartCells.Count);
        var cell = StartCells[index];
        StartCells.RemoveAt(index);

        return cell;
    }

    void CreateGoal()
    {
        int x = Random.Range(1, maze.GetLength(1));
        int y = Random.Range(1, maze.GetLength(0));
        if (maze[x, y] == path)
        {
            maze[x, y] = goal;
        }
        else
        {
            CreateGoal();
        }
    }

    void DrawDig()
    {
        for (int y = 0; y < maze.GetLength(0); y++)
        {
            for (int x = 0; x < maze.GetLength(1); x++)
            {
                if (maze[y, x] == wall) tilemap.SetTile(new Vector3Int(x - (maze.GetLength(1) / 2), y - (maze.GetLength(0) / 2), 0), wall_tile);
                else if (maze[y, x] == path) tilemap.SetTile(new Vector3Int(x - (maze.GetLength(1) / 2), y - (maze.GetLength(0) / 2), 0), path_tile);
                else tilemap.SetTile(new Vector3Int(x - (maze.GetLength(1) / 2), y - (maze.GetLength(0) / 2), 0), goal_tile);
            }
        }
    }
}
