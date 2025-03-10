using UnityEngine;

public class DroppedState : BallState
{
    public override void Enter()
    {
        base.Enter();
        BallSm.bounces = 0;
        SetParameters(BallSm.groundedMass, BallSm.groundedLinearDamping, true);
        
        // Ball should not collide with any player when it is on the ground.
        
        
        
    }

    public override void Tick()
    {
        base.Tick();
        BallSm.SetMaxHeight(BallSm.groundedMaxHeight);
        BallSm.FixVerticalSpeed(BallSm.groundedMaxHeight);

    }
}
