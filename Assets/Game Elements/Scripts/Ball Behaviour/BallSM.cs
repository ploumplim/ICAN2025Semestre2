using UnityEngine;

public class BallSM : MonoBehaviour
{
    public BallState currentState;
    
    [Header("Ball Stats")]
    [Tooltip("Horizontal force with which the ball will be thrown in its forward direction.")]
    public float ballSpeed = 10f;

    [Tooltip("Vertical force with which the ball will be thrown in its up direction.")]
    public float ballVSpeed = 1;
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
        Debug.Log("State changed to: " + newState);
    }
    
    // Throw the ball towards its forward direction.
    
    public void Throw()
    {
        ChangeState(GetComponent<MidAirState>());
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * ballSpeed, ForceMode.Impulse);
        rb.AddForce(transform.up * ballVSpeed, ForceMode.Impulse);
        
    }
    
    // Draw a gizmo to show the direction of the ball
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * 10);
    }
 
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            ChangeState(GetComponent<DroppedState>());
        }
    }
}
