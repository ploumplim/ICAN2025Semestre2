using System.Collections;
using UnityEngine;

public class HitState : BallState
{
    [HideInInspector] public Vector3 hitDirection;
    [HideInInspector] public float hitForce;
    
    public override void Enter()
    {

        if (BallSm.currentBallSpeedVec3 == Vector3.zero)
        {
            BallSm.currentBallSpeedVec3 = BallSm.rb.linearVelocity;
        }
        
        BallSm.OnHit?.Invoke();
        base.Enter();
        BallSm.rb.linearVelocity = Vector3.zero;
        BallSm.rb.angularVelocity = Vector3.zero;
        BallSm.OnHitStateStart?.Invoke();
        SetParameters(BallSm.flyingMass, BallSm.flyingLinearDamping, false);
        GameObject ballOwnerPlayer = BallSm.ballOwnerPlayer;
        PlayerScript ballOwnerPlayerScript = BallSm.ballOwnerPlayer.GetComponent<PlayerScript>();
        
        
        if (ballOwnerPlayer)
        {
            Physics.IgnoreCollision(BallSm.col, ballOwnerPlayer.GetComponent<CapsuleCollider>(), true);
        }
        
        
        // The ball should now be launched using the hitDirection and the chargeValueIncrementor of the owner player.
        // BallSm.rb.AddForce(hitDirection * 
        //                    (BallSm.currentBallSpeedVec3.magnitude +
        //                     (chargeValue * BallSm.ballOwnerPlayer.GetComponent<PlayerScript>().hitForce)),
        //     ForceMode.Impulse);
        hitForce = BallSm.currentBallSpeedVec3.magnitude + 
                    ballOwnerPlayerScript.hitForce;

        StartCoroutine(FreezeTimeRoutine());
        
        if (BallSm.growthType == BallSM.GrowthType.OnHit)
        {
            BallSm.GrowBall();
        }
        
    }

    private IEnumerator FreezeTimeRoutine()
    {
        // deactivate the ball's collider.
        BallSm.col.enabled = false;
        yield return new WaitForSeconds(hitForce * BallSm.hitFreezeTimeMultiplier);
        BallSm.col.enabled = true;
        BallSm.rb.linearVelocity = hitDirection * hitForce;
        BallSm.SetBallSpeedMinimum(BallSm.rb.linearVelocity.magnitude, hitDirection);
        StartCoroutine(CollisionToggle());

    }

    private IEnumerator CollisionToggle()
    {
        yield return new WaitForSeconds(BallSm.hitStateDuration);
        Physics.IgnoreCollision(BallSm.col, BallSm.ballOwnerPlayer.GetComponent<CapsuleCollider>(), false);
        
        if (hitForce >= BallSm.lethalSpeed)
        {
            BallSm.ChangeState(GetComponent<LethalBallState>());
        }
        else
        {
            BallSm.ChangeState(GetComponent<FlyingState>());

        }
    }

    public override void Tick()
    {
        base.Tick();
        BallSm.FixVerticalSpeed();
    }
    
    public override void Exit()
    { 
        base.Exit();
        if (BallSm.ballOwnerPlayer)
        {
            Physics.IgnoreCollision(BallSm.col, BallSm.ballOwnerPlayer.GetComponent<CapsuleCollider>(), false);
        }
        BallSm.currentBallSpeedVec3 = Vector3.zero;

    }

}


