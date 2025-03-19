using UnityEngine;
    public class ChargingState : PlayerState
    {
        private GameObject _ballToSlow;
        private Vector3 _parrySpherePosition;
        private Vector3 _currentBallSpeedVec3;
        
        public override void Enter()
        {
            base.Enter();
            PlayerScript.chargeValueIncrementor = 0f;
        }

        public override void Tick()
        {
            base.Tick();
            _parrySpherePosition = PlayerScript.transform.position + transform.forward * PlayerScript.hitDetectionOffset;
            PlayerScript.Move(PlayerScript.speed * PlayerScript.chargeSpeedModifier, PlayerScript.neutralLerpTime);
            ChargingForce();

            if (!_ballToSlow)
            {
                SlowMoZone();
            }

            if (_ballToSlow)
            {
                SlowDownBall();
            }
            
        }
        
        public void ChargingForce()
        {
                PlayerScript.chargeValueIncrementor += PlayerScript.chargeRate * Time.deltaTime;
                PlayerScript.chargeValueIncrementor = Mathf.Clamp(PlayerScript.chargeValueIncrementor, 
                    PlayerScript.chargeClamp, 1f);
                // Debug.Log(chargeValueIncrementor);
                
        }

        public void SlowMoZone()
        {
            // Create an overlap sphere equal to the size of the hit detection radius.
            Collider[] hitColliders = Physics.OverlapSphere(PlayerScript.transform.position, PlayerScript.hitDetectionRadius);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Ball"))
                {
                    _ballToSlow = hitCollider.gameObject;
                    _currentBallSpeedVec3 = hitCollider.gameObject.GetComponent<Rigidbody>().linearVelocity;
                    break;
                }
            }
            
        }
        
        public void SlowDownBall()
        {
            Rigidbody ballRb = _ballToSlow.GetComponent<Rigidbody>();
            if (ballRb)
            {
                float currentBallxSpeed = _currentBallSpeedVec3.x;
                float currentBallzSpeed = _currentBallSpeedVec3.z;
                
                
                // apply a force to the ball to slow it down.
                ballRb.AddForce(new Vector3(-currentBallxSpeed * PlayerScript.slowRate, 0, -currentBallzSpeed * PlayerScript.slowRate), ForceMode.Force);
                
                //If the ball is going below the minimum speed, set it to the minimum speed.
                if (ballRb.linearVelocity.magnitude <= PlayerScript.minimumSpeedPercentage * _currentBallSpeedVec3.magnitude)
                {
                    ballRb.linearVelocity = PlayerScript.minimumSpeedPercentage * _currentBallSpeedVec3;
                }
                
            }
        }
        
        public override void Exit()
        {
            base.Exit();
            if (_ballToSlow)
            {
                _ballToSlow.GetComponent<Rigidbody>().linearVelocity = _currentBallSpeedVec3;
                _ballToSlow = null;
            }
        }
    }