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

    [SerializeField]
    PlayerCharacter playerCharacter;

    bool isMoving = false;

    bool isAttack = false;

    float moveTime = 0.2f;

    float distance = 1.0f;

    Vector3Int currentGridPosition;

    Vector3Int beforeGridPosition;

    [SerializeField]
    TileBase path_tile;

    [SerializeField]
    TileBase goal_tile;

    [SerializeField]
    TileBase route_tile;

    [SerializeField]
    TileBase wall_path;

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

     eDirection direction = eDirection.down;

    enum eMode
    {
        debug,
        play
    }

    eMode mode = eMode.play;

    enum eAction
    {
        rotate,
        move
    }

    eAction action = eAction.move;

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
        transform.rotation= Quaternion.identity;
        //Debug.Log(direction);
        if (turnManager.GetPlayerTurn()) 
        {
            if(action==eAction.move)
            {
                if (isMoving) return;
        
                Vector3Int moveDirection = Vector3Int.zero;

                //�ړ�
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
                    StartCoroutine(MoveToCell(moveDirection));
                    playerCharacter.Healing(5);
                    turnManager.SwitchTurn();
                }
            }

            if (Input.GetKey(KeyCode.T))
            {
                action = eAction.rotate;
            }

            if(Input.GetKeyUp(KeyCode.T))
            {
                action = eAction.move;
            }

            //��]
            if (action == eAction.rotate)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    direction = eDirection.up;
                    playerAnim.SetFloat("X", 0);
                    playerAnim.SetFloat("Y", 1);
                }

                if (Input.GetKeyDown(KeyCode.S))
                {
                    direction = eDirection.down;
                    playerAnim.SetFloat("X", 0);
                    playerAnim.SetFloat("Y", -1);
                }

                if (Input.GetKeyDown(KeyCode.A))
                {
                    direction = eDirection.left;
                    playerAnim.SetFloat("X", -1);
                    playerAnim.SetFloat("Y", 0);
                }

                if(Input.GetKeyDown(KeyCode.D))
                {
                    direction = eDirection.right;
                    playerAnim.SetFloat("X", 1);
                    playerAnim.SetFloat("Y", 0);
                }
            }

            //�U��
            if (Input.GetKeyDown(KeyCode.K))
            {
                switch (direction)
                {
                    case eDirection.up:
                        createPosition = transform.position + new Vector3(0, 1, 0.5f);
                        hit = Physics2D.Raycast(transform.position + new Vector3(0, 1f, 0), Vector2.up, distance);
                        Debug.DrawRay(transform.position + new Vector3(0, 0.5f, 0), Vector2.up * distance, Color.red, 1f);
                        break;
                    case eDirection.down:
                        createPosition = transform.position + new Vector3(0, -1, 0.5f);
                        hit = Physics2D.Raycast(transform.position + new Vector3(0, -1f, 0), Vector2.down, distance);
                        Debug.DrawRay(transform.position + new Vector3(0, -0.5f, 0), Vector2.down * distance, Color.red, 1f);
                        break;
                    case eDirection.left:
                        createPosition = transform.position + new Vector3(-1, 0, 0.5f);
                        hit = Physics2D.Raycast(transform.position + new Vector3(-1f, 0, 0), Vector2.left, distance);
                        Debug.DrawRay(transform.position + new Vector3(-0.5f, 0, 0), Vector2.left * distance, Color.red, 1f);
                        break;
                    case eDirection.right:
                        createPosition = transform.position + new Vector3(1, 0, 0.5f);
                        hit = Physics2D.Raycast(transform.position + new Vector3(1f, 0, 0), Vector2.right, distance);
                        Debug.DrawRay(transform.position + new Vector3(0.5f , 0, 0), Vector2.right * distance, Color.red, 1f);
                        break;
                }
                //Debug.Log($"hit:{hit.collider.name}");
                isAttack = true;
                var cteateObject = Instantiate(attackEffect, createPosition, Quaternion.identity);
                if (hit.collider != null && hit.collider.tag == "Enemy") Attack(hit);
                turnManager.SwitchTurn();   
            }          
        }

#if UNITY_EDITOR
        //�ŒZ�o�H�T��
        if (Input.GetKeyDown(KeyCode.F4))
        {
            mazeManager.SearchShortestPath();
        }

        //�U���͂𑝂₷
        if (Input.GetKeyDown(KeyCode.F6))
        {
            attackValue = 99999; 
        }

        if (Input.GetKeyDown(KeyCode.F7))
        {
            mode = eMode.debug;
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
            }
        }
        isMoving = false;
    }

    bool CanMoveToTile(Vector3Int gridPosition)
    {
        if(mode==eMode.play)
        {
            TileBase tile = tilemap.GetTile(gridPosition);
            return tile == path_tile || tile == goal_tile || tile == route_tile;
        }

        else
        {
            TileBase tile = tilemap.GetTile(gridPosition);
            return tile == path_tile || tile == goal_tile || tile == route_tile || tile == wall_path;
        }
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

    public void AddAttackValue()
    {
        attackValue += 2;
    }
}
