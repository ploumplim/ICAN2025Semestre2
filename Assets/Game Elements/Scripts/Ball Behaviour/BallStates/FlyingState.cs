using UnityEngine;

public class FlyingState : BallState
{
    //The ball is midair. It will be in this state until it hits the ground.
    
    public override void Enter()
    {
        base.Enter();
        //Set the rigid body's linear damping.
        BallSm.rb.linearDamping = BallSm.flyingLinearDamping;
        //Set the ball's mass.
        BallSm.rb.mass = BallSm.flyingMass;
        // Remove the ball's gravity.
        BallSm.rb.useGravity = false;
        
    }

    public override void Tick()
    {
        base.Tick();
        // Set the ball's vertical speed to 0.
        BallSm.rb.linearVelocity = new Vector3(BallSm.rb.linearVelocity.x, 0, BallSm.rb.linearVelocity.z);
    }
}
