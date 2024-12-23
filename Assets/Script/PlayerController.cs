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

    const int wall = 1;

    int[,] maze;
    
    //迷路の壁の位置を入れておく。座標系はワールド座標
    List<Vector3> worldWallPosition = new List<Vector3>();


    void Start()
    {
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        tilemapController = GameObject.Find("TilemapController").GetComponent<TilemapController>();
        maze = tilemapController.Getmaze();
        SetwallPosition(maze);
    }

    // Update is called once per frame
    void Update()
    {

        //playerの移動（velocityはRigidBodyの速度ベクトル、normalizedは正規化をしています）
        rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * moveSpeed;

        //速度が0でない時、キー入力に合わせてアニメーション用のパラメーターを更新する
        if (rb.velocity != Vector2.zero)
        {
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                MoveRight();
            }
            else if (Input.GetAxisRaw("Horizontal") < 0)
            {
                MoveLeft();
            }
            else if (Input.GetAxisRaw("Vertical") > 0)
            {
                MoveUp();
            }
            else
            {
                MoveDown();
            }
        }
        else
        {
            playerAnim.SetFloat("X", 0);
            playerAnim.SetFloat("Y", 0);
        }
    }

    void MoveUp()
    {
        playerAnim.SetFloat("X", 0);
        playerAnim.SetFloat("Y", 1);
        var tartgetpos = new Vector3(transform.position.x, transform.position.y + 1, 0);
        transform.position = Vector3.MoveTowards(transform.position, tartgetpos, moveSpeed);
    }

    void MoveDown()
    {
        playerAnim.SetFloat("X", 0);
        playerAnim.SetFloat("Y", -1);
    }

    void MoveRight()
    {
        playerAnim.SetFloat("X", 1f);
        playerAnim.SetFloat("Y", 0);
    }

    void MoveLeft()
    {
        playerAnim.SetFloat("X", -1f);
        playerAnim.SetFloat("Y", 0);
    }

    void SetwallPosition(int[,] maze)
    {
        for (int y = 0; y < maze.GetLength(1); y++)
        {
            for (int x = 0; x < maze.GetLength(0); x++)
            {
                if (maze[y, x] == wall)
                {
                    worldWallPosition.Add(tilemap.GetCellCenterLocal(new Vector3Int(x, y, 0)));
                }
            }
        }
    }
}
