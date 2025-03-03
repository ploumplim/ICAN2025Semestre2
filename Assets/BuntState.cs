using UnityEngine;

public class BuntState : BallState
{
    public override void Enter()
    {
        base.Enter();
        SetParameters(BallSm.buntedMass, BallSm.buntedLinearDamping, true);
    }

    public override void Tick()
    {
        base.Tick();
        // Arrêter d'écouter l'événement quand on quitte l'état
        BallSm.SetMaxHeight(BallSm.buntedMaxHeight);
    }
}