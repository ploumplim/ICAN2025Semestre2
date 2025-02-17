using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToOriginalPos : MonoBehaviour
{
    [Tooltip("Time to return to the original position.")]
    public float timeToReturn = 3f;
    public float forceMultiplier = 0.1f;
    public GameObject objectToReturn;
    [HideInInspector] public Vector3 originalPos;
    [HideInInspector] public Vector3 originalRot;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public bool wasMoved;
    void Start()
    {
        // Set the object to return to parentless.
        objectToReturn.transform.parent = null;
        originalPos = objectToReturn.transform.position;
        originalRot = objectToReturn.transform.eulerAngles;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // If this game object's position becomes different from the original position,
        // set wasMoved to true.
        if (transform.position != originalPos &&
            !wasMoved)
        {
            wasMoved = true;
            StartCoroutine(ReturnToOriginalPosCoroutine());
        }
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<BallSM>())
        {
            // Set the rigid body as non-kinematic.
            rb.isKinematic = false;
            
            // Make the object fly off in the opposite direction of the ball
            rb.AddForce(-other.gameObject.GetComponent<Rigidbody>().linearVelocity * forceMultiplier,
                ForceMode.Impulse);
        }
    }
    
    // Return to original position coroutine
    IEnumerator ReturnToOriginalPosCoroutine()
    {
        // Wait for the time to return.
        yield return new WaitForSeconds(timeToReturn);
        
        // Set the object to return to its original position.
        transform.position = originalPos;
        
        // Reset the character's transform rotations.
        transform.rotation = Quaternion.identity;
        
        // Set kinematic
        rb.isKinematic = true;
        
        // Set WasMoved to false.
        wasMoved = false;
    }
}
