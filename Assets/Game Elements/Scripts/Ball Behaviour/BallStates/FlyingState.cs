using UnityEngine;

public class FlyingState : BallState
{
    //The ball is midair. It will be in this state until it hits the ground.
    
    public override void Enter()
    {
        base.Enter();
        SetParameters(BallSm.flyingMass, BallSm.flyingLinearDamping, false);
        
    }

    public override void Tick()
    {
        base.Tick();
        // Set the ball's vertical speed to 0.
        BallSm.SetMaxHeight(BallSm.flyingMaxHeight);
        BallSm.FixVerticalSpeed(BallSm.flyingMaxHeight);
    }
}
