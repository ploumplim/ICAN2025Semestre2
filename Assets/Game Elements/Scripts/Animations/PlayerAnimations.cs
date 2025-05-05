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
        AnimationBooleanManager();
    }

    private void AnimationTriggerManager(PlayerState state)
    {
        // animator.ResetTrigger("IsCharging");
        // animator.ResetTrigger("IsHitting");
        switch (state)
        {
            case ChargingState:
                animator.SetTrigger("IsCharging");
                break;
            case ReleaseState:
                animator.SetTrigger("IsHitting"); 
                break;
        }
    }
    
    private void AnimationBooleanManager()
    {
        animator.SetBool("IsMoving", playerScript.isMoving);
    }
}
