using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MazeBarMethod : MonoBehaviour
{ 
    const int path = 0;
    const int wall = 1;
    const int goal = 2;

    int[,] maze;

    Vector2Int goalPosition;

    public int[,] GenarateMaze(int width, int height)
    {
        if (width % 2 == 0) width++;
        if (height % 2 == 0) height++;

        maze = new int[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //外周を壁にする
                if (x == 0 || y == 0 || x == width - 1 || y == height - 1) maze[y, x] = wall;
                //内側はすべて通れるようにする
                else maze[y, x] = path;
            }
        }
        
        for (int y = 2; y < height - 1; y += 2)
        {
            for (int x = 2; x < width - 1; x += 2)
            {
                maze[y, x] = wall;//1マスおきに壁を立てる

                while (true)
                {
                    int direction;
                    if (y == 2) direction = Random.Range(0, 4);
                    else direction = Random.Range(0, 3);

                    int wallX = x;
                    int wallY = y;

                    switch (direction)
                    {
                        //右
                        case 0:
                            wallX++;
                            break;
                        //下
                        case 1:
                            wallY++;
                            break;
                        //左
                        case 2:
                            wallX--;
                            break;
                        //上
                        case 3:
                            wallY--;
                            break;
                    }

                    //壁じゃない場合のみ倒して終了
                    if (maze[wallY, wallX] != wall)
                    {
                        maze[wallY, wallX] = wall;
                        break;
                    }
                }
            }
        }
        CreateGoal();
        return maze;
    }

    void CreateGoal()
    {
        int x = Random.Range(1, maze.GetLength(1));
        int y = Random.Range(1, maze.GetLength(0));
        if (maze[x, y] == path)
        {
            maze[x, y] = goal;
            goalPosition = new Vector2Int(x, y);
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
