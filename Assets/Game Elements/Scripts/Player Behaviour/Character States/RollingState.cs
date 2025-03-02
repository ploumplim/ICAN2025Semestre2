using System;
using UnityEngine;

public class RollingState : PlayerState
{
    public float timer;
    public override void Enter()
    {
        base.Enter();
        timer = 0;

        
        // check if can pass through ledges.
        if (PlayerScript.canPassThroughLedges)
        {
            // Exclude the ledges layer while dashing.
            Physics.IgnoreLayerCollision(PlayerScript.playerLayer, PlayerScript.ledgeLayer, true);
        }
        
        // push the player in the direction of the moveInput.
        Vector3 direction = PlayerScript.moveInputVector2;
        PlayerScript.rb.AddForce(direction * PlayerScript.rollSpeed, ForceMode.Impulse);
    }

    public override void Tick()
    {
        base.Tick();
        timer += Time.deltaTime;
        //Apply the movement, decreasing the speed of the player over time.
        float decreasingSpeed = Mathf.Lerp(PlayerScript.rollSpeed, 0, GetComponent<RollingState>().timer / PlayerScript.rollDuration);
        PlayerScript.Move(decreasingSpeed, PlayerScript.rollLerpTime);
        // Catch();
        
        if (timer >= PlayerScript.rollDuration)
        {
            timer = 0;
            PlayerScript.PlayerEndedDash?.Invoke();
            PlayerScript.ChangeState(PlayerScript.GetComponent<NeutralState>());
        }
    }

    


    public override void Exit()
    {
        base.Exit();
        if (PlayerScript.canPassThroughLedges)
        {
            // Re-enable collisions with the ledges layer.
            Physics.IgnoreLayerCollision(PlayerScript.playerLayer, PlayerScript.ledgeLayer, false);
        }
    }


    // public void CheckCatch(GameObject caughtObject)
    // {
    //     if (timer <= PlayerScript.catchWindow)
    //     {
    //         PlayerScript.heldBall = caughtObject;
    //         caughtObject.GetComponent<BallSM>().ChangeState(caughtObject.GetComponent<InHandState>());
    //         // Debug.Log("ball caught!");
    //     }
    // }
    
    // public void Catch()
    // {
    //     Collider[] hitColliders = Physics.OverlapSphere(transform.position, PlayerScript.rollDetectionRadius);
    //     // Remove any collider in my hitColliders that are not tagged "Ball"
    //     hitColliders = Array.FindAll(hitColliders, hitCollider => hitCollider.CompareTag("Ball"));
    //     
    //     if (hitColliders.Length == 0) return;
    //     
    //     GameObject caughtBall = hitColliders[0].gameObject;
    //
    //     if (caughtBall)
    //     {
    //         BallSM ballSM = caughtBall.GetComponent<BallSM>();
    //         if (ballSM && ballSM.currentState == ballSM.GetComponent<MidAirState>() ||
    //             ballSM.currentState == ballSM.GetComponent<DroppedState>())
    //         {
    //             if (timer <= PlayerScript.catchWindow)
    //             {
    //                 PlayerScript.heldBall = caughtBall;
    //                 ballSM.ChangeState(ballSM.GetComponent<InHandState>());
    //             }
    //         }
    //                 
    //     }
    // }
}
