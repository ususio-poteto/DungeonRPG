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

        //player�̈ړ��ivelocity��RigidBody�̑��x�x�N�g���Anormalized�͐��K�������Ă��܂��F�ڂ����m�肽�����͓���̕����m�F���Ăˁj
        rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * moveSpeed;

        //���x��0�łȂ����A�L�[���͂ɍ��킹�ăA�j���[�V�����p�̃p�����[�^�[���X�V����
        if (rb.velocity != Vector2.zero)
        {
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                playerAnim.SetFloat("X", 1f);
                playerAnim.SetFloat("Y", 0);
            }
            else�@if(Input.GetAxisRaw("Horizontal") < 0)
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
