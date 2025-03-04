using UnityEngine;

public class BuntState : BallState
{
    
    public override void Enter()
    {
        base.Enter();
        SetParameters(BallSm.buntedMass, BallSm.buntedLinearDamping, true);
        BallSm.rb.linearVelocity = Vector3.zero;
    }

    public override void Tick()
    {
        base.Tick();
        // Arrêter d'écouter l'événement quand on quitte l'état
        BallSm.SetMaxHeight(BallSm.buntedMaxHeight);
        BallSm.FixVerticalSpeed(BallSm.buntedMaxHeight);
    }
    
    public override void Exit()
    {
        base.Exit();
    }
}