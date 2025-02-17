using UnityEngine;

public class DroppedState : BallState
{
    public override void Enter()
    {
        base.Enter();
        // Set the balls gravity to true.
        BallSm.rb.useGravity = true;
        BallSm.bounces = 0;
    }

    public override void Tick()
    {
        base.Tick();
        // Set the balls velocity to 0 gradually.
        BallSm.rb.linearVelocity = Vector3.Lerp(BallSm.rb.linearVelocity, Vector3.zero, Time.deltaTime);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
