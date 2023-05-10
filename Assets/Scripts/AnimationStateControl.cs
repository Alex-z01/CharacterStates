using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateControl : MonoBehaviour
{
    Animator animator;
    int isWalkingHash;

    private void Start()
    {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
    }

    private void Update()
    {
        bool isWalking = animator.GetBool("isWalking");
        bool isCrouched = animator.GetBool("isCrouched");
        bool forwardPressed = Input.GetKey(KeyCode.W);
        bool ctrlPressed = Input.GetKey(KeyCode.LeftControl);

        animator.SetBool("isWalking", forwardPressed);

        animator.SetBool("isCrouched", ctrlPressed);
    }
}
