using System;
using UnityEngine;
public class LethalBallState : BallState
{
    [HideInInspector] public float timer;

    public override void Enter()
    {
        base.Enter();
        timer = 0;
        if (BallSm.ballOwnerPlayer)
        {
            Physics.IgnoreCollision(BallSm.col, BallSm.ballOwnerPlayer.GetComponent<CapsuleCollider>(), true);
        }
    }

    public override void Tick()
    {
        base.Tick();
        
        if (timer >= BallSm.playerImmunityTime)
        {
            Physics.IgnoreCollision(BallSm.col, BallSm.ballOwnerPlayer.GetComponent<CapsuleCollider>(), false);
            // Debug.Log("Player is no longer immune to the ball.");
        }
        else
        {
            timer += Time.deltaTime;
            // Debug.Log("Player is immune to the ball.");
        }
        
        // if the ball is going under the lethalSpeed, set the ball to the FlyingState.
        if (BallSm.rb.linearVelocity.magnitude < BallSm.lethalSpeed)
        {
            BallSm.ChangeState(GetComponent<FlyingState>());
        }
    }
    
    public override void Exit()
    {
        base.Exit();
        if (BallSm.ballOwnerPlayer)
        {
            Physics.IgnoreCollision(BallSm.col, BallSm.ballOwnerPlayer.GetComponent<CapsuleCollider>(), false);
        }
    }
}