using UnityEngine;

public class DeadState : PlayerState
{
    public override void Enter()
    {
        base.Enter();
        PlayerScript.OnPlayerDeath?.Invoke();
        // ignore the ballLayer.
        Physics.IgnoreLayerCollision(PlayerScript.playerLayer,
            PlayerScript.ballLayer, true);
        
        // Make Kinematic
        PlayerScript.rb.isKinematic = true;
        
    }
    
    public override void Exit()
    {
        base.Exit();
        // stop ignoring the ballLayer.
        Physics.IgnoreLayerCollision(PlayerScript.playerLayer,
            PlayerScript.ballLayer, false);
        
        // Make non-Kinematic
        PlayerScript.rb.isKinematic = false;
    }
}
