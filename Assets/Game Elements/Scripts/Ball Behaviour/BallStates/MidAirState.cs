using UnityEngine;

public class MidAirState : BallState
{
    //The ball is midair. It will be in this state until it hits the ground.

    public override void Tick()
    {
        base.Tick();
        // Set the forward direction of the ball to the direction it is moving.
        BallSm.transform.forward = BallSm.rb.linearVelocity.normalized;
        
    }
}
