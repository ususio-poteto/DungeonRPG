using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using System.Security.Cryptography;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private int moveSpeed;

    [SerializeField]
    private Animator playerAnim;

    public Rigidbody2D rb;

    Tilemap tilemap;

    Grid grid;

    MazeManager mazeManager;

    TurnManager turnManager;

    GameManager gameManager;

    bool isMoving = false;

    float moveTime = 0.2f;

    Vector3Int currentGridPosition;

    Vector3Int beforeGridPosition;

    [SerializeField]
    TileBase path_tile;

    [SerializeField]
    TileBase goal_tile;

    [SerializeField]
    TileBase route_tile;

    SpriteRenderer spriteRenderer;

    [SerializeField]
    Sprite upSprite;

    [SerializeField]
    Sprite downSprite;

    [SerializeField]
    Sprite rightSprite;

    [SerializeField]
    Sprite leftSprite;

    Rigidbody2D rb2d;

    enum eDirection
    {
        up,
        down,
        left,
        right
    }

    eDirection direction;

    void Start()
    {
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        mazeManager = GameObject.Find("MazeManager").GetComponent<MazeManager>();
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        Vector3 worldPosition = transform.position;
        currentGridPosition = tilemap.WorldToCell(worldPosition);
        transform.position = tilemap.GetCellCenterWorld(currentGridPosition);
        //Debug.Log("transform.position" + transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(direction);
        if (turnManager.GetPlayerTurn()) 
        {
            if (isMoving) return;
        
            Vector3Int moveDirection = Vector3Int.zero;

            playerAnim.SetFloat("X", 0);
            playerAnim.SetFloat("Y", 0);

            if (Input.GetKeyDown(KeyCode.W)||Input.GetKeyDown(KeyCode.UpArrow))
            {
                moveDirection = Vector3Int.up;
                direction = eDirection.up;
                playerAnim.SetFloat("X", 0);
                playerAnim.SetFloat("Y", 1);
            }
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                moveDirection = Vector3Int.down;
                direction = eDirection.down;
                playerAnim.SetFloat("X", 0);
                playerAnim.SetFloat("Y", -1);
            }
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                moveDirection = Vector3Int.left;
                direction = eDirection.left;
                playerAnim.SetFloat("X", -1);
                playerAnim.SetFloat("Y", 0);
            }
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                moveDirection = Vector3Int.right;
                direction = eDirection.right;
                playerAnim.SetFloat("X", 1);
                playerAnim.SetFloat("Y", 0);
            }

            if (moveDirection != Vector3Int.zero)
            {
                //turnManager.SwitchTurn();
                StartCoroutine(MoveToCell(moveDirection));
            }

            if (direction == eDirection.up)
            {
                spriteRenderer.sprite = upSprite;
            }

            if (direction == eDirection.down)
            {
                spriteRenderer.sprite = downSprite;
            }

            if(direction == eDirection.left)
            {
                spriteRenderer.sprite = leftSprite;
            }

            if(direction==eDirection.right)
            {
                spriteRenderer.sprite = rightSprite;
            }
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F4))
        {
            mazeManager.SearchShortestPath(transform.position);
        }
#endif
    }
    
    System.Collections.IEnumerator MoveToCell(Vector3Int direction)
    {
        isMoving = true;

        // 移動先のグリッド座標を計算
        Vector3Int targetGridPosition = currentGridPosition + direction;

        // 移動可能なタイルであるかを確認（必要に応じて条件を変更）
        if (CanMoveToTile(targetGridPosition))
        {
            Vector3 targetPosition = tilemap.GetCellCenterWorld(targetGridPosition);

            beforeGridPosition = currentGridPosition;

            // 移動アニメーション
            float elapsedTime = 0f;
            Vector3 startPosition = transform.position;

            while (elapsedTime < moveTime)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition;
            currentGridPosition = targetGridPosition; // 現在のグリッド位置を更新

            if (IsGoalTile(targetGridPosition))
            {
                Debug.Log("ゴール!!");
                gameManager.isGoal();
                mazeManager.RecreateMaze();
            }
        }   
        isMoving = false;
        //実際には移動していないがターンが変わるので要修正
        //turnManager.SwitchTurn();
        
    }

    bool CanMoveToTile(Vector3Int gridPosition)
    {
        TileBase tile = tilemap.GetTile(gridPosition);
        return tile == path_tile || tile == goal_tile || tile == route_tile;
    }

    bool IsGoalTile(Vector3Int gridPosition)
    {
        TileBase tile = tilemap.GetTile(gridPosition);
        return tile == goal_tile;
    }
}
