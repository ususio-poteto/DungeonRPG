using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
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

    Vector3Int[] directions = new Vector3Int[] { Vector3Int.up, Vector3Int.down, Vector3Int.right, Vector3Int.left };

    enum state
    {
        patrol,
        tracking
    }

    state eState = state.patrol;

    Rigidbody2D rb2d;

    void Start()
    {
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        turnManager=GameObject.Find("TurnManager").GetComponent<TurnManager>();
        rb2d = GetComponent<Rigidbody2D>();
        Vector3 worldPosition = transform.position;
        currentGridPosition = tilemap.WorldToCell(worldPosition);
    }

//    void Update()
//    {
//#if UNITY_EDITOR
//        if (Input.GetKeyDown(KeyCode.F2))
//        {
//            MoveEnemy();
//        }
//#endif
//    }

    public void MyTurn()
    {
        MoveEnemy();
    }

    void MoveEnemy()
    {
        if (eState == state.patrol)
        {   //�Ȃ������ׂĂ̓G���������Ȃ��v�C��
            var rnd = Random.Range(0, directions.Length);
            StartCoroutine(MoveToCell(directions[rnd]));
        }

        
        else if (eState == state.tracking)
        {
            //A*�A���S���Y���ōŒZ�o�H�T�����s��
            //��������player�̂��Ƃ�ǂ�������悤�ɂ��邩
            //tilemap�Ƀt���O�𗧂ĂĂ������񂶁B
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
        turnManager.SwitchTurn();
    }

    bool CanMoveToTile(Vector3Int gridPosition)
    {
        TileBase tile = tilemap.GetTile(gridPosition);
        return tile == path_tile || tile == goal_tile;
    }
}
