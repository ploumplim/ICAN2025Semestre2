using UnityEngine;

public class MovingState : PlayerState
{
    public override void Tick()
    {
        base.Tick();
        PlayerScript.Move();
        
        // Change the player's rotation so that it matches the direction its moving.
        PlayerScript.transform.rotation = Quaternion.LookRotation(new Vector3(PlayerScript.rb.linearVelocity.x,
            0, PlayerScript.rb.linearVelocity.z));
    }
}
