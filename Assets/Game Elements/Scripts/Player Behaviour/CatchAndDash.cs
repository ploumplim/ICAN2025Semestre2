// using UnityEngine;
//
// public class CatchAndDash : MonoBehaviour
// {
//     public float detectionRadius = 5f;
//     public LayerMask ballLayer;
//
//     private void Update()
//     {
//         PlayerScript playerScript = GetComponentInParent<PlayerScript>();
//        
//         Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
//         foreach (var hitCollider in hitColliders)
//         {
//             BallSM ballSM = hitCollider.GetComponent<BallSM>();
//             if (ballSM != null && ballSM.currentState == ballSM.GetComponent<FlyingState>())
//             {
//                 if (playerScript.currentState == playerScript.GetComponent<RollingState>())
//                 {
//                     // Debug.Log("Ball is in MidAirState & player Roll");
//                     ballSM.GetComponent<BallSM>().ChangeState(ballSM.GetComponent<InHandState>());
//                     playerScript.heldBall = ballSM.gameObject;
//                     // Add your logic here
//                 }
//                 
//             }
//         }
//     }
//
//     private void OnDrawGizmos()
//     {
//         Gizmos.color = Color.red;
//         Gizmos.DrawWireSphere(transform.position, detectionRadius);
//     }
// }