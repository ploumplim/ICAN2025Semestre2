using UnityEngine;

public class CaughtState : BallState
{
    [HideInInspector] public float timer;
    private float _moveTimer;
    [SerializeField] private Transform playerHandTransform;
    private Vector3 currentBallDirection;


    public override void Enter()
    {        
        base.Enter();
        BallSm.OnBallCaught?.Invoke();
        // Save the current ball direction
        currentBallDirection = BallSm.rb.linearVelocity.normalized;
        
        
        timer = 0;
        SetParameters(BallSm.flyingMass, BallSm.flyingLinearDamping, false);

        
        BallSm.rb.linearVelocity = Vector3.zero;
        BallSm.rb.angularVelocity = Vector3.zero;
        
        if (BallSm.ballOwnerPlayer)
        {
            playerHandTransform = BallSm.ballOwnerPlayer.GetComponent<PlayerScript>().playerHand.transform;
            Physics.IgnoreCollision(BallSm.col, BallSm.ballOwnerPlayer.GetComponent<CapsuleCollider>(), true);
        }
        
    }

    public override void Tick()
    {
        base.Tick();
        Vector3 currentBallPosition = BallSm.transform.position;

        if (BallSm.ballOwnerPlayer && playerHandTransform)
        {
            float r = Mathf.Clamp01(_moveTimer / BallSm.ballMoveDuration);
            
            float curveVal = BallSm.GetComponent<BallSM>().movementCurve.Evaluate(r);
            if (transform.position != playerHandTransform.position)
            {
                _moveTimer += Time.deltaTime;
                Vector3 newPosition = Vector3.Lerp(currentBallPosition, playerHandTransform.position, curveVal);
                BallSm.rb.MovePosition(newPosition);
            }
            else
            {
                _moveTimer = 0;
            }

            if (r >= 1)
            {
                _moveTimer = 0;
            }

            if (BallSm.ballOwnerPlayer.GetComponent<PlayerScript>().currentState != BallSm.ballOwnerPlayer.GetComponent<GrabbingState>())
            {
                // Debug.Log("Ball should be released");;
                BallSm.rb.AddForce(currentBallDirection * BallSm.currentBallSpeedVec3.magnitude, ForceMode.VelocityChange);
                BallSm.ChangeState(GetComponent<FlyingState>());
                _moveTimer = 0;
                // The Ball should fly in the direction in which the player is facing.
            }
        }

        timer += Time.deltaTime;
        
        if (timer >= BallSm.playerImmunityTime)
        {
            Physics.IgnoreCollision(BallSm.col, BallSm.ballOwnerPlayer.GetComponent<CapsuleCollider>(), false);
            // Debug.Log("Player is no longer immune to the ball.");
        }
    }

    public override void Exit()
    {
        base.Exit();
        timer = 0;
        _moveTimer = 0;
        BallSm.CaughtStateEnded?.Invoke();
        if (BallSm.ballOwnerPlayer)
        {
            Physics.IgnoreCollision(BallSm.col, BallSm.ballOwnerPlayer.GetComponent<CapsuleCollider>(), false);
        }
        // BallSm.rb.linearVelocity = Vector3.zero;
        // BallSm.rb.angularVelocity = Vector3.zero;
    }

}
