using UnityEngine;

public class PauseState : GameState
{
    public override void Enter()
    {
        Debug.Log("PauseState Enter");
    }

    public override void Tick()
    {
        base.Tick();
    }
}
