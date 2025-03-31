using System;
using UnityEngine;
public class LethalBallState : BallState
{
    [HideInInspector] public float timer;

    public void Start()
    {
        timer = 0;
    }

    public override void Tick()
    {
        base.Tick();
        
        BallSm.SetMaxHeight(BallSm.flyingMaxHeight);
        BallSm.FixVerticalSpeed(BallSm.flyingMaxHeight);
        
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
}