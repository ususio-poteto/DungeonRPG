using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MazeWallMethod : MonoBehaviour
{
    const int path = 0;
    const int wall = 1;

    int width;
    int height;

    int[,] maze;

    enum directions
    {
        up,
        right,
        down,
        left
    }

    System.Random random;

    Stack<Vector2Int> currentWallCells;

    List<Vector2Int> startCells;

    public void Initialize(int setWidth,int setHeight)
    {
        if (setWidth % 2 == 0) setWidth++;
        if (setHeight % 2 == 0) setHeight++;

        width = setWidth;
        height = setHeight;

        maze = new int[width, height];

        startCells = new List<Vector2Int>();
        currentWallCells = new Stack<Vector2Int>();
        random = new System.Random();
    }

    //迷路を生成する
    public int[,] CreateMaze()
    {
        for(int row = 0; row < width; row++)
        {
            for(int col=0; col < height; col++)
            {
                //外周を壁にする
                if (row == 0 || col == 0 || row == width - 1 || col == height - 1) maze[row, col] = wall;
                //内側をすべて通路にする
                else
                {
                    maze[row, col] = path;
                    //開始候補として保持
                    if (row % 2 == 0 && col % 2 == 0) startCells.Add(new Vector2Int(row, col));
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
            var row = cell.x;
            var col = cell.y;

            //すでに壁の場合は何もしない
            if (maze[row, col] == path)
            {
                //拡張中の壁の情報を初期化
                currentWallCells.Clear();
                ExtendWall(row, col);
            }
        }
        return maze;
    }

    //壁を拡張
    void ExtendWall(int row,int col)
    {
        //伸ばす事ができる方向(1マス先が通路で2マス先まで範囲内)
        var direction = new List<directions>();
        //if (maze[row, col - 1] == path && !IsCurrentWall(row, col - 2)) direction.Add(directions.up);
        //if (maze[row + 1, col] == path && !IsCurrentWall(row + 2, col)) direction.Add(directions.right);
        //if (maze[row, col + 1] == path && !IsCurrentWall(row, col + 2)) direction.Add(directions.down);
        //if (maze[row - 1, col] == path && !IsCurrentWall(row - 2, col)) direction.Add(directions.left);

        // 範囲チェックを追加して境界外アクセスを防ぐ
        if (col > 1 && maze[row, col - 1] == path && !IsCurrentWall(row, col - 2)) direction.Add(directions.up);
        if (row < width - 2 && maze[row + 1, col] == path && !IsCurrentWall(row + 2, col)) direction.Add(directions.right);
        if (col < height - 2 && maze[row, col + 1] == path && !IsCurrentWall(row, col + 2)) direction.Add(directions.down);
        if (row > 1 && maze[row - 1, col] == path && !IsCurrentWall(row - 2, col)) direction.Add(directions.left);

        //ランダムに伸ばす
        if (direction.Count > 0)
        {
            //壁を作成
            SetWall(row, col);

            //伸ばす先が通路なら拡張を続ける
            var isPath = false;
            var dirIndex = random.Next(direction.Count);
            switch (direction[dirIndex])
            {
                case directions.up:
                    isPath = (maze[row, col - 2] == path);
                    SetWall(row, --col);
                    SetWall(row, --col);
                    break;
                case directions.right:
                    isPath = (maze[row + 2, col] == path);
                    SetWall(++row, col);
                    SetWall(++row, col);
                    break;
                case directions.down:
                    isPath = (maze[row, col + 2] == path);
                    SetWall(row, ++col);
                    SetWall(row, ++col);
                    break;
                case directions.left:
                    isPath = (maze[row - 2, col] == path);
                    SetWall(--row, col);
                    SetWall(--row, col);
                    break;
            }

            //既存の壁に接続できていない場合は拡張続行
            if (isPath)
            {
                ExtendWall(row, col);
            }
        }
        else
        {
            if(currentWallCells.Count > 0)
            {
                var beforecell=currentWallCells.Pop();
                ExtendWall(beforecell.x,beforecell.y);
            }
        }
    }

    void SetWall(int row, int col)
    {
        maze[row, col] = wall;
        if (row % 2 == 0 && col % 2 == 0) currentWallCells.Push(new Vector2Int(row, col));
    }

    bool IsCurrentWall(int row,int col)
    {
        return currentWallCells.Contains(new Vector2Int(row, col));
    }
}
