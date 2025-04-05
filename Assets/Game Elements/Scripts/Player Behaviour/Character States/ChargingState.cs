using UnityEngine;
using UnityEngine.Serialization;

public class ChargingState : PlayerState
    {
        private GameObject _caughtBall;
        [HideInInspector] public float chargeLimitTimer;
        
        public override void Enter()
        {
            base.Enter();
            PlayerScript.chargeValueIncrementor = 0f;
            chargeLimitTimer = 0f;
        }

        public override void Tick()
        {
            base.Tick();
            PlayerScript.Move(PlayerScript.speed * PlayerScript.chargeSpeedModifier, PlayerScript.neutralLerpTime);
            ChargingForce();

            if (!_caughtBall)
            {
                CatchZone();
            }
            
            if (chargeLimitTimer < PlayerScript.chargeTimeLimit)
            {
                chargeLimitTimer += Time.deltaTime;
            }
            else
            {
                PlayerScript.ChangeState(GetComponent<ReleaseState>());
            }
            
        }
        
        public void ChargingForce()
        {
                PlayerScript.chargeValueIncrementor += PlayerScript.chargeRate * Time.deltaTime;
                PlayerScript.chargeValueIncrementor = Mathf.Clamp(PlayerScript.chargeValueIncrementor, 
                    PlayerScript.chargeClamp, 1f);
                
        }

        public void CatchZone()
        {
            // Create an overlap sphere equal to the size of the hit detection radius.
            Collider[] hitColliders = Physics.OverlapSphere(PlayerScript.transform.position, PlayerScript.hitDetectionRadius);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Ball"))
                {
                    _caughtBall = hitCollider.gameObject;
                    _caughtBall.GetComponent<BallSM>().ballOwnerPlayer = gameObject;
                    if (_caughtBall.GetComponent<BallSM>().currentState != GetComponent<CaughtState>())
                    {
                        _caughtBall.GetComponent<BallSM>().ChangeState(_caughtBall.GetComponent<CaughtState>());
                    }
                    break;
                }
            }
            
        }
        
        public override void Exit()
        {
            base.Exit();
            if (_caughtBall)
            {
                // Make the player collide with the ball again.
                Physics.IgnoreCollision(PlayerScript.GetComponent<CapsuleCollider>(), _caughtBall.GetComponent<SphereCollider>(), false);
                
                // _ballToSlow.GetComponent<Rigidbody>().linearVelocity = _currentBallSpeedVec3;
                _caughtBall = null;
            }
            chargeLimitTimer = 0f;
        }
    }