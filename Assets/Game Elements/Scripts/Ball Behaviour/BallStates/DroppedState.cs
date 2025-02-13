using UnityEngine;

public class DroppedState : BallState
{
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
