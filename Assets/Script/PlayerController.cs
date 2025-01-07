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

        //�f�o�b�O�p
        //�N���b�N�����ꏊ�܂Ń��[�v����
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

        // �ړ���̃O���b�h���W���v�Z
        Vector3Int targetGridPosition = currentGridPosition + direction;

        // �ړ��\�ȃ^�C���ł��邩���m�F�i�K�v�ɉ����ď�����ύX�j
        if (CanMoveToTile(targetGridPosition))
        {
            Vector3 targetPosition = tilemap.GetCellCenterWorld(targetGridPosition);

            // �ړ��A�j���[�V����
            float elapsedTime = 0f;
            Vector3 startPosition = transform.position;

            while (elapsedTime < moveTime)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition;
            currentGridPosition = targetGridPosition; // ���݂̃O���b�h�ʒu���X�V

            if (IsGoalTile(targetGridPosition))
            {
                Debug.Log("�S�[��!!");
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
