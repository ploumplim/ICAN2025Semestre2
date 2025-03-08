using UnityEngine;

public class FlyingState : BallState
{
    //The ball is midair. It will be in this state until it hits the ground.
    [HideInInspector] public float timer;
    
    public override void Enter()
    {
        timer = 0;
        base.Enter();
        SetParameters(BallSm.flyingMass, BallSm.flyingLinearDamping, false);
        
        // The ball's collider should not hit the player's collider.
        Physics.IgnoreCollision(BallSm.col, BallSm.ballOwnerPlayer.GetComponent<CapsuleCollider>(), true);
    }

    public override void Tick()
    {
        base.Tick();
        
        if (timer >= BallSm.playerImmunityTime)
        {
            Physics.IgnoreCollision(BallSm.col, BallSm.ballOwnerPlayer.GetComponent<CapsuleCollider>(), false);
            // Debug.Log("Player is no longer immune to the ball.");
        }
        else
        {
            timer += Time.deltaTime;
            // Debug.Log("Player is immune to the ball.");
        }
        
        // Set the ball's vertical speed to 0.
        BallSm.SetMaxHeight(BallSm.flyingMaxHeight);
        BallSm.FixVerticalSpeed(BallSm.flyingMaxHeight);
    }
}
