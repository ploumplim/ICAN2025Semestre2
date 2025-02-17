using UnityEngine;

public class MovingState : PlayerState
{
    public override void Tick()
    {
        base.Tick();
        PlayerScript.Move(false);
        
    }
}
