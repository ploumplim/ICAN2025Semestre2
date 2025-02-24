using UnityEngine;

public class DroppedState : BallState
{
    public override void Enter()
    {
        base.Enter();
        // Set the balls gravity to true.
        // BallSm.rb.useGravity = true;
        BallSm.bounces = 0;
        
        //Set the rigid body's linear damping.
        BallSm.rb.linearDamping = BallSm.groundedLinearDamping;
        
        // Set the ball's mass.
        BallSm.rb.mass = BallSm.groundedMass;
        
        // The ball cannot be parried.
        BallSm.canBeParried = false;
    }
    
}
