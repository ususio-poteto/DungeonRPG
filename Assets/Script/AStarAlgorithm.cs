using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Animations;

public class AStarAlgorithm : MonoBehaviour
{
    public class Node
    {
        //���݈ʒu
        public Vector2Int position;
        public Node parent;
        public int G;//�ړ��R�X�g
        public int H;//�q���[���X�e�B�b�N�R�X�g
        public int F => G + H;//���R�X�g
    }

    /// <summary>
    /// A*�A���S���Y�������s(�ŒZ�o�H��T��)
    /// </summary>
    /// <param name="maze">���H���</param>
    /// <param name="start">�n�܂�̈ʒu(���g�̍��W)</param>
    /// <param name="goal">�ڕW�n�_(�S�[��)</param>
    /// <returns></returns>
    public List<Vector2Int> FindPath(int[,] maze,Vector2Int start,Vector2Int goal)
    {
        Debug.Log($"Start{start}");
        Debug.Log($"goal{goal}");
        //�ʘH�ƕǂ̐ݒ�
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
            //f�l���ŏ��̃m�[�h���擾
            Node currentNode = openList[0];
            foreach(var node in openList)
            {
                if (node.F < currentNode.F || (node.F == currentNode.F && node.H < currentNode.H))
                {
                    currentNode = node;
                }
            }

            //�S�[���ɓ��B�����Ƃ��A�o�H�𐶐����ĕԂ�
            if (currentNode.position == goal)
            {
                return ConstructPath(currentNode);
            }

            //�I�[�v�����X�g����폜���ăN���[�Y���X�g�ɒǉ�
            openList.Remove(currentNode);
            closeList.Add(currentNode.position);

            //�אڃm�[�h���`�F�b�N
            foreach(var direction in GetNeighbors())
            {
                Vector2Int neighborPos = currentNode.position + direction;

                //�͈͊O�܂��͕ǂ̏ꍇ�X�L�b�v
                if(neighborPos.x<0||neighborPos.x>=width||neighborPos.y<0||neighborPos.y>=height||
                    maze[neighborPos.x, neighborPos.y] == wall_num || closeList.Contains(neighborPos))
                {
                    continue;
                }

                //�אڃm�[�h�̃R�X�g���v�Z
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
                    //�R�X�g�����P���ꂽ�ꍇ
                    neighborNode.G = gCost;
                    neighborNode.parent = currentNode;
                }
            }
        }
        //�S�[���ɓ��B�ł��Ȃ������ꍇ
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
            new Vector2Int(0,1),//��
            new Vector2Int(1,0),//�E
            new Vector2Int(0,-1),//��
            new Vector2Int(-1,0)//��
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
        //�X�^�[�g����S�[���̏��ɂ���
        path.Reverse();
        return path;
    }
}
