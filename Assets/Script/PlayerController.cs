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
    TileBase pass_tile;
    
    //���H�̕ǂ̈ʒu�����Ă����B���W�n�̓��[���h���W
    List<Vector3> worldWallPosition = new List<Vector3>();


    void Start()
    {
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();

        Vector3 worldPosition = transform.position;
        currentGridPosition = tilemap.WorldToCell(worldPosition);
        transform.position = tilemap.GetCellCenterWorld(currentGridPosition);
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving) return;
        
        Vector3Int direction = Vector3Int.zero;

        if (Input.GetKeyDown(KeyCode.W)||Input.GetKeyDown(KeyCode.UpArrow))
        {
            direction = Vector3Int.up;
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            direction = Vector3Int.down;
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direction = Vector3Int.left;
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = Vector3Int.right;
        }

        if(direction!=Vector3Int.zero)
        {
            StartCoroutine(MoveToCell(direction));
        }
    }
    
    System.Collections.IEnumerator MoveToCell(Vector3Int direction)
    {
        isMoving = true;

        Vector3Int targetGridPositioin = currentGridPosition + direction;

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
        }

        isMoving = false;
    }

    bool CanMoveToTile(Vector3Int gridPosition)
    {
        TileBase tile =tilemap.GetTile(gridPosition);
        return tile == pass_tile;
    }
}
