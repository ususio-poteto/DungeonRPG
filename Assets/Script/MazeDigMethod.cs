using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MazeDigMethod : MonoBehaviour
{
    const int path = 0;
    const int wall = 1;

    int[,] maze;

    int width;
    int height;

    List<Vector2Int> StartCells;

    enum directions
    {
        up=0,
        right=1,
        down=2,
        left=3,
    }

    public void Initialize(int setWidth,int setHeight)
    {
        if(setHeight%2==0) setHeight++;
        if(setWidth%2==0) setWidth++;

        width = setWidth;
        height = setHeight;

        maze=new int[width,height];

        StartCells = new List<Vector2Int>();
    }

    public int[,] CreateMaze()
    {
        //すべて壁にする
        for(int row = 0; row < width; row++)
        {
            for (int col = 0; col < height; col++)
            {
                if (row == 0 || col == 0 || row == width - 1 || col == height - 1) maze[row, col] = path;//外周は判定のために通路にする
                else maze[row, col] = wall;
            }
        }
        Dig(1, 1);

        //外周を壁に戻す
        for(int row = 0; row < width; row++)
        {
            for(int col = 0; col < height; col++)
            {
                if (row == 0 || col == 0 || row == width - 1 || col == height - 1) maze[row, col] = wall;
            }
        }
        return maze;
    }

    void Dig(int row,int col)
    {
        System.Random rnd = new System.Random();

        while(true)
        {
            var direction = new List<directions>();
            if (maze[row, col - 1] == wall && maze[row, col - 2] == wall) direction.Add(directions.up);
            if (maze[row + 1, col] == wall && maze[row + 2, col] == wall) direction.Add(directions.right);
            if (maze[row, col + 1] == wall && maze[row, col + 2] == wall) direction.Add(directions.down);
            if (maze[row - 1, col] == wall && maze[row - 2, col] == wall) direction.Add(directions.left);

            //掘り進められない場合はループから抜ける
            if (direction.Count == 0) break;

            //指定座標を通路として穴掘り候補から削除する
            SetPath(row, col);

            var dirIndex = direction[rnd.Next(direction.Count)];

            switch (dirIndex)
            {
                case directions.up:
                    SetPath(row, --col);
                    SetPath(row, --col);
                    break;
                case directions.right:
                    SetPath(++row, col);
                    SetPath(++row, col);
                    break;
                case directions.down:
                    SetPath(row, ++col);
                    SetPath(row, ++col);
                    break;
                case directions.left:
                    SetPath(--row, col);
                    SetPath(--row, col);
                    break;
            }
        }

        //どこにも進められない場合、穴掘り候補からやり直し
        //候補座標が存在しないとき、穴掘り完了
        Vector2Int? cell = GetStartCell();

        //ここで再帰
        if (cell.HasValue)Dig(cell.Value.x, cell.Value.y);
    }

    void SetPath(int row,int col)
    {
        maze[row, col] = path;
        if (row % 2 == 1 && col % 2 == 1) StartCells.Add(new Vector2Int(row, col));
    }

    Vector2Int? GetStartCell()
    {
        if(StartCells.Count == 0) return null;

        System.Random rnd = new System.Random();
        var index = rnd.Next(StartCells.Count);
        var cell = StartCells[index];
        StartCells.RemoveAt(index);

        return cell;
    }
}
