using UnityEngine;

public class BallLauncher : MonoBehaviour
{
    public GameObject ball; // The ball to be launched, assigned in the inspector
    public Transform launchPoint; // The point from which the ball will be launched
    public float launchForce = 10f; // The force with which the ball will be launched
    public float launchInterval = 5f; // Interval between launches in seconds
    private float timeSinceLastLaunch = 0f; // Time elapsed since the last launch

    private void Start()
    {
        LaunchBall();
    }

    void Update()
    {
        timeSinceLastLaunch += Time.deltaTime;

        if (timeSinceLastLaunch >= launchInterval)
        {
            LaunchBall();
            timeSinceLastLaunch = 0f;
        }
    }

    void LaunchBall()
    {
        // Ensure the ball is not null
        if (ball != null)
        {
            
            // Reset the ball's position and rotation to the launch point
            ball.transform.position = launchPoint.position;
            ball.transform.rotation = launchPoint.rotation;

            // Get the Rigidbody component of the ball
            Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();

            // Apply a force to the ball to launch it along the x-axis of the object
            if (ballRigidbody != null)
            {
                ballRigidbody.linearVelocity = Vector3.zero; // Reset velocity
                ballRigidbody.angularVelocity = Vector3.zero; // Reset angular velocity
                ballRigidbody.AddForce(transform.right * launchForce, ForceMode.Impulse);
                ballRigidbody.gameObject.GetComponent<BallSM>().currentState=ballRigidbody.gameObject.GetComponent<MidAirState>();
            }
        }
    }
}