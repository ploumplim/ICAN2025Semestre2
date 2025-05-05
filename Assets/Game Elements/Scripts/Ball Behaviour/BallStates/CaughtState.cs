// using UnityEngine;
//
// public class CaughtState : BallState
// {
//     [HideInInspector] public float timer;
//     private float _caughtTimeoutTimer;
//     [SerializeField] private float caughtTimeout = 0.2f;
//     [SerializeField] private Transform playerHandTransform;
//
//
//     public override void Enter()
//     {        
//         base.Enter();
//         
//         BallSm.OnBallCaught?.Invoke();
//         
//         // _caughtTimeoutTimer = 0;
//         timer = 0;
//         SetParameters(BallSm.flyingMass, BallSm.flyingLinearDamping, false);
//
//         BallSm.currentBallSpeedVec3 = BallSm.rb.linearVelocity;
//         
//         BallSm.rb.linearVelocity = Vector3.zero;
//         BallSm.rb.angularVelocity = Vector3.zero;
//         // if (BallSm.rb.linearVelocity.magnitude > 0f)
//         // {
//         //     BallSm.rb.linearVelocity /= BallSm.rb.linearVelocity.magnitude;
//         // }
//         
//         if (BallSm.ballOwnerPlayer)
//         {
//             Physics.IgnoreCollision(BallSm.col, BallSm.ballOwnerPlayer.GetComponent<CapsuleCollider>(), true);
//         }
//         
//        
//        
//     }
//
//     public override void Tick()
//     {
//         base.Tick();
//
//         if (BallSm.ballOwnerPlayer)
//         {
//             
//             playerHandTransform = BallSm.ballOwnerPlayer.GetComponent<PlayerScript>().playerHand.transform;
//             
//             // Make the ball move towards the player's hand, reducing the speed of the ball over time to 0.
//             Vector3 targetPosition = playerHandTransform.position;
//             
//             
//             // When the distance between the targetPosition and the ball is less than 0.5f, stop the ball.
//             
//             // if (Vector3.Distance(targetPosition, BallSm.transform.position) < 0.5f)
//             // {
//             //     BallSm.rb.linearVelocity = Vector3.zero;
//             //     BallSm.rb.angularVelocity = Vector3.zero;
//             // }
//             // else
//             // {
//             BallSm.transform.position = targetPosition;
//             // }
//             
//         }
//
//         // _caughtTimeoutTimer += Time.deltaTime;
//         timer += Time.deltaTime;
//         
//         if (timer >= BallSm.playerImmunityTime)
//         {
//             Physics.IgnoreCollision(BallSm.col, BallSm.ballOwnerPlayer.GetComponent<CapsuleCollider>(), false);
//             // Debug.Log("Player is no longer immune to the ball.");
//         }
//         
//         // if (_caughtTimeoutTimer >= BallSm.ballOwnerPlayer.GetComponent<PlayerScript>().chargeTimeLimit + caughtTimeout)
//         // {
//         //     BallSm.rb.linearVelocity = BallSm.currentBallSpeedVec3;
//         //     BallSm.SetBallSpeedMinimum(BallSm.currentBallSpeedVec3.magnitude, BallSm.currentBallSpeedVec3.normalized);
//         //     BallSm.ChangeState(GetComponent<FlyingState>());
//         // }
//     }
//
//     public override void Exit()
//     {
//         base.Exit();
//         // _caughtTimeoutTimer = 0;
//         timer = 0;
//         if (BallSm.ballOwnerPlayer)
//         {
//             Physics.IgnoreCollision(BallSm.col, BallSm.ballOwnerPlayer.GetComponent<CapsuleCollider>(), false);
//         }
//         BallSm.rb.linearVelocity = Vector3.zero;
//         BallSm.rb.angularVelocity = Vector3.zero;
//     }
//
// }
