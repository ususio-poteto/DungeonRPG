using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TreeEditor;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class EnemyController : MonoBehaviour
{

    bool isMoving = false;

    Tilemap tilemap;

    [SerializeField]
    TileBase path_tile;

    [SerializeField]
    TileBase goal_tile;

    float moveTime = 0.2f;

    Vector3Int currentGridPosition;

    TurnManager turnManager;

    AStarAlgorithm aStarAlgorithm;

    MazeManager mazeManager;

    GameManager gameManager;

    Vector3Int[] directions = new Vector3Int[] { Vector3Int.up, Vector3Int.down, Vector3Int.right, Vector3Int.left };

    enum state
    {
        patrol,
        tracking,
        attack
    }

    [SerializeField]
    state eState = state.patrol;

    Rigidbody2D rb2d;

    RaycastHit2D trackingHit;

    float trackingDistance = 6;

    RaycastHit2D attackHit;

    float attackDistance = 1;

    Vector3 playerPos;

    void Start()
    {
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        aStarAlgorithm = GameObject.Find("A*Algorithm").GetComponent<AStarAlgorithm>();
        mazeManager = GameObject.Find("MazeManager").GetComponent<MazeManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        rb2d = GetComponent<Rigidbody2D>();
        Vector3 worldPosition = transform.position;
        currentGridPosition = tilemap.WorldToCell(worldPosition);
    }

    void Update()
    {
        Debug.Log(eState);
    }

    public void MyTurn()
    {
        foreach (Vector3 direction in directions)
        {
            trackingHit = Physics2D.Raycast(transform.position + direction, direction, trackingDistance);
            Debug.DrawRay(transform.position + direction, direction * trackingDistance, Color.red, 1f);

            if (trackingHit.collider != null) break;

        }

        if (trackingHit.collider == null) return;

        else if (trackingHit.collider.tag == "Player")
        {
            playerPos = trackingHit.collider.transform.position;
            eState = state.tracking;
        }

        else eState = state.patrol;

        ActionEnemy();
    }

    void ActionEnemy()
    {
        if (eState == state.patrol)
        {
            var rnd = Random.Range(0, directions.Length);
            StartCoroutine(MoveToCell(directions[rnd]));
        }


        else if (eState == state.tracking)
        {
            //A*�A���S���Y���ōŒZ�o�H�T�����s��
            //��������player�̂��Ƃ�ǂ�������悤�ɂ��邩
            //tilemap�Ƀt���O�𗧂ĂĂ������񂶁B
            TrackingMove();
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
        }
        isMoving = false;
        //turnManager.SwitchTurn();
    }

    void TrackingMove()
    {
        var maze = mazeManager.GetMaze();
        Debug.Log($"PlayerPos{ playerPos}");
        Vector2Int goalPos = new Vector2Int(Mathf.FloorToInt(playerPos.x), Mathf.FloorToInt(playerPos.y));
        Vector2Int startPos = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        var path = aStarAlgorithm.FindPath(maze, startPos, goalPos);
        //�����ňړ�����̂����˂΂Ȃ��
        //����ǂ�
    }
    
    bool CanMoveToTile(Vector3Int gridPosition)
    {
            TileBase tile = tilemap.GetTile(gridPosition);
            return tile == path_tile || tile == goal_tile;
    }
}
