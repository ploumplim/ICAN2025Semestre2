using System;
using UnityEngine;

public class DashingState : PlayerState
{
    public float timer;
    public override void Enter()
    {
        base.Enter();
        timer = 0;

        
        // check if can pass through ledges.
        if (PlayerScript.canPassThroughLedges)
        {
            // Exclude the ledges layer while dashing.
            Physics.IgnoreLayerCollision(PlayerScript.playerLayer, PlayerScript.ledgeLayer, true);
        }
        
    }

    public override void Tick()
    {
        base.Tick();
        timer += Time.deltaTime;
        //Apply the movement, decreasing the speed of the player over time.
        PlayerScript.Move(PlayerScript.dashSpeed, PlayerScript.dashLerpTime);
        
        if (timer >= PlayerScript.dashDuration)
        {
            timer = 0;
            PlayerScript.PlayerEndedDash?.Invoke();
            PlayerScript.ChangeState(PlayerScript.GetComponent<NeutralState>());
        }
    }

    


    public override void Exit()
    {
        base.Exit();
        if (PlayerScript.canPassThroughLedges)
        {
            // Re-enable collisions with the ledges layer.
            Physics.IgnoreLayerCollision(PlayerScript.playerLayer, PlayerScript.ledgeLayer, false);
        }
    }

    
}
