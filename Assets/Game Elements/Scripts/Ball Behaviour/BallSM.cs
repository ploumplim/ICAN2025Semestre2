using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class BallSM : MonoBehaviour
{

    
    // ~~VARIABLES~~
    public BallState currentState;
    
    [Header("Ball Propulsion Settings")]
    [Tooltip("Horizontal force with which the ball will be thrown in its forward direction. This is multiplied by" +
             "the charge value.")]
    public float ballSpeed = 10f;
    
    [Tooltip("The ball will never go faster than this value.")]
    public float maxSpeed = 20f;
    
    [Tooltip("minimum force that the ball has to have.")]
    public float minimumForce = 5f;
    
    [Tooltip("Vertical force with which the ball will be thrown in its up direction.")]
    public float ballVSpeed = 1;
    
    [Header("Ball Height Settings")]
    [Tooltip("Maximum height the ball can achieve.")]
    public float maxHeight = 10f;

    [Tooltip("Minimum height the ball can achieve.")]
    public float minHeight = -1f;
    
    [Header("Ball physical properties")]
    [Tooltip("The linear damping value when the ball is grounded.")]
    public float groundedLinearDamping = 1f;
        
    [Tooltip("The linear damping value when the ball is flying midair.")]
    public float midAirLinearDamping = 0.1f;
    
    [Tooltip("The mass of the ball while its grounded.")]
    public float groundedMass = 1f;
    
    [Tooltip("The mass of the ball while its midair.")]
    public float midAirMass = 0.1f;
    
    [Header("Aim assist settings (currently disabled)")]
    
    [FormerlySerializedAs("detectionRadius")] [Tooltip("Base radius of the detection sphere.")]
    public float baseDetectionRadius = 5f;
    
    [Tooltip("Multiplier for the detection radius. Affected by the speed of the ball. This increases the size" +
             " of the detection sphere as the ball moves faster.")]
    public float detectionRadiusMultiplier = 0.1f;
    
    [Tooltip("The strength of the homing effect.")]
    public float homingForce = 10f;
    
    [Header("Bouncing Stats")]
    [Tooltip("Ammount of Bounces that the ball can have.")]
    public int maxBounces = 3;
    
    [Header("Grounded Settings")]
   [Tooltip("Toggle. Turn on to make the ball be grounded by its speed and not by the time.")]
    public bool groundedBySpeed = false;

    [Tooltip("The ball will become grounded after this time if grounded by speed is false. If it is true," +
             "this value represents how soon the ball will check its speed after being shot to become grounded.")]
    public float timeToGrounded = 2f;
    
    [Tooltip("The ball will become grounded if it reaches this minimum speed if grounded by speed is true.")]
    public float minimumSpeedToGround = 5f;
    
    
    //----------------------------COMPONENTS----------------------------
    [HideInInspector]public Rigidbody rb;
    [HideInInspector]public SphereCollider sc;
    [HideInInspector]public GameObject player;
    
    //---------------------------PRIVATE VARIABLES---------------------------
    [HideInInspector]public float speedModifiedDetectionRadius; // Detection radius modified by the speed of the ball
    [HideInInspector]public Vector3 minimumSpeed;
    [HideInInspector]public int bounces = 0;
    [HideInInspector] public bool canBeParried = false;
    // ~~EVENTS~~
    public UnityEvent canBeParriedEvent;
    public UnityEvent cannotBeParriedEvent;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sc = GetComponent<SphereCollider>();
        BallState[] states = GetComponents<BallState>();
        foreach (BallState state in states)
        {
            state.Initialize(this);
        }
        
        currentState = GetComponent<DroppedState>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Call the Tick method of the current state
        currentState.Tick();
        
        // Clamp the Y value of the ball to the minimum and maximum height
        transform.position = new Vector3(transform.position.x, 
            Mathf.Clamp(transform.position.y, minHeight, maxHeight), transform.position.z);

        speedModifiedDetectionRadius = baseDetectionRadius +
                                       (rb.linearVelocity.magnitude * detectionRadiusMultiplier);

        SetMaxSpeed();
    }
    
    public void SetMaxSpeed()
    {
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }
    
    // Change the current state of the ball
    public void ChangeState(BallState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
        // Debug.Log("State changed to: " + newState);
    }
    
    // Throw the ball towards its forward direction.
    
    public void Throw(float chargeMultiplier)
    {
        float finalSpeed = ballSpeed * chargeMultiplier;
        float finalVSpeed = ballVSpeed;

        minimumSpeed = player.GetComponent<Rigidbody>().linearVelocity * 2f + minimumForce * player.transform.forward;
        
        rb.AddForce(minimumSpeed + transform.forward * finalSpeed, ForceMode.Impulse);
        rb.AddForce(transform.up * finalVSpeed, ForceMode.Impulse);
        // Debug.Log(chargeMultiplier);
    }
    
    public void Bounce()
    {
        rb.AddForce(transform.forward * homingForce, ForceMode.Impulse);
        rb.AddForce(transform.up * ballVSpeed, ForceMode.Impulse);
    }
    
    private void OnDrawGizmos()
    {
        // Draw a red line in the forward direction of the ball
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * 10);
        Gizmos.DrawWireSphere(transform.position, speedModifiedDetectionRadius);
    }
 
    private void OnCollisionEnter(Collision other)
    {
        switch (currentState)
        {
            case MidAirState:
                switch (other.gameObject.tag)
                {
                    case "Floor":
                        ChangeState(GetComponent<DroppedState>());
                        break;
                    case "Bouncer":
                        if (bounces < maxBounces)
                        {
                            ChangeState(GetComponent<TargetingState>());
                        }
                        else
                        {
                            ChangeState(GetComponent<DroppedState>());
                        }
                        break;
                    default:
                        break;
                }
                break;
            case TargetingState:
                switch (other.gameObject.tag)
                {
                    case "Floor":
                        ChangeState(GetComponent<DroppedState>());
                        break;
                }
                break;
            case DroppedState:
                break;
            default:
                break;
        }

        
    }

    
}
