using UnityEngine;

public class HitState : BallState
{
    [HideInInspector] public Vector3 hitDirection;
    [HideInInspector] public float timer;
    [HideInInspector] public float hitTimer;
    
    public override void Enter()
    {        
        base.Enter();
        SetParameters(BallSm.flyingMass, BallSm.flyingLinearDamping, false);
        GameObject ballOwnerPlayer = BallSm.ballOwnerPlayer;
        PlayerScript ballOwnerPlayerScript = BallSm.ballOwnerPlayer.GetComponent<PlayerScript>();
        
        timer = 0;
        hitTimer = 0;
        if (ballOwnerPlayer)
        {
            Physics.IgnoreCollision(BallSm.col, ballOwnerPlayer.GetComponent<CapsuleCollider>(), true);
        }


        float chargeValue = Mathf.Clamp(ballOwnerPlayerScript.chargeValueIncrementor, ballOwnerPlayerScript.chargeClamp, 1f);
        
        
        // The ball should now be launched using the hitDirection and the chargeValueIncrementor of the owner player.
        // BallSm.rb.AddForce(hitDirection * 
        //                    (BallSm.currentBallSpeedVec3.magnitude +
        //                     (chargeValue * BallSm.ballOwnerPlayer.GetComponent<PlayerScript>().hitForce)),
        //     ForceMode.Impulse);
        
        float hitForce = BallSm.currentBallSpeedVec3.magnitude + 
                         chargeValue * ballOwnerPlayerScript.hitForce;
        BallSm.rb.linearVelocity = hitDirection * hitForce;
        
        BallSm.SetBallSpeedMinimum(BallSm.rb.linearVelocity.magnitude, hitDirection);
        
    }

    public override void Tick()
    {
        hitTimer += Time.deltaTime;
        timer += Time.deltaTime;
        BallSm.FixVerticalSpeed();
        if (timer >= BallSm.playerImmunityTime)
        {
            Physics.IgnoreCollision(BallSm.col, BallSm.ballOwnerPlayer.GetComponent<CapsuleCollider>(), false);
            // Debug.Log("Player is no longer immune to the ball.");
        }
        if (hitTimer >= BallSm.hitStateDuration)
        {
            BallSm.ChangeState(GetComponent<FlyingState>());
        }
    }
    
    public override void Exit()
    { 
        base.Exit();
        hitTimer = 0;
        if (BallSm.ballOwnerPlayer)
        {
            Physics.IgnoreCollision(BallSm.col, BallSm.ballOwnerPlayer.GetComponent<CapsuleCollider>(), false);
        }
        BallSm.currentBallSpeedVec3 = Vector3.zero;

    }

}


