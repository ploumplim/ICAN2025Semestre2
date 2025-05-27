using System;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerScript playerScript;

    private void OnEnable()
    {
        playerScript.OnPlayerStateChanged += AnimationTriggerManager;
    }
    private void OnDisable()
    {
        playerScript.OnPlayerStateChanged -= AnimationTriggerManager;
    }

    private void Update()
    {
        AnimationUpdateManager();
    }

    private void AnimationTriggerManager(PlayerState state)
    {
        // animator.ResetTrigger("IsCharging");
        // animator.ResetTrigger("IsHitting");
        switch (state)
        {
            case GrabbingState:
                animator.SetTrigger("IsCharging");
                break;
            case ReleaseState:
                animator.SetTrigger("IsHitting"); 
                break;
            case NeutralState:
                // animator.ResetTrigger("IsCharging");
                // animator.ResetTrigger("IsHitting");
                break;
        }
    }
    
    private void AnimationUpdateManager()
    {
        // animator.SetBool("IsMoving", playerScript.isMoving);
        if (playerScript.currentState is NeutralState)
        {
            animator.SetFloat("RunningFloat", playerScript.rb.linearVelocity.magnitude);
        }
    }
}
