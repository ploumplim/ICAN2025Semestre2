using System.Collections;
using System.Collections.Generic;
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
        ballToHit = null;
        PlayerScript.OnHitButtonPressed?.Invoke();
        base.Enter();
    }
    
    //---------------------------------------------------------------------------------
    
    public void Hit()
    {
        // Debug.Log("Parry!");
        parrySpherePosition = PlayerScript.transform.position + transform.forward * PlayerScript.hitDetectionOffset;
        
        if (!ballToHit)
        {
            HitBox();
        }

        if (ballToHit)
        {
            StartCoroutine(HitTime());
        }

    }
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
                break;
            }
        }
    }
    IEnumerator HitTime()
    {
            ballToHit.GetComponent<BallSM>().ballOwnerPlayer = gameObject;
            PlayerScript.OnBallHitByPlayer?.Invoke();
            float currentBallSpeed = ballToHit.GetComponent<BallSM>().currentBallSpeedVec3.magnitude;
            ballToHit.GetComponent<BallSM>().ChangeState(ballToHit.GetComponent<HitState>());
            float hitForce = currentBallSpeed + PlayerScript.hitForce;
            
            yield return new WaitForSeconds(hitForce * ballToHit.GetComponent<BallSM>().hitFreezeTimeMultiplier);
            
            PlayerScript.rb.AddForce(-transform.forward * (PlayerScript.knockbackForceMultiplier * 3f), ForceMode.Impulse);
            PlayerScript.ChangeState(GetComponent<NeutralState>());

    }
    
    //---------------------------------------------------------------------------------
    public override void Tick()
    {
        base.Tick();
        if (_windowTimer < PlayerScript.hitWindow && !ballToHit)
        {
            _windowTimer += Time.deltaTime;
            Hit();
        }
        
        if (_windowTimer >= PlayerScript.hitWindow)
        {
            PlayerScript.OnPlayerHitReleased?.Invoke();
            PlayerScript.ChangeState(GetComponent<NeutralState>());
        }
        
        if (ballToHit)
        {
            BallDirection();
            PlayerScript.Move(0f, PlayerScript.hitLerpTime);

        }
        else
        {
            PlayerScript.Move(0f, PlayerScript.neutralLerpTime);

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
    
    
   
    //---------------------------------------------------------------------------------
    public override void Exit()
    {
        _windowTimer = 0;
        base.Exit();
        ballToHit = null;
    }
}
