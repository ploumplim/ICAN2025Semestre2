using UnityEngine;

public class BallSM : MonoBehaviour
{
    public BallState currentState;
    
    [Header("Ball Stats")]
    [Tooltip("Horizontal force with which the ball will be thrown in its forward direction.")]
    public float ballSpeed = 10f;

    [Tooltip("Vertical force with which the ball will be thrown in its up direction.")]
    public float ballVSpeed = 1;
    
    [Tooltip("Radius of the detection sphere.")]
    [HideInInspector]public float detectionRadius = 5f;
    
    [Tooltip("Multiplier for the detection radius. Affected by the speed of the ball.")]
    public float detectionRadiusMultiplier = 0.1f;
    
    [Tooltip("The strength of the homing effect.")]
    public float homingForce = 10f;
    
    [Tooltip("Time limit the ball has to find a homing target.")]
    public float targetingTime = 0.25f;

    //----------------------------COMPONENTS----------------------------
    [HideInInspector]public Rigidbody rb;
    [HideInInspector]public SphereCollider sc;
    [HideInInspector] public GameObject player;
    
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
        currentState.Tick();
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
    
    public void Throw()
    {
        rb.AddForce(transform.forward * ballSpeed, ForceMode.Impulse);
        rb.AddForce(transform.up * ballVSpeed, ForceMode.Impulse);
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
        
        if (currentState is TargetingState)
        {
            // If the ball is in the targeting state, draw a sphere and a cone to show the detection area.
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
        
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
