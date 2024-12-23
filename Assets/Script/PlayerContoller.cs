using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContoller : MonoBehaviour
{
    [SerializeField]
    private int moveSpeed;

    [SerializeField]
    private Animator playerAnim;

    public Rigidbody2D rb;

    // Update is called once per frame
    void Update()
    {

        //playerの移動（velocityはRigidBodyの速度ベクトル、normalizedは正規化をしています：詳しく知りたい方は動画の方を確認してね）
        rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * moveSpeed;

        //速度が0でない時、キー入力に合わせてアニメーション用のパラメーターを更新する
        if (rb.velocity != Vector2.zero)
        {
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                playerAnim.SetFloat("X", 1f);
                playerAnim.SetFloat("Y", 0);
            }
            else　if(Input.GetAxisRaw("Horizontal") < 0)
            {
                playerAnim.SetFloat("X", -1f);
                playerAnim.SetFloat("Y", 0);
            }
            else if (Input.GetAxisRaw("Vertical") > 0)
            {
                playerAnim.SetFloat("X", 0);
                playerAnim.SetFloat("Y", 1);
            }
            else
            {
                playerAnim.SetFloat("X", 0);
                playerAnim.SetFloat("Y", -1);
            }
        }
        else
        {
            playerAnim.SetFloat("X", 0);
            playerAnim.SetFloat("Y", 0);
        }
    }
}
