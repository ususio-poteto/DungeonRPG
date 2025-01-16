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
        new Vector2Int(0,1),//��
        new Vector2Int(1,0),//�E
        new Vector2Int(0,-1),//��
        new Vector2Int(-1,0)//��
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
    /// ���D��T�������s����
    /// </summary>
    List<Vector2Int> BFS(Vector2Int start, Vector2Int goal)
    {
        int height = maze.GetLength(0);
        int width = maze.GetLength(1);

        //�K��ς݂̃Z�����L�^����
        bool[,] visited = new bool[height,width];
        visited[start.y, start.x] = true;

        //�T���L���[(���݂̃Z���ƌo�H��ێ�)
        Queue<(Vector2Int position, List<Vector2Int> path)> queue = new Queue<(Vector2Int, List<Vector2Int>)>();
        queue.Enqueue((start, new List<Vector2Int> { start }));

        while (queue.Count > 0)
        {
            var (current, path) = queue.Dequeue();

            //�S�[���ɓ��B�����Ƃ�
            if(current == goal)
            {
                return path;
            }

            //4�������m�F
            foreach(var direction in directions)
            {
                Vector2Int neighbor = current + direction;

                //�z��͈͓̔��ł���A�ǂł͂Ȃ��A���K��ł��邩���m�F
                if (neighbor.y >= 0 && neighbor.y < height &&
                    neighbor.x >= 0 && neighbor.x < width &&
                    maze[neighbor.y, neighbor.x] == path_num &&
                    !visited[neighbor.y, neighbor.x])
                {
                    visited[neighbor.y, neighbor.x] = true;

                    //�V�����o�H���쐬���ăL���[�ɒǉ�
                    var newPath = new List<Vector2Int>(path) { neighbor };
                    queue.Enqueue((neighbor, newPath));
                }
            
            }
        }
        return null;
    }
}
