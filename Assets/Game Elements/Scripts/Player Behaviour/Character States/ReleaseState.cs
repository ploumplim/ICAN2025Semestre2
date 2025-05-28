using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class ReleaseState : PlayerState
{
    [FormerlySerializedAs("ballToParry")] [HideInInspector]public GameObject ballToHit;
    [HideInInspector]public Vector3 parrySpherePosition;
    private float _windowTimer;

    //---------------------------------------------------------------------------------
    public override void Enter()
    {
        base.Enter();
        ballToHit = null;
        PlayerScript.OnHitButtonPressed?.Invoke();
        parrySpherePosition = PlayerScript.transform.position + transform.forward * PlayerScript.hitDetectionOffset;
    }
    //---------------------------------------------------------------------------------
    public override void Tick()
    {
        base.Tick();
        
        if (!ballToHit)
        {
            HitBox();
        }

        if (ballToHit)
        {
            PlayerScript.OnBallHitByPlayer?.Invoke();
            ballToHit.GetComponent<BallSM>().ballOwnerPlayer = PlayerScript.gameObject;
            BallDirection();
            ballToHit.GetComponent<BallSM>().ChangeState(ballToHit.GetComponent<HitState>());
            PlayerScript.rb.AddForce(-transform.forward * (PlayerScript.knockbackForceMultiplier * 3f), ForceMode.Impulse);
            PlayerScript.ChangeState(GetComponent<NeutralState>());
            return;
        }
        
        _windowTimer += Time.deltaTime;
        
        if (_windowTimer >= PlayerScript.hitWindow)
        {
            PlayerScript.ChangeState(GetComponent<NeutralState>());
        }
        

        PlayerScript.Move(PlayerScript.speed, PlayerScript.neutralLerpTime);
        
        
    }
    //---------------------------------------------------------------------------------
    
    public void HitBox()
    {
        // create an overlap sphere that detects the ball. If it did, set the ball to parry to the ball that was detected.
        Collider[] hitColliders = Physics.OverlapSphere(parrySpherePosition, PlayerScript.hitDetectionRadius);
        
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag("Ball"))
            {
                ballToHit = hitCollider.gameObject;
                ballToHit.GetComponent<BallSM>().ballOwnerPlayer = gameObject;
                ballToHit.GetComponent<BallVisuals>().UpdateFlyingColor(GetComponent<PlayerVisuals>().playerCapMaterial.color);
                break;
            }
        }
    }
    
    public void BallDirection()
    {
        Vector3 direction = Vector3.zero;
        GameObject player = PlayerScript.gameObject; 
        switch (PlayerScript.hitType)
        {
            case PlayerScript.HitType.ForwardHit:
                // Send the ball in the direction the player is facing.
                direction = new Vector3(player.transform.forward.x, 0f, player.transform.forward.z).normalized;
                break;
            case PlayerScript.HitType.ReflectiveHit:
                direction = ballToHit.transform.position - transform.position;
                direction = new Vector3(direction.x, 0f, direction.z).normalized;
                break;
        }
        
        ballToHit.GetComponent<HitState>().hitDirection = direction;
        
    }
    
    // private void HitTime()
    // {
    //     Debug.Log("Hit!");
    //     ballToHit.GetComponent<BallSM>().ballOwnerPlayer = gameObject;
    //     PlayerScript.OnBallHitByPlayer?.Invoke();
    //     BallDirection();
    //     ballToHit.GetComponent<BallSM>().ChangeState(ballToHit.GetComponent<HitState>());
    //     PlayerScript.rb.AddForce(-transform.forward * (PlayerScript.knockbackForceMultiplier * 3f), ForceMode.Impulse);
    //     PlayerScript.ChangeState(GetComponent<NeutralState>());
    // }
   
    //---------------------------------------------------------------------------------
    public override void Exit()
    {
        PlayerScript.OnPlayerHitReleased?.Invoke();
        _windowTimer = 0;
        base.Exit();
        ballToHit = null;
    }
}
