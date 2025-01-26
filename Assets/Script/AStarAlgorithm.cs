using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Animations;

public class AStarAlgorithm : MonoBehaviour
{
    public class Node
    {
        //現在位置
        public Vector2Int position;
        public Node parent;
        public int G;//移動コスト
        public int H;//ヒューリスティックコスト
        public int F => G + H;//総コスト
    }

    /// <summary>
    /// A*アルゴリズムを実行(最短経路を探索)
    /// </summary>
    /// <param name="maze">迷路情報</param>
    /// <param name="start">始まりの位置(自身の座標)</param>
    /// <param name="goal">目標地点(ゴール)</param>
    /// <returns></returns>
    public List<Vector2Int> FindPath(int[,] maze,Vector2Int start,Vector2Int goal)
    {
        Debug.Log($"Start{start}");
        Debug.Log($"goal{goal}");
        //通路と壁の設定
        const int path_num = 0;
        const int wall_num = 1;
        const int goal_num = 2;

        int width = maze.GetLength(0);
        int height = maze.GetLength(1);

        var openList = new List<Node>();
        var closeList = new HashSet<Vector2Int>();

        Node startNode = new Node { position = start, G = 0, H = Heuristic(start, goal) };
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            //f値が最小のノードを取得
            Node currentNode = openList[0];
            foreach(var node in openList)
            {
                if (node.F < currentNode.F || (node.F == currentNode.F && node.H < currentNode.H))
                {
                    currentNode = node;
                }
            }

            //ゴールに到達したとき、経路を生成して返す
            if (currentNode.position == goal)
            {
                return ConstructPath(currentNode);
            }

            //オープンリストから削除してクローズリストに追加
            openList.Remove(currentNode);
            closeList.Add(currentNode.position);

            //隣接ノードをチェック
            foreach(var direction in GetNeighbors())
            {
                Vector2Int neighborPos = currentNode.position + direction;

                //範囲外または壁の場合スキップ
                if(neighborPos.x<0||neighborPos.x>=width||neighborPos.y<0||neighborPos.y>=height||
                    maze[neighborPos.x, neighborPos.y] == wall_num || closeList.Contains(neighborPos))
                {
                    continue;
                }

                //隣接ノードのコストを計算
                int gCost = currentNode.G + 1;
                Node neighborNode = openList.Find(n => n.position == neighborPos);

                if(neighborNode == null)
                {
                    neighborNode = new Node
                    {
                        position = neighborPos,
                        parent = currentNode,
                        G = gCost,
                        H = Heuristic(neighborPos, goal)
                    };
                    openList.Add(neighborNode); 
                }
                else if (gCost > neighborNode.G)
                {
                    //コストが改善された場合
                    neighborNode.G = gCost;
                    neighborNode.parent = currentNode;
                }
            }
        }
        //ゴールに到達できなかった場合
        return null;
    }

    private static int Heuristic(Vector2Int a,Vector2Int b)
    {
        return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
    }

    private static List<Vector2Int> GetNeighbors()
    {
        return new List<Vector2Int>
        {
            new Vector2Int(0,1),//上
            new Vector2Int(1,0),//右
            new Vector2Int(0,-1),//下
            new Vector2Int(-1,0)//左
        };
    }

    private static List<Vector2Int> ConstructPath(Node node)
    {
        var path = new List<Vector2Int>();
        while (node != null)
        {
            path.Add(node.position);
            node = node.parent;
        }
        //スタートからゴールの順にする
        path.Reverse();
        return path;
    }
}
