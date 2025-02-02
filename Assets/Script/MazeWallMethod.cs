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

    //���H�𐶐�����
    public int[,] CreateMaze()
    {
        for(int row = 0; row < width; row++)
        {
            for(int col=0; col < height; col++)
            {
                //�O����ǂɂ���
                if (row == 0 || col == 0 || row == width - 1 || col == height - 1) maze[row, col] = wall;
                //���������ׂĒʘH�ɂ���
                else
                {
                    maze[row, col] = path;
                    //�J�n���Ƃ��ĕێ�
                    if (row % 2 == 0 && col % 2 == 0) startCells.Add(new Vector2Int(row, col));
                }
            }
        }

        //�ǂ��g���ł��Ȃ��Ȃ�܂Ń��[�v
        while (startCells.Count > 0)
        {
            //�����_���ɊJ�n�Z�����擾���A�J�n��₩��폜
            var index = random.Next(startCells.Count);
            var cell = startCells[index];
            startCells.RemoveAt(index);
            var row = cell.x;
            var col = cell.y;

            //���łɕǂ̏ꍇ�͉������Ȃ�
            if (maze[row, col] == path)
            {
                //�g�����̕ǂ̏���������
                currentWallCells.Clear();
                ExtendWall(row, col);
            }
        }
        return maze;
    }

    //�ǂ��g��
    void ExtendWall(int row,int col)
    {
        //�L�΂������ł������(1�}�X�悪�ʘH��2�}�X��܂Ŕ͈͓�)
        var direction = new List<directions>();
        //if (maze[row, col - 1] == path && !IsCurrentWall(row, col - 2)) direction.Add(directions.up);
        //if (maze[row + 1, col] == path && !IsCurrentWall(row + 2, col)) direction.Add(directions.right);
        //if (maze[row, col + 1] == path && !IsCurrentWall(row, col + 2)) direction.Add(directions.down);
        //if (maze[row - 1, col] == path && !IsCurrentWall(row - 2, col)) direction.Add(directions.left);

        // �͈̓`�F�b�N��ǉ����ċ��E�O�A�N�Z�X��h��
        if (col > 1 && maze[row, col - 1] == path && !IsCurrentWall(row, col - 2)) direction.Add(directions.up);
        if (row < width - 2 && maze[row + 1, col] == path && !IsCurrentWall(row + 2, col)) direction.Add(directions.right);
        if (col < height - 2 && maze[row, col + 1] == path && !IsCurrentWall(row, col + 2)) direction.Add(directions.down);
        if (row > 1 && maze[row - 1, col] == path && !IsCurrentWall(row - 2, col)) direction.Add(directions.left);

        //�����_���ɐL�΂�
        if (direction.Count > 0)
        {
            //�ǂ��쐬
            SetWall(row, col);

            //�L�΂��悪�ʘH�Ȃ�g���𑱂���
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

            //�����̕ǂɐڑ��ł��Ă��Ȃ��ꍇ�͊g�����s
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
