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
using UnityEngine.EventSystems;
using UnityEngine.Jobs;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    int moveSpeed;

    [SerializeField]
    Animator playerAnim;

    [SerializeField]
    GameObject attackEffect;

    Rigidbody2D rb;

    Tilemap tilemap;

    MazeManager mazeManager;

    TurnManager turnManager;

    GameManager gameManager;

    bool isMoving = false;

    bool isAttack = false;

    float moveTime = 0.2f;

    float distance = 1.5f;

    Vector3Int currentGridPosition;

    Vector3Int beforeGridPosition;

    [SerializeField]
    TileBase path_tile;

    [SerializeField]
    TileBase goal_tile;

    [SerializeField]
    TileBase route_tile;

    Rigidbody2D rb2d;

    RaycastHit2D hit;

    Vector3 createPosition;

    [SerializeField]
    int attackValue;

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
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        rb2d = GetComponent<Rigidbody2D>();
        Vector3 worldPosition = transform.position;
        currentGridPosition = tilemap.WorldToCell(worldPosition);
        transform.position = tilemap.GetCellCenterWorld(currentGridPosition);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(direction);
        if (turnManager.GetPlayerTurn()) 
        {
            if (isMoving) return;
        
            Vector3Int moveDirection = Vector3Int.zero;

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
            if (Input.GetKeyDown(KeyCode.K))
            {
                switch (direction)
                {
                    case eDirection.up:
                        createPosition = transform.position + new Vector3(0, 1, 0.5f);
                        hit = Physics2D.Raycast(transform.position + new Vector3(0, 0.6f, 0), Vector2.up, distance);
                        break;
                    case eDirection.down:
                        createPosition = transform.position + new Vector3(0, -1, 0.5f);
                        hit = Physics2D.Raycast(transform.position + new Vector3(0, -0.6f, 0), Vector2.down, distance);
                        break;
                    case eDirection.left:
                        createPosition = transform.position + new Vector3(-1, 0, 0.5f);
                        hit = Physics2D.Raycast(transform.position + new Vector3(0.6f, 0, 0), Vector2.left, distance);
                        break;
                    case eDirection.right:
                        createPosition = transform.position + new Vector3(1, 0, 0.5f);
                        hit = Physics2D.Raycast(transform.position + new Vector3(-0.6f, 0, 0), Vector2.right, distance); 
                        break;
                }
                isAttack = true;
                var cteateObject = Instantiate(attackEffect, createPosition, Quaternion.identity);
                Attack(hit);
            }               
        }
        
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F4))
        {
            mazeManager.SearchShortestPath(transform.position);
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            attackValue = 99999; 
        }


        switch (direction)
        {
            case eDirection.up:
                Debug.DrawRay(transform.position + new Vector3(0, 0.5f, 0), Vector2.up * distance, Color.red);
                break;
            case eDirection.down:
                Debug.DrawRay(transform.position + new Vector3(0, -0.5f, 0), Vector2.down * distance, Color.red);
                break;
            case eDirection.left:
                Debug.DrawRay(transform.position + new Vector3(-0.5f, 0, 0), Vector2.left * distance, Color.red);
                break;
            case eDirection.right:
                Debug.DrawRay(transform.position + new Vector3(0.5f, 0, 0), Vector2.right * distance, Color.red);
                break;
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

            beforeGridPosition = currentGridPosition;

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
                gameManager.isGoal();
                mazeManager.RecreateMaze();
            }
        }   
        isMoving = false;
        //���ۂɂ͈ړ����Ă��Ȃ����^�[�����ς��̂ŗv�C��
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

    void Attack(RaycastHit2D target)
    {
        if (target.collider.tag == "Enemy")
        {
            Debug.Log(target.collider.name);
            var damageble = target.collider.GetComponent<IDamagable>();
            damageble.TakeDamage(attackValue);
        }
    }
}
