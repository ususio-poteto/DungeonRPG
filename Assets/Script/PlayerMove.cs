using System.Collections;
using System.Collections.Generic;
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

    SpriteRenderer spriteRenderer;

    //[SerializeField]
    //Sprite upSprite;

    //[SerializeField]
    //Sprite downSprite;

    //[SerializeField]
    //Sprite rightSprite;

    //[SerializeField]
    //Sprite leftSprite;

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
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        //Vector3 worldPosition = transform.position;
        //currentGridPosition = tilemap.WorldToCell(worldPosition);
        //transform.position = tilemap.GetCellCenterWorld(currentGridPosition);
        var startPosition = mazeManager.GetPlayerStartPosition();
        currentGridPosition = new Vector3Int(startPosition.x, startPosition.y, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (turnManager.GetPlayerTurn())
        {
            if (isMoving) return;

            Vector3Int moveDirection = Vector3Int.zero;

            playerAnim.SetFloat("X", 0);
            playerAnim.SetFloat("Y", 0);

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
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
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F4))
        {
            mazeManager.SearchShortestPath(transform.position);
        }
#endif
    }

    /*
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
        turnManager.SwitchTurn();

    }
    */
    System.Collections.IEnumerator MoveToCell(Vector3Int direction)
    {
        isMoving = true;
        Vector3Int targetPosition = currentGridPosition + direction;

        float elapsedTime = 0f;
        if (CanMoveToTile(targetPosition))
        {

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
