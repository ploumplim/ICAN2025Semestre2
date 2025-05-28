using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class GrabbingState : PlayerState
    {
        private GameObject _caughtBall;
        private float _grabAngleTimer;
        [HideInInspector]public float currentAngle;
        
        public override void Enter()
        {
            base.Enter();
            PlayerScript.OnGrabStateEntered?.Invoke();
            // StartCoroutine(CatchWindowCoroutine());
            currentAngle = PlayerScript.maxGrabAngle;
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

            GrabAngleUpdater();
            
            if (!_caughtBall)
            {
                CatchZone();
            }
            
            PlayerScript.grabCurrentCharge -= PlayerScript.grabDischargeRate * Time.deltaTime;
            // Debug.Log("Grab charge: " + PlayerScript.grabCurrentCharge);
            
            if (PlayerScript.grabCurrentCharge <= 0)
            {
                PlayerScript.ChangeState(GetComponent<NeutralState>());
            }
        }
        
        // public void ChargingForce()
        // {
        //         PlayerScript.chargeValueIncrementor += PlayerScript.chargeRate * Time.deltaTime;
        //         PlayerScript.chargeValueIncrementor = Mathf.Clamp(PlayerScript.chargeValueIncrementor, 
        //             PlayerScript.chargeClamp, 1f);
        //         
        // }

        private void GrabAngleUpdater()
        {
            _grabAngleTimer += Time.deltaTime;
            
            float maxAngle = PlayerScript.maxGrabAngle;
            float minAngle = PlayerScript.minGrabAngle;

            float holdTime = Mathf.Clamp(_grabAngleTimer / PlayerScript.grabTimeLimit, 0, 1);
            float curveValue = PlayerScript.grabShrinkCurve.Evaluate(holdTime);
            currentAngle = Mathf.Lerp(maxAngle, minAngle, curveValue);
        }

        private void CatchZone()
        {
            // Create an overlap sphere equal to the size of the hit detection radius.
            Collider[] hitColliders = Physics.OverlapSphere(PlayerScript.transform.position, PlayerScript.grabDetectionRadius);
            foreach (var hitCollider in hitColliders)
            {
                if (!hitCollider.CompareTag("Ball"))
                {
                    continue;
                }
                
                Vector3 directionToObject = (hitCollider.transform.position - PlayerScript.transform.position).normalized;
                
                
                float angle = Vector3.Angle(PlayerScript.transform.forward, directionToObject);
                
                if (angle <= currentAngle/2f)
                {
                    PlayerScript.OnPlayerCatch?.Invoke();
                    _caughtBall = hitCollider.gameObject;
                    _caughtBall.GetComponent<BallSM>().ballOwnerPlayer = gameObject;
                    _caughtBall.GetComponent<BallSM>().currentBallSpeedVec3 = _caughtBall.GetComponent<BallSM>().rb.linearVelocity;
                    if (_caughtBall.GetComponent<BallSM>().currentState != GetComponent<CaughtState>())
                    {
                        _caughtBall.GetComponent<BallSM>().ChangeState(_caughtBall.GetComponent<CaughtState>());
                    }
                    _caughtBall.GetComponent<BallVisuals>().UpdateFlyingColor(GetComponent<PlayerVisuals>().playerCapMaterial.color);
                    break;
                }
            }
            
        }
        
        public override void Exit()
        {
            base.Exit();
            PlayerScript.OnGrabStateExited?.Invoke();
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
            if (PlayerScript == null) return;

            // Draw the grab detection radius
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, PlayerScript.grabDetectionRadius);


            // Draw the grab angle as a sector
            if (PlayerScript.currentState is GrabbingState grabbingState)
            {
                Handles.color = new Color(1, 0, 1, 0.2f); // Semi-transparent magenta
                Vector3 forward = transform.forward;
                Handles.DrawSolidArc(
                    transform.position,
                    Vector3.up,
                    Quaternion.Euler(0, -grabbingState.currentAngle * 0.5f, 0) * forward,
                    grabbingState.currentAngle,
                    PlayerScript.grabDetectionRadius
                );
            }
        }
    }