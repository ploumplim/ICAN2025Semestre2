using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class BallSM : MonoBehaviour
{

    
    // ~~VARIABLES~~
    public BallState currentState;
    
    //-------------------------------------------------------------------------------------
    [Header("Ball Propulsion Settings")]
    [Tooltip("The ball will never go faster than this value.")]
    public float maxSpeed = 20f;
    
    //-------------------------------------------------------------------------------------
    [FormerlySerializedAs("maxHeight")]
    [Header("Ball Height Settings")]
    [Tooltip("Maximum height the ball can achieve while grounded.")]
    public float groundedMaxHeight = 10f;
    
    [Tooltip("Maximum height the ball can achieve while bunted.")]
    public float buntedMaxHeight = 20f;
    
    [Tooltip("Maximum height the ball can achieve while midair.")]
    public float flyingMaxHeight = 30f;
    
    
    
    [Tooltip("Minimum height the ball can achieve.")]
    public float minHeight = -1f;
    
    //-------------------------------------------------------------------------------------
    [Header("Ball physical properties")]
    [Tooltip("The linear damping value when the ball is grounded.")]
    public float groundedLinearDamping = 1f;
    
    [Tooltip("The linear damping value when the ball is bunted.")]
    public float buntedLinearDamping = 0.5f;
        
    [FormerlySerializedAs("midAirLinearDamping")] [Tooltip("The linear damping value when the ball is flying midair.")]
    public float flyingLinearDamping = 0.1f;
    
    [Tooltip("The mass of the ball while its grounded.")]
    public float groundedMass = 1f;
    
    [Tooltip("The mass of the ball while its bunted.")]
    public float buntedMass = 0.5f;
    
    [FormerlySerializedAs("midAirMass")] [Tooltip("The mass of the ball while its midair.")]
    public float flyingMass = 0.1f;
    
    //-------------------------------------------------------------------------------------
    [Header("Dropped Settings")]
    [Tooltip("The ball will become dropped if it reaches this minimum speed if grounded by speed is true.")]
    public float minimumSpeedToGround = 5f;
    
    
    
    //----------------------------COMPONENTS----------------------------
    [HideInInspector]public Rigidbody rb;
    
    //---------------------------PRIVATE VARIABLES---------------------------
    [HideInInspector]public int bounces = 0;
    
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~EVENTS~~~~~~~~~~~~~~~~~~~~~~~~~~
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        BallState[] states = GetComponents<BallState>();
        foreach (BallState state in states)
        {
            state.Initialize(this);
        }
        currentState = GetComponent<DroppedState>();
        
    }
    
    // ~~~~~~~~~~~~~~~~~~~~~~ CHANGE STATE ~~~~~~~~~~~~~~~~~~~~~~
    public void ChangeState(BallState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
        // Debug.Log("State changed to: " + newState);
    }
    
    //~~~~~~~~~~~~~~~~~~~~~~ UPDATE ~~~~~~~~~~~~~~~~~~~~~~

    // Update is called once per frame
    void FixedUpdate()
    {
        // Call the Tick method of the current state
        currentState.Tick();
        
        // Call the SetMaxSpeed method, which will keep the ball from going faster than the maxSpeed value
        SetMaxSpeed();
    }

    public void FixVerticalSpeed(float maxHeight)
    {
        if (transform.position.y >= maxHeight)
        {
            // When the ball reaches the maxHeight, set the vertical speed to 0.
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        }
    }
    public void SetMaxHeight(float maxHeight)
    {
        transform.position = new Vector3(transform.position.x, 
            Mathf.Clamp(transform.position.y, minHeight, maxHeight), transform.position.z);
    }
    public void SetMaxSpeed()
    {
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }
    

    //~~~~~~~~~~~~~~~~~~~~~~ DRAW GIZMOS ~~~~~~~~~~~~~~~~~~~~~~
    private void OnDrawGizmos()
    {
        // Draw a line that goes towards the ground from the ball. The color depends on the state.
        switch (currentState)
        {
            case FlyingState:
                Gizmos.color = Color.red;
                break;
            case DroppedState:
                Gizmos.color = Color.green;
                break;
            case BuntedBallState:
                Gizmos.color = Color.magenta;
                break;
            default:
                break;
        }
        Gizmos.DrawRay(transform.position, transform.up * -100);
    }
    
    //~~~~~~~~~~~~~~~~~~~~~~ COLLISIONS ~~~~~~~~~~~~~~~~~~~~~~
 
    private void OnCollisionEnter(Collision other)
    {
        
        switch (currentState)
        {
            case FlyingState:
                if (other.gameObject.CompareTag("Bouncer"))
                { 
                    bounces++;
                }
                break;
            case DroppedState:
                break;
            case BuntedBallState:
                if (other.gameObject.CompareTag("Floor"))
                {
                    ChangeState(GetComponent<DroppedState>());
                }
                break;
            default:
                break;
        }

        
    }

    
}
