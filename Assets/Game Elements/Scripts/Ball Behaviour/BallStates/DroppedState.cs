using UnityEngine;

public class DroppedState : BallState
{
    public override void Enter()
    {
        base.Enter();
        BallSm.bounces = 0;
        // BallSm.ResetBallSize();
        SetParameters(BallSm.groundedMass, BallSm.groundedLinearDamping, true);
        
        // Ball should not collide with any player when it is on the ground.
        Physics.IgnoreLayerCollision(BallSm.ballColliderLayer, BallSm.playerColliderLayer, true);
        
        
        
    }

    public override void Tick()
    {
        BallSm.SetMaxHeight(BallSm.minHeight, BallSm.groundedMaxHeight);

    }
    
    public override void Exit()
    {
        base.Exit();
        Physics.IgnoreLayerCollision(BallSm.ballColliderLayer, BallSm.playerColliderLayer, false);
    }
}
