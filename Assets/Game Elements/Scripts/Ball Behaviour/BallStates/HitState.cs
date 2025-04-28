using System.Collections;
using UnityEngine;

public class HitState : BallState
{
    [HideInInspector] public Vector3 hitDirection;
    [HideInInspector] public float hitForce;
    
    public override void Enter()
    {        
        base.Enter();
        BallSm.OnHit?.Invoke();
        SetParameters(BallSm.flyingMass, BallSm.flyingLinearDamping, false);
        GameObject ballOwnerPlayer = BallSm.ballOwnerPlayer;
        PlayerScript ballOwnerPlayerScript = BallSm.ballOwnerPlayer.GetComponent<PlayerScript>();
        
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
        hitForce = BallSm.currentBallSpeedVec3.magnitude + 
                   chargeValue * ballOwnerPlayerScript.hitForce;

        StartCoroutine(FreezeTimeRoutine());
        
        if (BallSm.growthType == BallSM.GrowthType.OnHit)
        {
            BallSm.GrowBall();
        }
        
    }

    private IEnumerator FreezeTimeRoutine()
    {
        yield return new WaitForSeconds(hitForce * BallSm.hitFreezeTimeMultiplier);
        BallSm.rb.linearVelocity = hitDirection * hitForce;
        BallSm.SetBallSpeedMinimum(BallSm.rb.linearVelocity.magnitude, hitDirection);
        StartCoroutine(CollisionToggle());

    }

    private IEnumerator CollisionToggle()
    {
        yield return new WaitForSeconds(BallSm.hitStateDuration);
        Physics.IgnoreCollision(BallSm.col, BallSm.ballOwnerPlayer.GetComponent<CapsuleCollider>(), false);
        BallSm.ChangeState(GetComponent<FlyingState>());
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


