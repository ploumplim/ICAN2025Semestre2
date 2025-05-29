using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class BallSM : MonoBehaviour
{

    public enum GrowthType
    {
        OnHit,
        OnBounce
    }
    
    // ~~VARIABLES~~
    public BallState currentState;
    
    //-------------------------------------------------------------------------------------
    [Header("Ball Propulsion Settings")]
    [Tooltip("The ball will never go faster than this value.")]
    public float maxSpeed = 20f;
    public float minSpeed = 10f;
    [Tooltip("The ball becomes lethal when it reaches this speed.")]
    public float lethalSpeed = 10f;
    public float firstTimeLethalWaitTime = 0.1f;
    public float hitFreezeTimeMultiplier = 0.01f;
    //-------------------------------------------------------------------------------------
    [FormerlySerializedAs("maxHeight")]
    [Header("Ball Height Settings")]
    [Tooltip("Maximum height the ball can achieve while grounded.")]
    public float groundedMaxHeight = 10f;
    [Tooltip("Maximum height the ball can achieve while midair.")]
    public float flyingMaxHeight = 30f;
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
    [Header("Ball Size Settings")]
    [Tooltip("The ball's normal size.")]
    public float neutralScale = 1f;
    [Tooltip("The ball's growth rate.")]
    public float growthRate = 0.1f;
    [Tooltip("The ball's shrink rate.")]
    public float shrinkRate = 0.1f;
    [Tooltip("The ball's minimum scale.")]
    public float minimumScale = 0.5f;
    [Tooltip("The ball's maximum scale.")]
    public float maximumScale = 3.5f;
    [Tooltip("The ball's growth type.")]
    public GrowthType growthType = GrowthType.OnHit;
    //-------------------------------------------------------------------------------------
    [Header("Charging ball Settings")] 
    [Tooltip("Rate of movement towards the player's hand.")]
    public AnimationCurve movementCurve;
    public float ballMoveDuration = 0.5f;
    
    //-------------------------------------------------------------------------------------
    [Header("Hit State Settings")]
    [Tooltip("How long the ball remains in the hit state.")]
    public float hitStateDuration = 0.2f;
    
    // -------------------------------------------------------------------------------------
    [Header("Player contact Settings")]
    [Tooltip("The time the player is immune to the ball after hitting it.")]
    public float playerImmunityTime = 0.1f;
    
    
    //----------------------------COMPONENTS----------------------------
    [HideInInspector]public Rigidbody rb;
    
    //---------------------------PRIVATE or HIDDEN VARIABLES---------------------------
    [HideInInspector]public int bounces = 0;
    public GameObject ballOwnerPlayer;
    [HideInInspector]public SphereCollider col;
    [HideInInspector]public int pointWallPoints;
    [HideInInspector]public int playerColliderLayer;
    [HideInInspector]public int ballColliderLayer;
    [HideInInspector]public Vector3 currentBallSpeedVec3;
    [HideInInspector]public float ballSpeedFloor;
    [HideInInspector] public bool onLethal; 
    
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~EVENTS~~~~~~~~~~~~~~~~~~~~~~~~~~
    
    public UnityEvent<int> pointWallHit;
    public UnityEvent<int> OnPointBounce;
    public UnityEvent<int> OnNeutralBounce;
    public UnityEvent<float> OnBallFlight;
    public UnityEvent OnBallCaught;
    [FormerlySerializedAs("OnPerfectHit")] public UnityEvent OnHit;
    public UnityEvent OnBallLethal;
    public UnityEvent OnHitStateStart;
    public UnityEvent CaughtStateEnded;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        onLethal = false;
        col = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        playerColliderLayer = LayerMask.NameToLayer("Player");
        ballColliderLayer = LayerMask.NameToLayer("Ball");
        BallState[] states = GetComponents<BallState>();
        foreach (BallState state in states)
        {
            state.Initialize(this);
        }
        currentState = GetComponent<DroppedState>();
        Physics.IgnoreLayerCollision(ballColliderLayer, playerColliderLayer, true);
        ballSpeedFloor = minSpeed;


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

        if (currentState is DroppedState)
        {
            return;
        }
        
        SetMaxSpeed();
    }

    public void FixVerticalSpeed()
    {
        if (rb.linearVelocity.y > 0)
        {
            // When the ball reaches the maxHeight, set the vertical speed to 0.
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        }
    }
    public void SetMaxHeight(float miniHeight, float maxHeight)
    {
        transform.position = new Vector3(transform.position.x, 
            Mathf.Clamp(transform.position.y, miniHeight, maxHeight), transform.position.z);
    }
    public void SetMaxSpeed()
    {
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }
    //~~~~~~~~~~~~~~~~~~~~~~ BALL SPEED MANAGERS ~~~~~~~~~~~~~~~~~~~~~~
    public void SetBallSpeedMinimum(float currentSpeed, Vector3 ballDirection)
    {
        switch (currentSpeed)
        {
            case > 0f when currentSpeed < ballSpeedFloor:
                rb.linearVelocity = ballDirection * ballSpeedFloor;
                break;
            
            case > 0f when currentSpeed > ballSpeedFloor:
                ballSpeedFloor = currentSpeed;
                break;
        }
    }
    
    //~~~~~~~~~~~~~~~~~~~~~~ GROWTH ~~~~~~~~~~~~~~~~~~~~~~

    public void GrowBall()
    {
        transform.localScale += new Vector3(growthRate, growthRate, growthRate);
        if (transform.localScale.x > maximumScale)
        {
            transform.localScale = new Vector3(maximumScale, maximumScale, maximumScale);
        }
    }
    
    public void ShrinkBall()
    {
        transform.localScale -= new Vector3(shrinkRate, shrinkRate, shrinkRate);
        if (transform.localScale.x < minimumScale)
        {
            transform.localScale = new Vector3(minimumScale, minimumScale, minimumScale);
        }
    }
    
    public void ResetBallSize()
    {
        transform.localScale = new Vector3(neutralScale, neutralScale, neutralScale);
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
            // case BuntedBallState:
            //     Gizmos.color = Color.magenta;
            //     break;
            default:
                break;
        }
        Gizmos.DrawRay(transform.position, transform.up * -100);
    }
    
    //~~~~~~~~~~~~~~~~~~~~~~ COLLISIONS ~~~~~~~~~~~~~~~~~~~~~~
 
    private void OnCollisionEnter(Collision other)
    {
        
        SetBallSpeedMinimum(rb.linearVelocity.magnitude, rb.linearVelocity.normalized);

        if (other.gameObject.CompareTag("NeutralWall"))
        {
            pointWallHit?.Invoke(pointWallPoints);
            GameManager.Instance.levelManager.gameCameraScript.screenShakeGO.GetComponent<ScreenShake>().StartLitleScreenShake(rb.linearVelocity.magnitude);
            OnPointBounce?.Invoke(bounces);
        }
        
        switch (currentState)
        {
            case FlyingState:
                bounces++;
                // Check the ball GrowthType. If it's OnBounce, grow the ball.
                if (growthType == GrowthType.OnBounce && !other.gameObject.CompareTag("Player"))
                {
                    GrowBall();
                }
                
                
                
                if (other.gameObject.CompareTag("NeutralWall"))
                {
                    OnNeutralBounce?.Invoke(bounces);
                }

                break;
            case DroppedState:
                break;
            case LethalBallState:
                bounces++;
                if (growthType == GrowthType.OnBounce && !other.gameObject.CompareTag("Player")
                    && !other.gameObject.CompareTag("NonGrowerSurface"))
                {
                    GrowBall();
                }

                if (other.gameObject.CompareTag("PointWall"))
                {
                    pointWallHit?.Invoke(pointWallPoints);
                }
                
                if (other.gameObject.CompareTag("NeutralWall"))
                {
                    OnNeutralBounce?.Invoke(bounces);
                }
                
                if (other.gameObject.CompareTag("Bouncer"))
                {
                    OnNeutralBounce?.Invoke(bounces);
                }
                break;
            
            default:
                break;
        }

        
    }

    
}
