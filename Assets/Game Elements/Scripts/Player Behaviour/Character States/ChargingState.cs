using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class ChargingState : PlayerState
    {
        private GameObject _caughtBall;
        private float _grabAngleTimer;
        [HideInInspector]public float currentAngle;
        
        public override void Enter()
        {
            base.Enter();
            // StartCoroutine(CatchWindowCoroutine());
        }
        
        // private IEnumerator CatchWindowCoroutine()
        // {
        //     // CatchZone();
        //     yield return new WaitForSeconds(PlayerScript.catchWindow);
        // }

        public override void Tick()
        {
            base.Tick();
            PlayerScript.Move(PlayerScript.speed * PlayerScript.chargeSpeedModifier, PlayerScript.neutralLerpTime);
            // ChargingForce();
            
            
            if (!_caughtBall)
            {
                CatchZone();
            }
            
            
        }
        
        // public void ChargingForce()
        // {
        //         PlayerScript.chargeValueIncrementor += PlayerScript.chargeRate * Time.deltaTime;
        //         PlayerScript.chargeValueIncrementor = Mathf.Clamp(PlayerScript.chargeValueIncrementor, 
        //             PlayerScript.chargeClamp, 1f);
        //         
        // }

        public void CatchZone()
        {
            _grabAngleTimer += Time.deltaTime;
            
            float maxAngle = PlayerScript.maxGrabAngle;
            float minAngle = PlayerScript.minGrabAngle;

            float holdTime = Mathf.Clamp(_grabAngleTimer / PlayerScript.grabTimeLimit, 0, 1);
            float curveValue = PlayerScript.grabShrinkCurve.Evaluate(holdTime);
            currentAngle = Mathf.Lerp(maxAngle, minAngle, curveValue);
            
            // Create an overlap sphere equal to the size of the hit detection radius.
            Collider[] hitColliders = Physics.OverlapSphere(PlayerScript.transform.position, PlayerScript.grabDetectionRadius);
            foreach (var hitCollider in hitColliders)
            {
                Vector3 directionToObject = (hitCollider.transform.position - PlayerScript.transform.position).normalized;
                
                float angle = Vector3.Angle(PlayerScript.transform.forward, directionToObject);
                if (angle <= currentAngle)
                {
                    if (hitCollider.CompareTag("Ball"))
                    {
                        PlayerScript.OnPlayerCatch?.Invoke();
                        _caughtBall = hitCollider.gameObject;
                        _caughtBall.GetComponent<BallSM>().ballOwnerPlayer = gameObject;
                        _caughtBall.GetComponent<BallSM>().currentBallSpeedVec3 =
                            _caughtBall.GetComponent<BallSM>().rb.linearVelocity;
                        if (_caughtBall.GetComponent<BallSM>().currentState != GetComponent<CaughtState>())
                        {
                            _caughtBall.GetComponent<BallSM>().ChangeState(_caughtBall.GetComponent<CaughtState>());
                        }

                        break;
                    }
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
            _grabAngleTimer = 0;
            currentAngle = PlayerScript.maxGrabAngle;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            // Draw the forward direction
            Gizmos.DrawRay(transform.position, transform.forward * PlayerScript.grabDetectionRadius);
            // Draw the shrinking angle
            float halfAngle = currentAngle / 2f;
            Vector3 leftBoundary = Quaternion.AngleAxis(-halfAngle, Vector3.up) * transform.forward;
            Vector3 rightBoundary = Quaternion.AngleAxis(halfAngle, Vector3.up) * transform.forward;

            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(transform.position, leftBoundary * PlayerScript.grabDetectionRadius);
            Gizmos.DrawRay(transform.position, rightBoundary * PlayerScript.grabDetectionRadius);

            // Optionally, draw an arc to represent the angle
            Gizmos.color = Color.magenta;
            int segments = 20;
            for (int i = 0; i < segments; i++)
            {
                float angle1 = -halfAngle + (i * (currentAngle / segments));
                float angle2 = -halfAngle + ((i + 1) * (currentAngle / segments));

                Vector3 point1 = Quaternion.AngleAxis(angle1, Vector3.up) * transform.forward * PlayerScript.grabDetectionRadius;
                Vector3 point2 = Quaternion.AngleAxis(angle2, Vector3.up) * transform.forward * PlayerScript.grabDetectionRadius;

                Gizmos.DrawLine(transform.position + point1, transform.position + point2);
            }
            
            
        }
    }