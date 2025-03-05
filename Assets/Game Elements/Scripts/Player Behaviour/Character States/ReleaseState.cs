using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReleaseState : PlayerState
{
    [HideInInspector]public GameObject ballToParry;
    [HideInInspector]public Vector3 parrySpherePosition;
    [HideInInspector]public float currentBallSpeed;

    [HideInInspector] public bool ballHit;
    //---------------------------------------------------------------------------------
    public override void Enter()
    {
        base.Enter();
        Hit();
        ballHit = false;
    }

    public void Hit()
    {
        // Debug.Log("Parry!");
        PlayerScript.PlayerPerformedHit?.Invoke();
        PlayerScript.hitTimer = PlayerScript.releaseDuration;
        StartCoroutine(HitTime());
    }
    
    IEnumerator HitTime()
    {
        
        yield return new WaitForSeconds(PlayerScript.releaseDuration);
        // Debug.Log("Parry Ended");
        PlayerScript.ChangeState(GetComponent<NeutralState>());
    }
    
    //---------------------------------------------------------------------------------
    public override void Tick()
    {
        base.Tick();
        // // Create a value called "parrySphereDistance" which is equal to the radius of the parry sphere, so that the sphere
        // // is always at a distance from the player that is equal to the radius of the sphere.
        // float parrySphereDistance = PlayerScript.parryDetectionRadius;
        //
        // // Now create a vector3 called "parrySpherePosition" which is equal to the player's forward
        // // position plus the parrySphereDistance.
        parrySpherePosition = PlayerScript.transform.position;
        PlayerScript.Move(PlayerScript.speed * PlayerScript.chargeSpeedModifier, PlayerScript.chargeLerpTime);   
        HitBox();
        HitTheBall();
    }

    public void HitBox()
    {
        // create an overlap sphere that detects the ball. If it did, set the ball to parry to the ball that was detected.
        Collider[] hitColliders = Physics.OverlapSphere(parrySpherePosition, PlayerScript.hitDetectionRadius);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].gameObject.CompareTag("Ball"))
            {
                ballToParry = hitColliders[i].gameObject;
            }
            i++;
        }
    }

    public void HitTheBall()
    {
        if (ballToParry && !ballHit)
        {
            float verticalPercent;
            float minimumBallSpeed = ballToParry.GetComponent<BallSM>().minimumSpeedToGround;
            Rigidbody ballRigidbody = ballToParry.GetComponent<Rigidbody>();
            if (ballRigidbody != null)
            {
                currentBallSpeed = ballToParry.GetComponent<Rigidbody>().linearVelocity.magnitude;

                if (ballToParry.GetComponent<BallSM>().currentState != ballToParry.GetComponent<FlyingState>())
                {
                    currentBallSpeed = minimumBallSpeed * 1.3f;
                    verticalPercent = 0f;
                }
                else
                {
                    verticalPercent = PlayerScript.verticalPercent;
                }

                ballRigidbody.linearVelocity = Vector3.zero;
                GameObject player = PlayerScript.gameObject;
                Vector3 direction;
                ballToParry.GetComponent<BallSM>().ChangeState(ballToParry.GetComponent<FlyingState>());
                switch (PlayerScript.hitType)
                {
                    case PlayerScript.HitType.ForwardHit:
                        // Send the ball in the direction the player is facing.
                        direction = new Vector3(player.transform.forward.x, verticalPercent, player.transform.forward.z).normalized;
                        ApplyForce(ballRigidbody, direction);
                        // Debug.Log("Parry");

                        break;
                    case PlayerScript.HitType.ReflectiveHit:
                        direction = ballToParry.transform.position - transform.position;
                        direction = new Vector3(direction.x, verticalPercent, direction.z).normalized;

                        ApplyForce(ballRigidbody, direction);
                        break;
                }
                ballHit = true;
            }
            ballToParry = null;
            // PlayerScript.ChangeState(GetComponent<NeutralState>());
        }

    }

    public void ApplyForce(Rigidbody ballRigidBody, Vector3 direction)
    { 
        ballRigidBody.AddForce(direction * (PlayerScript.chargeValueIncrementor *
                                          PlayerScript.hitForce * currentBallSpeed), ForceMode.Impulse);
    }
    
    //---------------------------------------------------------------------------------
    public override void Exit()
    {
        base.Exit();
        PlayerScript.chargeValueIncrementor = 0f;
        ballToParry = null;

    }
}
