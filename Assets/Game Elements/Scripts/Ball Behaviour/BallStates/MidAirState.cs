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
        // Set the forward direction of the ball to the direction it is moving.
        // BallSm.transform.forward = BallSm.rb.linearVelocity.normalized;
        timer += Time.deltaTime;
        // If the timer is greater than the timeToBeGrounded, change state to DroppedState.
        if (timer > BallSm.timeToGrounded)
        {
            BallSm.ChangeState(BallSm.GetComponent<DroppedState>());
        }
        
    }
}
