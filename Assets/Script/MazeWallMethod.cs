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

    //迷路を生成する
    public int[,] CreateMaze()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //外周を壁にしておき、開始候補として保持
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

        //壁が拡張できなくなるまでループ
        while (startCells.Count > 0)
        {
            //ランダムに開始セルを取得し、開始候補から削除
            var index = random.Next(startCells.Count);
            var cell = startCells[index];
            startCells.RemoveAt(index);
            var x = cell.x;
            var y = cell.y;

            //すでに壁の場合は何もしない
            if (maze[x, y] == path)
            {
                //拡張中の壁情報を初期化
                currentWallCells.Clear();
                ExtendWall(x, y);
            }
        }
        CreateGoal();
        return maze;
    }

    //壁を拡張する
    void ExtendWall(int x, int y)
    {
        //伸ばすことだできる方向(1マス先が通路で2マス先まで範囲内)
        //2マス先が壁で自分自身の場合、伸ばす
        var directions = new List<direction>();
        if (maze[x, y - 1] == path && !IsCurrentWall(x, y - 2)) directions.Add(direction.up);
        if (maze[x + 1, y] == path && !IsCurrentWall(x + 2, y)) directions.Add(direction.right);
        if (maze[x, y + 1] == path && !IsCurrentWall(x, y + 2)) directions.Add(direction.down);
        if (maze[x - 1, y] == path && !IsCurrentWall(x + 2, y)) directions.Add(direction.left);

        //ランダムに伸ばす
        if (directions.Count > 0)
        {
            //壁を作成(この地点から伸ばす)
            SetWall(x, y);

            //伸ばす先が通路の場合は、拡張を続ける
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
            //既存の壁に接続できていない場合は拡張続行
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
