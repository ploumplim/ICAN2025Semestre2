using System.Collections;
using UnityEngine;

public class HitState : BallState
{
    [HideInInspector] public Vector3 hitDirection;
    [HideInInspector] public float hitForce;
    private float _timer;
    
    public override void Enter()
    {
        
        base.Enter();
        if (BallSm.currentBallSpeedVec3 == Vector3.zero)
        {
            BallSm.currentBallSpeedVec3 = BallSm.rb.linearVelocity;
        }
        
        BallSm.OnHit?.Invoke();
        BallSm.OnHitStateStart?.Invoke();
        
        SetParameters(BallSm.flyingMass, BallSm.flyingLinearDamping, false);
        GameObject HitStateBallOwnerPlayer = BallSm.ballOwnerPlayer;
        PlayerScript ballOwnerPlayerScript = BallSm.ballOwnerPlayer.GetComponent<PlayerScript>();
        
        //TODO : Faire que la balle ne puisse plus hit le playerOwner
       
        
        if (HitStateBallOwnerPlayer)
        {
            Physics.IgnoreCollision(BallSm.col, HitStateBallOwnerPlayer.GetComponent<CapsuleCollider>(), true);
        }

        BallSm.rb.linearVelocity = hitDirection * (BallSm.currentBallSpeedVec3.magnitude + BallSm.ballOwnerPlayer.GetComponent<PlayerScript>().hitForce);
        
        BallSm.SetBallSpeedMinimum(BallSm.rb.linearVelocity.magnitude, hitDirection);
        
        if (BallSm.growthType == BallSM.GrowthType.OnHit)
        {
            BallSm.GrowBall();
        }
        
    }
    
    
    

    public override void Tick()
    {
        base.Tick();
        _timer += Time.deltaTime;
        BallSm.FixVerticalSpeed();
        if (_timer >= BallSm.hitStateDuration)
        {
            BallSm.ChangeState(GetComponent<FlyingState>());
        }
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


