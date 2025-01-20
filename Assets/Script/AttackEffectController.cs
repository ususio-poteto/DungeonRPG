using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AttackEffectController : MonoBehaviour
{
    float animationLength;

    float currentTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        Animator animator = GetComponent<Animator>();
        AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        animationLength = animatorStateInfo.length;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if(currentTime >= animationLength)
        {
            Destroy(gameObject); 
        }
    }
}
