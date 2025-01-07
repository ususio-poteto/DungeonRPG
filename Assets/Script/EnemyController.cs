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

    Vector3Int currentGridPosition;

    float moveTime = 0.2f;

    Vector3Int[] directions = new Vector3Int[] { Vector3Int.up, Vector3Int.down, Vector3Int.right, Vector3Int.left };

    void Start()
    {
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        Vector3 worldPosition = transform.position;
        currentGridPosition = tilemap.WorldToCell(worldPosition);
    }
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F2))
        {
            MoveEnemy();
        }
#endif
    }

    void MoveEnemy()
    {
        var rnd = Random.Range(0, directions.Length);
        StartCoroutine(MoveToCell(directions[rnd]));
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
        }
        isMoving = false;
    }

    bool CanMoveToTile(Vector3Int gridPosition)
    {
        TileBase tile = tilemap.GetTile(gridPosition);
        return tile == path_tile || tile == goal_tile;
    }
}
