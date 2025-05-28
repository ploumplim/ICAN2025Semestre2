using System;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerScript playerScript;
    [SerializeField] private float animationSpeedMultiplier = 0.1f;

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
                animator.SetBool("IsGrabbing",true);
                break;
            case ReleaseState:
                animator.SetTrigger("IsHitting");
                animator.SetBool("IsGrabbing",false);
                break;
            case NeutralState:
                animator.SetTrigger("EnterNeutral"); 
                animator.SetBool("IsGrabbing",false);
                break;
            case KnockbackState:
                animator.SetBool("IsGrabbing",false);
                break;
            case SprintState:
                animator.SetBool("IsGrabbing",false);
                break;
        }
    }
    
    private void AnimationUpdateManager()
    {
        // animator.SetBool("IsMoving", playerScript.isMoving);
        if (playerScript.currentState is NeutralState)
        {
            animator.SetFloat("RunningFloat", playerScript.rb.linearVelocity.magnitude);
            animator.SetFloat("AnimationSpeedFloat", playerScript.rb.linearVelocity.magnitude * animationSpeedMultiplier);
        }
    }
}
