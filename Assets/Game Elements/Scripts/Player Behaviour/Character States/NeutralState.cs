using UnityEngine;

public class NeutralState : PlayerState
{
    
    
    public override void Tick()
    {
        base.Tick();
        PlayerScript.Move(PlayerScript.speed, PlayerScript.neutralLerpTime);
    }
}
