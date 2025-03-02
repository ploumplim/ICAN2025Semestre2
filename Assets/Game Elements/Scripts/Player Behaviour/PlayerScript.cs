using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerScript : MonoBehaviour
{

    
    // ------------------------------ PUBLIC VARIABLES ------------------------------
    public enum MoveType
    {
        Velocity,
        Force
    };

    public enum ParryType
    {
        ForwardParry,
        ReflectiveParry
    }
    
    [HideInInspector] public PlayerState currentState;
    
    [Header("MOVEMENT TYPES: \n " +
        "Velocity: The player's movement is controlled \n" +
        " by changing the velocity of the rigidbody.\n" +
        "Force: The player's movement is controlled \n" +
        " by applying a force to the rigidbody. \n" +
        " This means that all movement variables \n " +
        "should be decreased to avoid the player moving too fast.")]
    [Tooltip("Choose the player's movement type.")]
    public MoveType movementType = MoveType.Velocity;
    [Header("Movement variables")]
    [Tooltip("The player's speed when he has balls.")]
    public float speed = 5f;
    [Tooltip("The speed at which the player moves when charging their hit.")]
    public float chargeSpeedModifier = 0.5f;
    //---------------------------------------------------------------------------------------
    [Header("Rotation Lerps")]
    [Tooltip("Lerp time for the rotation while not aiming")]
    public float neutralLerpTime = 0.1f;
    [Tooltip("Lerp time for the rotation while rolling")]
    public float rollLerpTime = 0.1f;
    [Tooltip("Lerp time for the rotation while charging a hit")]
    public float chargeLerpTime = 0.1f;
    
    //---------------------------------------------------------------------------------------
    [Header("Knockback")]
    [Tooltip("Time where the player loses control after being struck by the ball.")]
    public float knockbackTime = 0.5f;
    [Tooltip("This is the force multiplier applied to the player when hit by a ball.")]
    public float knockbackForce = 10f;
    [Tooltip("The normal linear drag of the player.")]
    public float linearDrag = 3f;
    [Tooltip("The linear drag when the player is hit by a ball.")]
    public float hitLinearDrag = 0f;
    
    //---------------------------------------------------------------------------------------
    [Header("Hit parameters")]
    [Tooltip("Select the type of parry.")]
    public ParryType parryType = ParryType.ForwardParry;
    [Tooltip("The rate at which the charge value increases.")]
    public float chargeRate = 0.5f;
    [Tooltip("The time the player has to wait between each parry.")]
    public float parryCooldown = 0.5f;
    [Tooltip("The speed multiplier on the ball. .")]
    public float parryForce = 10f;
    [Tooltip("This number is the minimum value that the charge reaches when tapped.")]
    public float chargeClamp = 0.5f;
    [Tooltip("This value (between 0 and 1) grants direction in the vertical axis to the player's parry. This is only" +
             "applied when the ball is grounded.")]
    public float verticalPercent = 0.2f;
    [Tooltip("The window of opportunity that the parry will hit the ball.")]
    public float parryWindow = 0.4f;
    [Tooltip("The radius of the sphere that will detect the ball when parrying.")]
    public float parryDetectionRadius = 3.5f;
    //---------------------------------------------------------------------------------------
    [Header("Roll")]
    [Tooltip("The initial speed of the roll.")]
    public float rollSpeed = 10f;
    [Tooltip("The duration of the roll.")]
    public float rollDuration = 1f;
    [Tooltip("This is the radius of the sphere that will detect the ball when rolling.")]
    public float rollDetectionRadius = 5f;
    [Tooltip("This boolean determines if when dashing the character can pass through ledges.")]
    public bool canPassThroughLedges = false;
    //---------------------------------------------------------------------------------------
    [HideInInspector] public GameObject MultiplayerManager;
    
    
    // [Tooltip("The time the player has to wait between each roll.")]
    // public float rollCooldown = 0.5f;
    // [Tooltip("The speed that the player has to have at the end of the roll, if they dont catch" +
    //          "the ball while rolling.")]
    // public float rollEndSpeed = 5f;
    
    [Header("Scene References")]
    public Camera playerCamera;

    public GameObject playerHand;
    
    [Header("Events")]
    // ------------------------------ EVENTS ------------------------------
    public UnityEvent CanParryTheBallEvent;
    public UnityEvent CannotParryTheBallEvent;
    [FormerlySerializedAs("BallParried")] public UnityEvent PlayerParried;
    public UnityEvent PlayerDashed;
    public UnityEvent PlayerEndedDash;
    
    // ------------------------------ PRIVATE VARIABLES ------------------------------
    
    private bool _isAiming;
    [HideInInspector] public PlayerInput playerInput;
    [HideInInspector] public InputAction moveAction;
    [HideInInspector] public InputAction throwAction;
    [HideInInspector] public InputAction rollAction;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public int ledgeLayer;
    [HideInInspector] public int playerLayer;
    
    // ------------------------------ BALL ------------------------------
    [HideInInspector] public GameObject heldBall;
    [HideInInspector] public BallSM ballSM;

    // ------------------------------ CHARGING ------------------------------
    [HideInInspector]public float chargeValueIncrementor = 0f;
    // ------------------------------ PARRY ------------------------------
    [HideInInspector] public float parryTimer = 0f;
    // ------------------------------ ROLL ------------------------------
    // [HideInInspector]public bool ballCaughtWhileRolling;
    
    // ------------------------------ MOVE ------------------------------
    [FormerlySerializedAs("moveInput")] [HideInInspector] public Vector2 moveInputVector2;
    
    
    
    
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ METHODS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public void Start()
    {
        SetPlayerParameters();
    }
    
    public void SetPlayerParameters()
    {
        MultiplayerManager = GameObject.FindWithTag("MultiPlayerManager");
        playerCamera = MultiplayerManager.GetComponent<MultiplayerManager>().camera;
        GetComponent<PlayerVisuals>().parryTimerVisuals = MultiplayerManager.GetComponent<MultiplayerManager>().ParryTimeVisual;
        GetComponent<PlayerVisuals>().chargeVisuals = MultiplayerManager.GetComponent<MultiplayerManager>().ChargeVisualObject;
        
        
        
        rb = GetComponent<Rigidbody>(); 
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        throwAction = playerInput.actions["Attack"];
        rollAction = playerInput.actions["Roll"];
        
        ledgeLayer = LayerMask.NameToLayer("Ledge");
        playerLayer = gameObject.layer;
        
        PlayerState[] states = GetComponents<PlayerState>();
        foreach (PlayerState state in states)
        {
            state.Initialize(this);
        }

        currentState = GetComponent<NeutralState>();
    }

    private void FixedUpdate()
    {
        currentState.Tick();
        moveInputVector2 = moveAction.ReadValue<Vector2>();

        // If the player is holding a ball, set the ball's position to the player's hand
        if (heldBall)
        {
            heldBall.transform.position = playerHand.transform.position;
            if (!ballSM)
            {
                ballSM = heldBall.GetComponent<BallSM>();
            }
        }
        
        
        if (parryTimer > 0)
        {
            parryTimer -= Time.deltaTime;
        }
        
    }
    

    // ------------------------------ STATE MANAGEMENT ------------------------------

    public void ChangeState(PlayerState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
        // Debug.Log("State changed to: " + newState);
    }

    // ------------------------------ COLLISIONS ------------------------------

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<BallSM>())
        {
            // Debug.Log(currentState);
            if (other.gameObject.GetComponent<BallSM>().currentState==other.gameObject.GetComponent<FlyingState>())
            {
                if (currentState is not KnockbackState)
                {
                    PlayerEndedDash?.Invoke();
                    ChangeState(GetComponent<KnockbackState>());
                    // Push the player in the opposite direction of the ball
                    Vector3 direction = transform.position - other.transform.position;
                    rb.AddForce(
                        direction.normalized * other.gameObject.GetComponent<Rigidbody>().linearVelocity.magnitude * knockbackForce,
                        ForceMode.Impulse);
                    // Set ball to dropped state
                    other.gameObject.GetComponent<BallSM>().ChangeState(other.gameObject.GetComponent<DroppedState>());
                    // Apply an opposite force to the ball
                    other.gameObject.GetComponent<Rigidbody>().AddForce(
                        -direction.normalized * other.gameObject.GetComponent<Rigidbody>().linearVelocity.magnitude * knockbackForce,
                        ForceMode.Impulse);
                }
                
            }

        }
    }

    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ INPUTS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // ------------------------------ MOVE ------------------------------

    public void Move(float moveSpeed, float lerpMoveSpeed)
    {
        // Apply movement
        if (moveInputVector2 != Vector2.zero)
        {
            // Get the camera's forward and right vectors
            Vector3 cameraForward = playerCamera.transform.forward;
            Vector3 cameraRight = playerCamera.transform.right;

            // Flatten the vectors to the ground plane and normalize
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            // Calculate the movement direction
            Vector3 moveDirection = (cameraForward * moveInputVector2.y + cameraRight * moveInputVector2.x);

            // Apply movement and set the player's direction
            switch (movementType)
            {
                case MoveType.Force:
                    rb.AddForce(moveDirection * moveSpeed, ForceMode.VelocityChange);
                    break;
                case MoveType.Velocity:
                    rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed,
                        rb.linearVelocity.y,
                        moveDirection.z * moveSpeed);
                    break;
            }
            transform.forward = Vector3.Slerp(transform.forward, moveDirection, lerpMoveSpeed);
        }
    }
    
    // ------------------------------ CHARGE ATTACK ------------------------------
    public void OnChargeAttack(InputAction.CallbackContext context)
    {
        if (currentState is NeutralState && context.started)
        {
            ChangeState(GetComponent<ChargingState>());
        }
        
        // if (currentState is ChargingState && context.performed)
        // { 
        //     // Debug.Log("charging! charge: " + chargeValueIncrementor);
        // }
        else if (currentState is ChargingState && context.canceled) 
        { 
            ChangeState(GetComponent<ReleaseState>());
            // Debug.Log("released! charge: " + chargeValueIncrementor);
        }
        
        
    }
    // ------------------------------ ROLL ------------------------------
    
    public void OnRoll(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (currentState is NeutralState && moveInputVector2 != Vector2.zero)
            {
                PlayerDashed?.Invoke();
                ChangeState(GetComponent<RollingState>());
            }
        }
    }

    // ------------------------------ PLAYER GIZMOS ------------------------------

    // Create a gizmo to show the direction the player is looking at
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 10);
        
        // draw the parry sphere
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * parryDetectionRadius, parryDetectionRadius);
        
        
    }
}