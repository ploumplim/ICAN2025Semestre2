using UnityEngine;
public class LethalBallState : BallState
{
    public override void Tick()
    {
        base.Tick();
        
        BallSm.SetMaxHeight(BallSm.flyingMaxHeight);
        BallSm.FixVerticalSpeed(BallSm.flyingMaxHeight);
        
        // if the ball is going under the lethalSpeed, set the ball to the FlyingState.
        if (BallSm.rb.linearVelocity.magnitude < BallSm.lethalSpeed)
        {
            BallSm.ChangeState(GetComponent<FlyingState>());
        }
    }
}