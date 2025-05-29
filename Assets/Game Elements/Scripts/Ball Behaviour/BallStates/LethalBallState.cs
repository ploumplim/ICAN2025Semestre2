// using System;
// using System.Collections;
// using UnityEngine;
// public class LethalBallState : BallState
// {
//     [HideInInspector] public float timer;
//
//     public override void Enter()
//     {
//         base.Enter();
//         timer = 0;
//         BallSm.OnBallFlight?.Invoke(BallSm.rb.linearVelocity.magnitude);
//         if (BallSm.ballOwnerPlayer)
//         {
//             Physics.IgnoreCollision(BallSm.col, BallSm.ballOwnerPlayer.GetComponent<CapsuleCollider>(), true);
//         }
//         if (!BallSm.onLethal)
//         {
//             BallSm.OnBallLethal?.Invoke();
//             // Save current speed
//             BallSm.currentBallSpeedVec3 = BallSm.rb.linearVelocity;
//             // Set ball speed to 0.
//             BallSm.rb.linearVelocity = Vector3.zero;
//             // Deactivate the collider
//             BallSm.col.enabled = false;
//             BallSm.onLethal = true;
//             StartCoroutine(FirstTimeLethal());
//         }
//     }
//     
//     public IEnumerator FirstTimeLethal()
//     {
//         yield return new WaitForSeconds(BallSm.firstTimeLethalWaitTime);
//         BallSm.rb.linearVelocity = BallSm.currentBallSpeedVec3;
//         BallSm.col.enabled = true;
//     }
//
//     public override void Tick()
//     {
//         base.Tick();
//         
//         if (timer >= BallSm.playerImmunityTime)
//         {
//             Physics.IgnoreCollision(BallSm.col, BallSm.ballOwnerPlayer.GetComponent<CapsuleCollider>(), false);
//             // Debug.Log("Player is no longer immune to the ball.");
//         }
//         else
//         {
//             timer += Time.deltaTime;
//             // Debug.Log("Player is immune to the ball.");
//         }
//         
//         // if the ball is going under the lethalSpeed, set the ball to the FlyingState.
//         // if (BallSm.rb.linearVelocity.magnitude < BallSm.lethalSpeed)
//         // {
//         //     BallSm.ChangeState(GetComponent<FlyingState>());
//         // }
//     }
//     
//     public override void Exit()
//     {
//         base.Exit();
//         if (BallSm.ballOwnerPlayer)
//         {
//             Physics.IgnoreCollision(BallSm.col, BallSm.ballOwnerPlayer.GetComponent<CapsuleCollider>(), false);
//         }
//     }
// }