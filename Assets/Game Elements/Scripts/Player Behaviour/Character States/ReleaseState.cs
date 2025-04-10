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
        StartCoroutine(HitTime());
    }
    
    IEnumerator HitTime()
    {
        yield return new WaitForSeconds(PlayerScript.releaseDuration);
        PlayerScript.ChangeState(GetComponent<NeutralState>());
    }
    
    //---------------------------------------------------------------------------------
    public override void Tick()
    {
        base.Tick();
        
        parrySpherePosition = PlayerScript.transform.position + transform.forward * PlayerScript.hitDetectionOffset;
        
        // PlayerScript.Move(PlayerScript.speed * PlayerScript.releaseSpeedModifier, PlayerScript.chargeLerpTime);


        if (!ballToHit)
        {
            HitBox();
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
        Rigidbody ballRigidbody = ballToHit.GetComponent<Rigidbody>();
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
    
    
    // public void HitTheBall()
    // {
    //     if (!ballHit)
    //     {
    //         PlayerScript.OnBallHitByPlayer?.Invoke(PlayerScript.chargeValueIncrementor);
    //         float verticalPercent;
    //         // float minimumBallSpeed = ballToHit.GetComponent<BallSM>().hitStateDuration;
    //         Rigidbody ballRigidbody = ballToHit.GetComponent<Rigidbody>();
    //         if (ballRigidbody != null)
    //         {
    //             currentBallSpeed = ballToHit.GetComponent<Rigidbody>().linearVelocity.magnitude;
    //             // Debug.Log("Ball Speed: " + currentBallSpeed);
    //             
    //             if (ballToHit.GetComponent<BallSM>().currentState != ballToHit.GetComponent<FlyingState>() &&
    //                 ballToHit.GetComponent<BallSM>().currentState != ballToHit.GetComponent<LethalBallState>())
    //             {
    //                 // currentBallSpeed = minimumBallSpeed * 1.3f;
    //                 verticalPercent = 0f;
    //             }
    //             else
    //             {
    //                 verticalPercent = PlayerScript.verticalPercent;
    //             }
    //
    //             // ballRigidbody.linearVelocity = Vector3.zero;
    //             
    //             
    //             GameObject player = PlayerScript.gameObject;
    //             Vector3 direction;
    //             // Set the ball owner to the player that hit the ball.
    //
    //             ballToHit.GetComponent<BallSM>().ballOwnerPlayer = player;
    //             
    //             PlayerScript.OnBallHitEventMethod(ballToHit); // Call the event that triggers the ball hit.
    //
    //             ballToHit.GetComponent<BallSM>().ChangeState(ballToHit.GetComponent<FlyingState>()); // Change the ball's state to flying.
    //             
    //
    //             switch (PlayerScript.hitType)
    //             {
    //                 case PlayerScript.HitType.ForwardHit:
    //                     // Send the ball in the direction the player is facing.
    //                     direction = new Vector3(player.transform.forward.x, verticalPercent, player.transform.forward.z).normalized;
    //                     ApplyForce(ballRigidbody, direction);
    //                     // Debug.Log("Parry");
    //
    //                     break;
    //                 case PlayerScript.HitType.ReflectiveHit:
    //                     direction = ballToHit.transform.position - transform.position;
    //                     direction = new Vector3(direction.x, verticalPercent, direction.z).normalized;
    //                     ApplyForce(ballRigidbody, direction);
    //                     break;
    //                 case PlayerScript.HitType.EightDirHit:
    //                     
    //                     direction = _eightDirDirection;
    //                     
    //                     if (direction == Vector3.zero)
    //                     {
    //                         
    //                         //direction = EightDirVector3DirectionFromForward();
    //                         Debug.Log("Direction is zero : new direction is " + direction);
    //                     }
    //                     
    //                     direction = new Vector3(direction.x, verticalPercent, direction.z).normalized;
    //                     ApplyForce(ballRigidbody, direction);
    //                     break;
    //             }
    //             // Check the ball's growthType. If it's OnHit, grow the ball.
    //             if (ballToHit.GetComponent<BallSM>().growthType == BallSM.GrowthType.OnHit)
    //             {
    //                 ballToHit.GetComponent<BallSM>().GrowBall();
    //             }
    //             
    //             
    //             
    //             ballHit = true;
    //         }
    //         ballToHit = null;
    //     }
    //
    // }

    public void ApplyForce(Rigidbody ballRigidBody, Vector3 direction)
    { 
        // Calculate the new velocity based on the direction and current speed
        float newSpeed = PlayerScript.chargeValueIncrementor * PlayerScript.hitForce * currentBallSpeed;
        ballRigidBody.linearVelocity = direction.normalized * newSpeed;
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
