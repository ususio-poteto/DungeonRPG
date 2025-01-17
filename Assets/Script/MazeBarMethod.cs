using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MazeBarMethod : MonoBehaviour
{
    const int path = 0;
    const int wall = 1;

    int[,] maze;

    int width;
    int height;

    public int[,] GenarateMaze(int setWidth,int setHeight)
    {
        if (setWidth % 2 == 0) setWidth++;
        if (setHeight % 2 == 0) setHeight++;

        width = setWidth;
        height = setHeight;

        maze = new int[width, height];

        for(int row = 0; row < width; row++)
        {
            for(int col = 0; col < height; col++)
            {
                //外周すべてを壁にする
                if (row == 0 || col == 0 || row == width - 1 || col == height - 1) maze[row, col] = wall;
                else maze[row, col] = path;
            }
        }

        for(int row = 2; row < width - 1; row += 2)
        {
            for(int col = 2; col < height - 1; col += 2)
            {
                maze[row, col] = wall;//1マスおきに壁を立てる
                while (true)
                {
                    int direction;
                    if (col == 2) direction = Random.Range(0, 4);
                    else direction = Random.Range(0,3);

                    int wallX = row;
                    int wallY = col;

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
                        case 3:
                            wallY--;
                            break;
                    }

                    //壁じゃない場合のみ倒して終了
                    if (maze[wallX, wallY] != wall)
                    {
                        maze[wallX, wallY] = wall;
                        break;
                    }
                }
            }
        }
        return maze;
    }
}
