using System;
using UnityEngine;
using UnityEngine.Serialization;

public class InitialPushFunction : MonoBehaviour
{
    [SerializeField] private float pushForce = 10f; // The force to be applied to the object

    [SerializeField] private float ballSpeedFloor = 0; // The minimum speed of the ball

    void Start()
    {
        GetComponent<Rigidbody>().AddForce(Vector3.forward * pushForce, ForceMode.Impulse);
    }

    void FixedUpdate()
    {
        // Get the current speed of the object
        float currentSpeed = GetComponent<Rigidbody>().linearVelocity.magnitude;

        // If the current speed is less than the minimum speed, set it to the minimum speed
        if (currentSpeed < ballSpeedFloor)
        {
            Vector3 ballDirection = GetComponent<Rigidbody>().linearVelocity.normalized;
            GetComponent<Rigidbody>().linearVelocity = ballDirection * ballSpeedFloor;
        }
        
        // If the current speed is greater than the minimum speed, set the minimum speed to the current speed
        else if (currentSpeed > ballSpeedFloor)
        {
            ballSpeedFloor = currentSpeed;
        }
    }
}
