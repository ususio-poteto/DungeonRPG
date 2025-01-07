using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyController : MonoBehaviour
{
    bool isMoving = false;

    MazeManager mazeManager;

    Tilemap tilemap;

    Vector3Int currentGridPosition;

    [SerializeField]
    TileBase path_tile;

    [SerializeField]
    TileBase goal_tile;

    void EnemyTurn()
    {

    }

    void MoveEnemy()
    {
        if (isMoving) return;

        Vector3Int direction = Vector3Int.zero;


        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
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

        if (direction != Vector3Int.zero)
        {
            StartCoroutine(MoveToCell(direction));
        }
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

            if (IsGoalTile(targetGridPosition))
            {
                Debug.Log("ゴール!!");
                mazeManager.RecreateMaze();
            }
        }
        isMoving = false;
    }

    void AttackEnemy()
    {

    }
}
