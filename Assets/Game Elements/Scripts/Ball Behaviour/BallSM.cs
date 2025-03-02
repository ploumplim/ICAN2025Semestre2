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
    [Header("Ball Height Settings")]
    [Tooltip("Maximum height the ball can achieve.")]
    public float maxHeight = 10f;

    [Tooltip("Minimum height the ball can achieve.")]
    public float minHeight = -1f;
    
    //-------------------------------------------------------------------------------------
    [Header("Ball physical properties")]
    [Tooltip("The linear damping value when the ball is grounded.")]
    public float groundedLinearDamping = 1f;
        
    [FormerlySerializedAs("midAirLinearDamping")] [Tooltip("The linear damping value when the ball is flying midair.")]
    public float flyingLinearDamping = 0.1f;
    
    [Tooltip("The mass of the ball while its grounded.")]
    public float groundedMass = 1f;
    
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
    [HideInInspector] public bool canBeParried = false;
    // ~~EVENTS~~
    
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

        // Call the SetMaxHeight method, which will keep the ball within the height limits
        SetMaxHeight();
        
        // Call the SetMaxSpeed method, which will keep the ball from going faster than the maxSpeed value
        SetMaxSpeed();
        
        // Call the FixVerticalSpeed method, which will keep the ball from going up when it reaches the maxHeight
        FixVerticalSpeed();
        
        // Check the current speed of the ball. If it's above minimumSpeedToGround, the state of the ball will change to FlyingState.
        if (rb.linearVelocity.magnitude > minimumSpeedToGround)
        {
            ChangeState(GetComponent<FlyingState>());
        }
        else
        {
            ChangeState(GetComponent<DroppedState>());
        }
    }

    public void FixVerticalSpeed()
    {
        if (transform.position.y >= maxHeight)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        }
    }
    public void SetMaxHeight()
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
        // Draw a red line in the forward direction of the ball
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * 10);
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
            default:
                break;
        }

        
    }

    
}
