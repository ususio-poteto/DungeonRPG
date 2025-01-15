using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    int moveSpeed;

    [SerializeField]
    Animator playerAnim;

    [SerializeField]
    Rigidbody2D rb2d;

    Tilemap tilemap;

    Grid grid;

    MazeManager mazeManager;

    TurnManager turnManager;

    GameManager gameManager;

    bool isMoving = false;

    float moveTime = 0.2f;

    //åªç›ínÇ™ì¸ÇÈ
    Vector2Int currentGridPosition;

    Vector3Int beforeGridPosition;

    [SerializeField]
    TileBase path_tile;

    [SerializeField]
    TileBase goal_tile;

    SpriteRenderer spriteRenderer;

    int[,] maze;

    const int path = 0;

    void Start()
    {
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        mazeManager = GameObject.Find("MazeManager").GetComponent<MazeManager>();
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        maze = mazeManager.GetMaze();
        currentGridPosition = mazeManager.GetPlayerStartPosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (turnManager.GetPlayerTurn())
        {
            if (isMoving) return;

            Vector2Int moveDirection = Vector2Int.zero;

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                moveDirection = Vector2Int.up;
            }
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                moveDirection = Vector2Int.down;
            }
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                moveDirection = Vector2Int.left;
            }
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                moveDirection = Vector2Int.right;
            }

            if (moveDirection != Vector2Int.zero)
            {
                //turnManager.SwitchTurn();
                //StartCoroutine(MoveToCell(moveDirection));
                TryMove(moveDirection);
            }
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F4))
        {
            mazeManager.SearchShortestPath(transform.position);
        }
#endif
    }

    void TryMove(Vector2Int direction)
    {
        Vector2Int targetPosition = currentGridPosition + direction;

        if (IsInBounds(targetPosition) && maze[targetPosition.x,targetPosition.y]==path)
        {
            Vector3 worldPosition = tilemap.GetCellCenterLocal(new Vector3Int(targetPosition.x, targetPosition.y, 0));
            transform.position = worldPosition;
            currentGridPosition = targetPosition;
        }
    }

    bool IsInBounds(Vector2Int targetPosition)
    {
        return targetPosition.x >= 0 && targetPosition.x <= maze.GetLength(1) &&
            targetPosition.y > 0 && targetPosition.y <= maze.GetLength(0);
    }

    bool CanMoveToCell(Vector2Int targetPosition)
    {
        TileBase tile = tilemap.GetTile(new Vector3Int(targetPosition.x, targetPosition.y, 0));
        return tile == path_tile || tile == goal_tile;
    }
}
