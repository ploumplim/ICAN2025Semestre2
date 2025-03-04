using UnityEngine;

public class DroppedState : BallState
{
    public override void Enter()
    {
        base.Enter();
        // Set the balls gravity to true.
        // BallSm.rb.useGravity = true;
        BallSm.bounces = 0;
        SetParameters(BallSm.groundedMass, BallSm.groundedLinearDamping, true);
    }

    public override void Tick()
    {
        base.Tick();
        BallSm.SetMaxHeight(BallSm.groundedMaxHeight);
        BallSm.FixVerticalSpeed(BallSm.groundedMaxHeight);

    }
}
