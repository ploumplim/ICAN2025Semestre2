using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ReleaseState : PlayerState
{
    [FormerlySerializedAs("ballToParry")] [HideInInspector]public GameObject ballToHit;
    [HideInInspector]public Vector3 parrySpherePosition;
    [HideInInspector]public float currentBallSpeed;
    [HideInInspector] public bool ballHit;
    private Vector3 _eightDirDirection;
    //---------------------------------------------------------------------------------
    public override void Enter()
    {
        base.Enter();
        Hit();
        PlayerScript.OnPlayerHitReleased?.Invoke(PlayerScript.chargeValueIncrementor);
        ballHit = false;
        currentBallSpeed = 0f;
    }

    public void Hit()
    {
        // Debug.Log("Parry!");
        PlayerScript.hitTimer = PlayerScript.releaseDuration;
        parrySpherePosition = PlayerScript.transform.position + transform.forward * PlayerScript.hitDetectionOffset;
        StartCoroutine(HitTime());
    }
    
    IEnumerator HitTime()
    {
        if (!ballToHit)
        {
            HitBox();
        }
        yield return new WaitForSeconds(PlayerScript.releaseDuration);
        PlayerScript.ChangeState(GetComponent<NeutralState>());

    }
    
    //---------------------------------------------------------------------------------
    public override void Tick()
    {
        base.Tick();
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
                HitBall();
                break;
            }
        }
    }

    public void HitBall()
    {
        PlayerScript.OnBallHitByPlayer?.Invoke(PlayerScript.chargeValueIncrementor);
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
        
        ballToHit.GetComponent<BallSM>().ChangeState(ballToHit.GetComponent<HitState>());
    }
    
    
   
    //---------------------------------------------------------------------------------
    public override void Exit()
    {
        base.Exit();
        PlayerScript.chargeValueIncrementor = 0f;
        ballToHit = null;
        _eightDirDirection = Vector3.zero;
    }
}
