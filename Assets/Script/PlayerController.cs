using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private int moveSpeed;

    [SerializeField]
    private Animator playerAnim;

    public Rigidbody2D rb;

    Tilemap tilemap;

    MazeManager mazeManager;

    bool isMoving = false;

    float moveTime = 0.2f;

    Vector3Int currentGridPosition;

    [SerializeField]
    TileBase path_tile;

    [SerializeField]
    TileBase goal_tile;

    SpriteRenderer spriteRenderer;

    [SerializeField]
    Sprite upSprite;

    [SerializeField]
    Sprite downSprite;

    [SerializeField]
    Sprite rightSprite;

    [SerializeField]
    Sprite leftSprite;

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
        mazeManager = GameObject.Find("MazeManager").GetComponent<MazeManager>();
        Vector3 worldPosition = transform.position;
        currentGridPosition = tilemap.WorldToCell(worldPosition);
        transform.position = tilemap.GetCellCenterWorld(currentGridPosition);
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
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

        if(moveDirection!=Vector3Int.zero)
        {
            StartCoroutine(MoveToCell(moveDirection));
        }

        //デバッグ用
        //クリックした場所までワープする
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            var onClickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var tilemapPosition = tilemap.WorldToCell(onClickPosition);
            var cellPosition = tilemap.GetCellCenterWorld(tilemapPosition);
            transform.position = new Vector3(cellPosition.x, cellPosition.y, 0);
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
                mazeManager.RecreateMaze();
            }
        }   
        isMoving = false;
    }

    bool CanMoveToTile(Vector3Int gridPosition)
    {
        TileBase tile = tilemap.GetTile(gridPosition);
        return tile == path_tile || tile == goal_tile;
    }

    bool IsGoalTile(Vector3Int gridPosition)
    {
        TileBase tile = tilemap.GetTile(gridPosition);
        return tile == goal_tile;
    }
}
