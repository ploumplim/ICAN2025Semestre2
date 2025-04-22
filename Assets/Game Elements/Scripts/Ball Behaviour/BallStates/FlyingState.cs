using UnityEngine;

public class FlyingState : BallState
{
    
    public override void Enter()
    {
        base.Enter();
        
        SetParameters(BallSm.flyingMass, BallSm.flyingLinearDamping, false);
        BallSm.OnBallFlight?.Invoke(BallSm.rb.linearVelocity.magnitude);
        BallSm.currentBallSpeedVec3 = BallSm.rb.linearVelocity; 
        
        
    }

    public override void Tick()
    {
        base.Tick();
        
        // if the ball is going above the lethal speed, set the ball to the LethalBallState.
        if (BallSm.rb.linearVelocity.magnitude >= BallSm.lethalSpeed)
        {
            BallSm.ChangeState(GetComponent<LethalBallState>());
        }
    }
    
    public override void Exit()
    {
        base.Exit();
    }
}
