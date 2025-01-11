using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthFirstSearch : MonoBehaviour
{
    MazeManager mazeManager;

    int[,] maze;

    int[,] visitedArray;

    int mazeWidth;

    int mazeHeight;

    Vector2Int PlayerGridPosition;
    // Start is called before the first frame update
    void Start()
    {
        mazeManager = GetComponent<MazeManager>();
        maze = mazeManager.GetMaze();
        mazeWidth = maze.GetLength(0);
        mazeHeight= maze.GetLength(1);
        visitedArray = new int[mazeWidth , mazeHeight];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
