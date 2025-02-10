using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallBehaviour : MonoBehaviour
{
    public Vector3 direction;
    public bool bounceMoment;
    public float bounceMomentTime;
    public float bounceMomentDuration;
    public float detectionSphereRadius;
    
    public Rigidbody rb;
    


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Set the direction of the ball equal to its velocity.
        direction = rb.velocity.normalized;
        
        // If the ball is moving, rotate it to face the direction it's moving.
        if (rb.velocity.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
        
        if (bounceMoment)
        {
            Debug.Log("searching");
            DetectTargets();
            bounceMomentTime += Time.deltaTime;
            if (bounceMomentTime >= bounceMomentDuration)
            {
                bounceMoment = false;
                bounceMomentTime = 0;
            }
        }
    }

    private void DetectTargets()
    {
        // Create a detection sphere around the ball.
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionSphereRadius);
        
        // Remove all colliders from my array that are not in the 'Target' tag.
        colliders = Array.FindAll(colliders, collider => collider.CompareTag("Target"));
        
        
        // sort the colliders by distance from the ball
        Array.Sort(colliders, (collider1, collider2) =>
        {
            float distance1 = Vector3.Distance(transform.position, collider1.transform.position);
            float distance2 = Vector3.Distance(transform.position, collider2.transform.position);
            return distance1.CompareTo(distance2);
        });
        
        
        // Set the ball's movement direction towards the closest target.
        if (colliders.Length > 0)
        {
            bounceMoment = false;
            bounceMomentTime = 0;
            Vector3 targetDirection = colliders[0].transform.position - transform.position;
            rb.velocity = targetDirection.normalized * rb.velocity.magnitude;
        }
        
    }
    // Draw the detection sphere in the Scene view.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionSphereRadius);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("BounceWall"))
        {
            bounceMoment = true;
        }
    }
    
}
