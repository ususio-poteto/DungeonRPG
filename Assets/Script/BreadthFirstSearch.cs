using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BreadthFirstSearch : MonoBehaviour
{
    [SerializeField]
    Tilemap tilemap;

    [SerializeField]
    Tile shotesetTile;

    const int path_num = 0;
    const int wall_num = 1;
    const int goal_num = 2;
    const int route = 4;

    Vector2Int[] directions =
    {
        new Vector2Int(0,1),//上
        new Vector2Int(1,0),//右
        new Vector2Int(0,-1),//下
        new Vector2Int(-1,0)//左
    };

    int[,] maze;

    [SerializeField]
    MazeManager mazeManager;

    public List<Vector2Int> Search(Vector3 playerPosition, Vector2Int goalPosition)
    {
        Debug.Log("BFS" + playerPosition);
        maze = mazeManager.GetMaze();
        Vector2Int start = new Vector2Int(Mathf.FloorToInt(playerPosition.x), Mathf.FloorToInt(playerPosition.y));
        //maze[start.x, start.y] = route;
        //Debug.Log("startPosition" + start.x + "," + start.y);
        Vector2Int goal = goalPosition;
        List<Vector2Int> path = BFS(start,goal);
        return path;
    }

    /// <summary>
    /// 幅優先探索を実行する
    /// </summary>
    List<Vector2Int> BFS(Vector2Int start, Vector2Int goal)
    {
        int height = maze.GetLength(0);
        int width = maze.GetLength(1);

        //訪問済みのセルを記録する
        bool[,] visited = new bool[height,width];
        visited[start.y, start.x] = true;

        //探索キュー(現在のセルと経路を保持)
        Queue<(Vector2Int position, List<Vector2Int> path)> queue = new Queue<(Vector2Int, List<Vector2Int>)>();
        queue.Enqueue((start, new List<Vector2Int> { start }));

        while (queue.Count > 0)
        {
            var (current, path) = queue.Dequeue();

            //ゴールに到達したとき
            if(current == goal)
            {
                return path;
            }

            //4方向を確認
            foreach(var direction in directions)
            {
                Vector2Int neighbor = current + direction;

                //配列の範囲内であり、壁ではなく、未訪問であるかを確認
                if (neighbor.y >= 0 && neighbor.y < height &&
                    neighbor.x >= 0 && neighbor.x < width &&
                    maze[neighbor.y, neighbor.x] == path_num &&
                    !visited[neighbor.y, neighbor.x])
                {
                    visited[neighbor.y, neighbor.x] = true;

                    //新しい経路を作成してキューに追加
                    var newPath = new List<Vector2Int>(path) { neighbor };
                    queue.Enqueue((neighbor, newPath));
                }
            
            }
        }
        return null;
    }
}
