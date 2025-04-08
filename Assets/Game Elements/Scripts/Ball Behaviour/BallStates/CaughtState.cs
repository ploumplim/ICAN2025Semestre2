using UnityEngine;

public class CaughtState : BallState
{
    [HideInInspector] public float timer;
    private float _caughtTimeoutTimer;
    [SerializeField] private float caughtTimeout = 0.2f;


    public override void Enter()
    {
        _caughtTimeoutTimer = 0;
        timer = 0;
        SetParameters(BallSm.flyingMass, BallSm.flyingLinearDamping, false);
        base.Enter();
        BallSm.currentBallSpeedVec3 = BallSm.rb.linearVelocity;
        BallSm.rb.linearVelocity = Vector3.zero;
        if (BallSm.ballOwnerPlayer)
        {
            Physics.IgnoreCollision(BallSm.col, BallSm.ballOwnerPlayer.GetComponent<CapsuleCollider>(), true);
        }
        
        //Set the ball's current Y value to it's maximum.
        BallSm.SetMaxHeight(BallSm.flyingMaxHeight,BallSm.flyingMaxHeight);
        
    }

    public override void Tick()
    {            
        _caughtTimeoutTimer += Time.deltaTime;
        timer += Time.deltaTime;
        // Over time, the ball should slow down and stop. Use the slowTime of the ballOwnerPlayer to determine how fast the ball should slow down.
        
        
        
        if (timer >= BallSm.playerImmunityTime)
        {
            Physics.IgnoreCollision(BallSm.col, BallSm.ballOwnerPlayer.GetComponent<CapsuleCollider>(), false);
            // Debug.Log("Player is no longer immune to the ball.");
        }
        
        if (_caughtTimeoutTimer >= BallSm.ballOwnerPlayer.GetComponent<PlayerScript>().chargeTimeLimit + caughtTimeout)
        {
            BallSm.rb.linearVelocity = BallSm.currentBallSpeedVec3;
            BallSm.ChangeState(GetComponent<FlyingState>());
        }
    }

    public override void Exit()
    {
        _caughtTimeoutTimer = 0;
        timer = 0;
        base.Exit();
        if (BallSm.ballOwnerPlayer)
        {
            Physics.IgnoreCollision(BallSm.col, BallSm.ballOwnerPlayer.GetComponent<CapsuleCollider>(), false);
        }
    }

}
