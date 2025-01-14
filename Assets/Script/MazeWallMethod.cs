using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MazeWallMethod : MonoBehaviour
{
    const int path = 0;
    const int wall = 1;
    const int goal = 2;

    int width;
    int height;

    int[,] maze;

    Vector2Int goalPosition;

    enum direction
    {
        up,
        right,
        down,
        left
    }

    System.Random random;

    Stack<Vector2Int> currentWallCells;

    List<Vector2Int> startCells;

    public void Initialize(int setwidth, int setheight)
    {
        if (setwidth % 2 == 0) setwidth++;
        if (setheight % 2 == 0) setheight++;

        width = setwidth;
        height = setheight;

        maze = new int[width, height];

        startCells = new List<Vector2Int>();
        currentWallCells = new Stack<Vector2Int>();
        random = new System.Random();
    }

    //���H�𐶐�����
    public int[,] CreateMaze()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //�O����ǂɂ��Ă����A�J�n���Ƃ��ĕێ�
                if (x == 0 || y == 0 || x == width - 1 || y == height - 1) maze[x, y] = wall;
                else
                {
                    maze[x, y] = path;
                    if (x % 2 == 0 && y % 2 == 0)
                    {
                        startCells.Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        //�ǂ��g���ł��Ȃ��Ȃ�܂Ń��[�v
        while (startCells.Count > 0)
        {
            //�����_���ɊJ�n�Z�����擾���A�J�n��₩��폜
            var index = random.Next(startCells.Count);
            var cell = startCells[index];
            startCells.RemoveAt(index);
            var x = cell.x;
            var y = cell.y;

            //���łɕǂ̏ꍇ�͉������Ȃ�
            if (maze[x, y] == path)
            {
                //�g�����̕Ǐ���������
                currentWallCells.Clear();
                ExtendWall(x, y);
            }
        }
        CreateGoal();
        return maze;
    }

    //�ǂ��g������
    void ExtendWall(int x, int y)
    {
        //�L�΂����Ƃ��ł������(1�}�X�悪�ʘH��2�}�X��܂Ŕ͈͓�)
        //2�}�X�悪�ǂŎ������g�̏ꍇ�A�L�΂�
        var directions = new List<direction>();
        if (maze[x, y - 1] == path && !IsCurrentWall(x, y - 2)) directions.Add(direction.up);
        if (maze[x + 1, y] == path && !IsCurrentWall(x + 2, y)) directions.Add(direction.right);
        if (maze[x, y + 1] == path && !IsCurrentWall(x, y + 2)) directions.Add(direction.down);
        if (maze[x - 1, y] == path && !IsCurrentWall(x + 2, y)) directions.Add(direction.left);

        //�����_���ɐL�΂�
        if (directions.Count > 0)
        {
            //�ǂ��쐬(���̒n�_����L�΂�)
            SetWall(x, y);

            //�L�΂��悪�ʘH�̏ꍇ�́A�g���𑱂���
            var isPath = false;
            var dirIndex = random.Next(directions.Count);
            switch (directions[dirIndex])
            {
                case direction.up:
                    isPath = (maze[x, y - 2] == path);
                    SetWall(x, --y);
                    SetWall(x, --y);
                    break;
                case direction.right:
                    isPath = (maze[x + 2, y] == path);
                    SetWall(++x, y);
                    SetWall(++x, y);
                    break;
                case direction.down:
                    isPath = (maze[x, y + 2] == path);
                    SetWall(x, ++y);
                    SetWall(x, ++y);
                    break;
                case direction.left:
                    isPath = (maze[x - 2, y] == path);
                    SetWall(--x, y);
                    SetWall(--x, y);
                    break;
            }
            //�����̕ǂɐڑ��ł��Ă��Ȃ��ꍇ�͊g�����s
            if (isPath)
            {
                ExtendWall(x, y);
            }
        }
        else
        {
            if (currentWallCells.Count > 0)
            {
                var beforeCell = currentWallCells.Pop();
                ExtendWall(beforeCell.x, beforeCell.y);
            }
        }
    }

    void SetWall(int x, int y)
    {
        maze[x, y] = wall;
        if (x % 2 == 0 && y % 2 == 0) currentWallCells.Push(new Vector2Int(x, y));
    }

    bool IsCurrentWall(int x, int y)
    {
        return currentWallCells.Contains(new Vector2Int(x, y));
    }

    void CreateGoal()
    {
        int x = Random.Range(1, maze.GetLength(1));
        int y = Random.Range(1, maze.GetLength(0));
        if (maze[x, y] == path)
        {
            maze[x, y] = goal;
            goalPosition= new Vector2Int(x, y);
        }
        else
        {
            CreateGoal();
        }
    }

    public Vector2Int GetGoalPosition()
    {
        return goalPosition;
    }
}
