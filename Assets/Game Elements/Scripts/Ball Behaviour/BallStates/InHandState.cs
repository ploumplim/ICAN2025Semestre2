using UnityEngine;

public class InHandState : BallState
{
    public override void Enter()
    {
        base.Enter();
        BallSm.rb.isKinematic = true;
        BallSm.sc.enabled = false;
    }
    
    public override void Tick()
    {
        base.Tick();
        // Make the ball face the same direction as the player.
        BallSm.transform.forward = BallSm.player.transform.forward;
    }
    
    
    public override void Exit()
    {
        base.Exit();
        BallSm.rb.isKinematic = false; 
        BallSm.sc.enabled = true;
        // BallSm.rb.useGravity =false;
    }
}
