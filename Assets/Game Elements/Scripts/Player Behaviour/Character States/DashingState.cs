using System;
using System.Collections;
using UnityEngine;

public class DashingState : PlayerState
{
    [HideInInspector] public float timer;
    private float dashSpeed;
    
    public override void Enter()
    {
        base.Enter();
        timer = 0;
        
        Physics.IgnoreLayerCollision(PlayerScript.playerLayer, PlayerScript.hazardLayer, true);
        Physics.IgnoreLayerCollision(PlayerScript.playerLayer, PlayerScript.ballLayer, true);
        
        // Set the player's velocity to the dash speed.
        dashSpeed = PlayerScript.dashBurst;


    }

    public override void Tick()
    {
        base.Tick();
        timer += Time.deltaTime;
        //Apply the movement, decreasing the speed of the player over time.
        dashSpeed = Mathf.Lerp(dashSpeed, 0, timer / PlayerScript.dashDuration);
        PlayerScript.Move(dashSpeed, PlayerScript.neutralLerpTime);
        
        
        if (timer >= PlayerScript.dashDuration)
        {
            timer = 0;
            PlayerScript.OnPlayerEndDash?.Invoke();
            PlayerScript.ChangeState(PlayerScript.GetComponent<NeutralState>());
        }
    }
    

    public override void Exit()
    {
        base.Exit();
        Physics.IgnoreLayerCollision(PlayerScript.playerLayer, PlayerScript.hazardLayer, false);
        Physics.IgnoreLayerCollision(PlayerScript.playerLayer, PlayerScript.ballLayer, false);
        
        
    }
    // --------------------- DEPRECATED -----------------------
    // private void CheckPlayerCollisions()
    // {
    //     // Create an overlap sphere using the players position and the roll detection radius.
    //     Collider[] hitColliders =
    //         Physics.OverlapSphere(PlayerScript.transform.position, PlayerScript.rollDetectionRadius);
    //
    //     // If the collider is a player, that player is knock backed unless they are already in the knockback state.
    //     foreach (var hitCollider in hitColliders)
    //     {
    //         if (hitCollider.GetComponent<PlayerScript>())
    //         {
    //             PlayerScript playerDashedInto = hitCollider.GetComponent<PlayerScript>();
    //
    //             // If the player is not the player that is dashing to avoid self knock back.
    //             if (playerDashedInto != PlayerScript)
    //             {
    //                 if (playerDashedInto.currentState == playerDashedInto.GetComponent<DashingState>())
    //                 {
    //                     PlayerScript.ChangeState(GetComponent<KnockbackState>());
    //                     // Both players are dashing, so this player is pushed back.
    //                     Vector3 direction = playerDashedInto.transform.position - PlayerScript.transform.position;
    //                     PlayerScript.rb.AddForce(
    //                         direction *
    //                         ((PlayerScript.rb.linearVelocity.magnitude * PlayerScript.dashKnockbackModifier) *
    //                          PlayerScript.knockbackForce), ForceMode.Impulse);
    //
    //                     // The other player is also pushed back.
    //                     playerDashedInto.ChangeState(playerDashedInto.GetComponent<KnockbackState>());
    //                     // Push the player back in the opposite direction of the dashing player using an impulse.
    //                     playerDashedInto.rb.AddForce(
    //                         -direction *
    //                         ((PlayerScript.rb.linearVelocity.magnitude * PlayerScript.dashKnockbackModifier) *
    //                          PlayerScript.knockbackForce), ForceMode.Impulse);
    //                 }
    //
    //                 else if (playerDashedInto.currentState != playerDashedInto.GetComponent<KnockbackState>())
    //                 {
    //                     playerDashedInto.ChangeState(playerDashedInto.GetComponent<KnockbackState>());
    //
    //                     // Calculate the vector between the two players.
    //                     Vector3 direction = playerDashedInto.transform.position - PlayerScript.transform.position;
    //
    //                     // Push the player back in the opposite direction of the dashing player using an impulse.
    //                     playerDashedInto.rb.AddForce(
    //                         direction *
    //                         ((PlayerScript.rb.linearVelocity.magnitude * PlayerScript.dashKnockbackModifier) *
    //                          PlayerScript.knockbackForce), ForceMode.Impulse);
    //                 }
    //
    //
    //             }
    //         }
    //     }
    //     // clear hit colliders
    //     Array.Clear(hitColliders, 0, hitColliders.Length);
    // }
    //
    // private void CheckBallCollisions()
    // {
    //     Collider[] hitColliders = Physics.OverlapSphere(PlayerScript.transform.position, PlayerScript.rollDetectionRadius);
    //     // If the collider is a ball, the ball is pushed towards the direction of the dash.
    //     foreach (var hitCollider in hitColliders)
    //     {
    //         if (hitCollider.GetComponent<BallSM>() && !_ballWasHit)
    //         {
    //             float currentBallSpeed = hitCollider.GetComponent<Rigidbody>().linearVelocity.magnitude;
    //             hitCollider.GetComponent<BallSM>().rb.linearVelocity = Vector3.zero;
    //             _ballWasHit = true;
    //             BallSM ballDashedInto = hitCollider.GetComponent<BallSM>();
    //             Vector3 direction = direction = ballDashedInto.transform.position - transform.position;
    //             
    //
    //             if (currentBallSpeed <= 0)
    //             {
    //                 currentBallSpeed = 1f;
    //             }
    //
    //             ballDashedInto.ChangeState(ballDashedInto.GetComponent<FlyingState>());
    //             ballDashedInto.rb.AddForce(new Vector3(direction.normalized.x, 0, direction.normalized.y)
    //                                        * (PlayerScript.ballDashForce * currentBallSpeed), ForceMode.Impulse);
    //             
    //             ballDashedInto.ballOwnerPlayer = PlayerScript.gameObject;
    //         }
    //     }
    //     
    //     // clear hit colliders
    //     Array.Clear(hitColliders, 0, hitColliders.Length);
    // }

    
}
