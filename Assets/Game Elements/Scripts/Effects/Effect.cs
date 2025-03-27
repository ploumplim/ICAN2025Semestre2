using UnityEngine;

public class Effect : MonoBehaviour
{
    public void SpeedEffect(GameObject ball)
    {
        Debug.Log(ball.GetComponent<Rigidbody>().linearVelocity);
        Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();
        if (ballRigidbody != null)
        {
            ballRigidbody.linearVelocity *= 2;
            
            Debug.Log(ballRigidbody.linearVelocity);
        }
    }
   public void BounceEffect(GameObject ball)
   {
       Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();
       if (ballRigidbody != null)
       {
           // Re-enable gravity
           ballRigidbody.useGravity = true;
           ballRigidbody.linearVelocity /= 2;
           

           // Apply an upward force to simulate the bounce
           ballRigidbody.AddForce(Vector3.up * 5f, ForceMode.Impulse);
       } 
   }
   // public void NoGravityEffect(GameObject ball)
   // {
   //     Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();
   //     if (ballRigidbody != null)
   //     {
   //         // Disable gravity
   //         ballRigidbody.useGravity = false;
   //         ballRigidbody.mass = 0f;
   //
   //         // Add a physics material with high bounciness
   //         Collider ballCollider = ball.GetComponent<Collider>();
   //         if (ballCollider != null)
   //         {
   //             PhysicsMaterial bounceMaterial = new PhysicsMaterial();
   //             bounceMaterial.bounciness = 1f;
   //             bounceMaterial.frictionCombine = PhysicsMaterialCombine.Minimum;
   //             bounceMaterial.bounceCombine = PhysicsMaterialCombine.Maximum;
   //             ballCollider.material = bounceMaterial;
   //         }
   //     }
   // }
   public void PlayerMagnetEffect(GameObject ball)
   {
       Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();
       if (ballRigidbody != null)
       {
           GameObject player = GameObject.FindWithTag("Player");
           if (player != null)
           {
               Vector3 directionToPlayer = (player.transform.position - ball.transform.position).normalized;
               float speed = 10f; // Adjust the speed as needed
               ballRigidbody.linearVelocity = directionToPlayer * speed;
               Debug.LogWarning(directionToPlayer);
           }
           else
           {
               Debug.LogWarning("Player not found in the scene.");
           }
       }
   }
}
