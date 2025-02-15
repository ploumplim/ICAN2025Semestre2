using UnityEngine;
using UnityEngine.Serialization;

public class BallSM : MonoBehaviour
{
    public BallState currentState;
    
    [Header("Ball Stats")]
    [Tooltip("Horizontal force with which the ball will be thrown in its forward direction.")]
    public float ballSpeed = 10f;

    [Tooltip("Vertical force with which the ball will be thrown in its up direction.")]
    public float ballVSpeed = 1;
    
    [FormerlySerializedAs("detectionRadius")] [Tooltip("Base radius of the detection sphere.")]
    public float baseDetectionRadius = 5f;
    
    [Tooltip("Multiplier for the detection radius. Affected by the speed of the ball. This increases the size" +
             " of the detection sphere as the ball moves faster.")]
    public float detectionRadiusMultiplier = 0.1f;
    
    [Tooltip("The strength of the homing effect.")]
    public float homingForce = 10f;
    
    
    [Tooltip("Maximum height the ball can achieve.")]
    public float maxHeight = 10f;

    [Tooltip("Minimum height the ball can achieve.")]
    public float minHeight = -1f;

    //----------------------------COMPONENTS----------------------------
    [HideInInspector]public Rigidbody rb;
    [HideInInspector]public SphereCollider sc;
    [HideInInspector] public GameObject player;
    
    //---------------------------PRIVATE VARIABLES---------------------------
    [HideInInspector]public float speedModifiedDetectionRadius; // Detection radius modified by the speed of the ball

    
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
        float finalVSpeed = ballVSpeed * chargeMultiplier;
        rb.AddForce(transform.forward * finalSpeed, ForceMode.Impulse);
        rb.AddForce(transform.up * finalVSpeed, ForceMode.Impulse);
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
                        ChangeState(GetComponent<TargetingState>());
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
