using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MazeDigMethod : MonoBehaviour
{
    const int path = 0;
    const int wall = 1;
    const int goal = 2;

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

    public int[,] CreateMaze()
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
        Dig(1, 1);

        //�O����ǂɖ߂�
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (x == 0 || y == 0 || x == width - 1 || y == height - 1) maze[x, y] = wall;
            }
        }
        CreateGoal();
        return maze;
    }

    void Dig(int x, int y)
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
        }

        //�ǂ��ɂ����i�߂��Ȃ��ꍇ�A���@��J�n��₩��@�蒼��
        //�����W�����݂��Ȃ��Ƃ��A���@�芮��
        Vector2Int? cell = GetStartCell();
        //���������ōċA
        if (cell.HasValue) Dig(cell.Value.x, cell.Value.y);
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
}
