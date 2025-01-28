using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using TreeEditor;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class EnemyController : MonoBehaviour
{
    TurnManager turnManager;

    AStarAlgorithm aStarAlgorithm;

    MazeManager mazeManager;

    GameManager gameManager;

    Vector3Int[] directions = new Vector3Int[] { Vector3Int.up, Vector3Int.down, Vector3Int.right, Vector3Int.left };

    bool isMoving = false;

    Vector3Int currentGridPosition;

    Tilemap tilemap;

    [SerializeField]
    TileBase path_tile;

    [SerializeField]
    TileBase goal_tile;

    [SerializeField]
    GameObject AttackEffect;

    float moveTime = 0.2f;

    enum state
    {
        patrol,
        tracking,
        attack
    }

    [SerializeField]
    state eState = state.patrol;

    [SerializeField]
    float attackDistance;

    [SerializeField]
    float canMoveDistance;

    [SerializeField]
    int attackValue;

    Vector3 playerPos;

    void Start()
    {
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        aStarAlgorithm = GameObject.Find("A*Algorithm").GetComponent<AStarAlgorithm>();
        mazeManager = GameObject.Find("MazeManager").GetComponent<MazeManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        Vector3 worldPosition = transform.position;
        currentGridPosition = tilemap.WorldToCell(worldPosition);
    }

    /// <summary>
    /// �^�[��������Ă������̏���
    /// </summary>
    public void MyTurn()
    {
        var player = GameObject.FindWithTag("Player");        
        if (player == null) return;

        var playerRoute = SearchPlayer(player.transform.position);
        Debug.Log($"routeCount{playerRoute.Count}");
        if (playerRoute.Count < 3) eState = state.attack;
        else if (playerRoute.Count <= 300) eState = state.tracking;
        else eState = state.patrol;

        if (eState == state.patrol) RandomMove();
        else if (eState == state.tracking) TrackingMove(playerRoute[1]);
        else if (eState == state.attack) AttackPlayer();

    }

    /// <summary>
    /// �ŏ��ɍU���ł��邩���m�F
    /// �U�����ł���Ȃ�U��������
    /// </summary>
    void AttackPlayer()
    {
        foreach(Vector3 direction in directions)
        {
            var hit = Physics2D.Raycast(transform.position + direction, direction, attackDistance);
            Debug.DrawRay(transform.position + direction, direction * attackDistance, Color.blue, 0.5f);

            if (hit.collider != null && hit.collider.tag == "Player")
            {
                //Debug.Log("hit");
                var IDamagable=hit.collider.GetComponent<IDamagable>();
                IDamagable.TakeDamage(attackValue);
                Instantiate(AttackEffect, transform.position + direction, Quaternion.identity);
                break;
            }
        }
    }

    /// <summary>
    /// player�܂ł̍ŒZ�o�H������
    /// </summary>
    List<Vector2Int> SearchPlayer(Vector3 setPosition)
    {
        Vector2Int startPos = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        Vector3 playerPos = setPosition;
        Vector2Int goalPos = new Vector2Int(Mathf.FloorToInt(playerPos.x), Mathf.FloorToInt(playerPos.y));
        int[,] maze = mazeManager.GetMaze();
        var playerRoute = aStarAlgorithm.FindPath(maze, startPos, goalPos);
        return playerRoute;
    }

    /// <summary>
    /// �ǐՈړ�
    /// </summary>
    void TrackingMove(Vector2Int targetPosition)
    {
        Vector3 targetPos = tilemap.GetCellCenterLocal(new Vector3Int(targetPosition.x,targetPosition.y,0));
        //Debug.Log($"targetPos:{targetPos}");
        transform.position = Vector3.Lerp(transform.position, targetPos, 1f);
        currentGridPosition = tilemap.WorldToCell(targetPos);
    }

    /// <summary>
    /// �G�̃����_���E�H�[�N
    /// </summary>
    void RandomMove()
    {
        var rnd = Random.Range(0, directions.Length);
        StartCoroutine(MoveToCell(directions[rnd]));
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

    bool CanMoveToTile(Vector3Int gridPosition)
    {
        TileBase tile = tilemap.GetTile(gridPosition);
        return tile == path_tile || tile == goal_tile;
    }
}