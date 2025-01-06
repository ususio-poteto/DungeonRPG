using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private int moveSpeed;

    [SerializeField]
    private Animator playerAnim;

    public Rigidbody2D rb;

    Tilemap tilemap;

    TilemapController tilemapController;

    bool isMoving = false;

    public float moveTime = 0.2f;

    Vector3Int currentGridPosition;

    [SerializeField]
    TileBase path_tile;

    [SerializeField]
    TileBase goal_tile;
    
    //���H�̕ǂ̈ʒu�����Ă����B���W�n�̓��[���h���W
    List<Vector3> worldWallPosition = new List<Vector3>();


    void Start()
    {
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        tilemapController = GameObject.Find("TilemapController").GetComponent<TilemapController>();
        Vector3 worldPosition = transform.position;
        currentGridPosition = tilemap.WorldToCell(worldPosition);
        transform.position = tilemap.GetCellCenterWorld(currentGridPosition);
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving) return;
        
        Vector3Int direction = Vector3Int.zero;

        playerAnim.SetFloat("X", 0);
        playerAnim.SetFloat("Y", 0);

        if (Input.GetKeyDown(KeyCode.W)||Input.GetKeyDown(KeyCode.UpArrow))
        {
            direction = Vector3Int.up;
            playerAnim.SetFloat("X", 0);
            playerAnim.SetFloat("Y", 1);
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            direction = Vector3Int.down;
            playerAnim.SetFloat("X", 0);
            playerAnim.SetFloat("Y", -1);
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direction = Vector3Int.left;
            playerAnim.SetFloat("X", -1);
            playerAnim.SetFloat("Y", 0);
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = Vector3Int.right;
            playerAnim.SetFloat("X", 1);
            playerAnim.SetFloat("Y", 0);
        }

        if(direction!=Vector3Int.zero)
        {
            StartCoroutine(MoveToCell(direction));
        }
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
                tilemapController.RecreateMaze();
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
