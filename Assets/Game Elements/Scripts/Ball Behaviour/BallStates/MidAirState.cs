using UnityEngine;

public class MidAirState : BallState
{
    //The ball is midair. It will be in this state until it hits the ground.

    public float timer = 0;
    
    public override void Enter()
    {
        base.Enter();
        timer = 0;
    }
    
    public override void Tick()
    {
        base.Tick();
        timer += Time.deltaTime;
        
        if (!BallSm.groundedBySpeed)
        {
            // If the timer is greater than the timeToBeGrounded, change state to DroppedState.
            if (timer > BallSm.timeToGrounded)
            {
                BallSm.ChangeState(BallSm.GetComponent<DroppedState>());
            }
        }

        if (BallSm.groundedBySpeed)
        {
            if (timer > BallSm.timeToGrounded)
            {
                if (BallSm.rb.linearVelocity.magnitude < BallSm.minimumSpeedToGround)
                {
                    BallSm.ChangeState(BallSm.GetComponent<DroppedState>());
                }
            }
            // If the ball has a certain speed, change state to DroppedState.

        }
        
        // set the ball's forward position to the direction of the velocity.
        BallSm.transform.forward = BallSm.rb.linearVelocity.normalized;
    }
}
