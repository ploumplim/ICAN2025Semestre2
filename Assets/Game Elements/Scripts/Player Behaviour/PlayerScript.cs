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

    public enum HitType
    {
        ForwardHit,
        ReflectiveHit
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
    [Tooltip("The rate at which speed picks up when the input is being performed.")]
    public float acceleration = 0.1f;
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
    [FormerlySerializedAs("parryType")]
    [Header("Hit parameters")]
    [Tooltip("Select the type of hit.")]
    public HitType hitType = HitType.ForwardHit;
    [Tooltip("The rate at which the charge value increases for a hit.")]
    public float chargeRate = 0.5f;
    [FormerlySerializedAs("parryCooldown")] [Tooltip("The duration that the hit has to apply force to the ball.")]
    public float releaseDuration = 0.5f;
    [FormerlySerializedAs("parryForce")] [Tooltip("The speed multiplier on the ball when hit.")]
    public float hitForce = 10f;
    [Tooltip("This number is the minimum value that the charge reaches when tapped.")]
    public float chargeClamp = 0.5f;
    [Tooltip("This value (between 0 and 1) grants direction in the vertical axis to the player's hit. This is only" +
             "applied when the ball is grounded.")]
    public float verticalPercent = 0.2f;
    [FormerlySerializedAs("parryDetectionRadius")] [Tooltip("The radius of the sphere that will detect the ball when hitting.")]
    public float hitDetectionRadius = 3.5f;
    //---------------------------------------------------------------------------------------
    [Header("Bunt Settings")]
    [Tooltip("The force applied to the ball when bunting.")]
    public float buntForce = 10f;

    [FormerlySerializedAs("buntTime")] [Tooltip("The time the player has to wait between each bunt.")]
    public float buntCooldown = 0.5f;
    
    
    [Tooltip("The radius of the sphere that will detect the ball when bunting.")]
    public float buntSphereRadius;

    [Tooltip(("The position of the bunt sphere."))]
    public float buntSpherePositionOffset;
    
    //---------------------------------------------------------------------------------------
    [Header("Roll Settings")]
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
    
    [FormerlySerializedAs("PlayerParried")]
    [Header("Events")]
    // ------------------------------ EVENTS ------------------------------
    [FormerlySerializedAs("BallParried")] public UnityEvent PlayerPerformedHit;
    public UnityEvent PlayerDashed;
    public UnityEvent PlayerEndedDash;
    
    // ------------------------------ PRIVATE VARIABLES ------------------------------
    
    private bool _isAiming;
    [HideInInspector] public PlayerInput playerInput;
    [HideInInspector] public InputAction moveAction;
    [HideInInspector] public InputAction throwAction;
    [HideInInspector] public InputAction rollAction;
    [HideInInspector] public InputAction BuntAction;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public int ledgeLayer;
    [HideInInspector] public int playerLayer;
    
    // ------------------------------ BALL ------------------------------
    [HideInInspector] public BallSM ballSM;

    // ------------------------------ CHARGING ------------------------------
    [HideInInspector]public float chargeValueIncrementor = 0f;
    // ------------------------------ HIT ------------------------------
    [FormerlySerializedAs("parryTimer")] [HideInInspector] public float hitTimer = 0f;
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
        GetComponent<PlayerVisuals>().hitTimerVisuals = MultiplayerManager.GetComponent<MultiplayerManager>().HitTimeVisual;
        GetComponent<PlayerVisuals>().chargeVisuals = MultiplayerManager.GetComponent<MultiplayerManager>().ChargeVisualObject;
        
        
        
        rb = GetComponent<Rigidbody>(); 
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        throwAction = playerInput.actions["Attack"];
        rollAction = playerInput.actions["Roll"];
        BuntAction = playerInput.actions["Bunt"];
        
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
        
        
        if (hitTimer > 0)
        {
            hitTimer -= Time.deltaTime;
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
                    rb.AddForce(moveDirection * Mathf.Lerp(0, moveSpeed,acceleration), ForceMode.VelocityChange);
                    break;
                case MoveType.Velocity:
                    rb.linearVelocity = new Vector3(moveDirection.x * Mathf.Lerp(0, moveSpeed,acceleration),
                        rb.linearVelocity.y,
                        moveDirection.z * Mathf.Lerp(0, moveSpeed,acceleration));
                    break;
            }

            if (moveDirection != Vector3.zero)
            {
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
    // ------------------------------ BUNT ------------------------------
    public void OnBunt(InputAction.CallbackContext context)
    {
        // Check the player state.
        // Then, create an overlap sphere to detect the ball.
        // If the ball is detected, apply a force to the ball using the bunt parameters.
        if (currentState is NeutralState && context.started)
        {
            ChangeState(GetComponent<BuntingPlayerState>());
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
        
        // draw the hit sphere
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hitDetectionRadius);
        
        // Draw the bunt sphere
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position + transform.forward * buntSpherePositionOffset, buntSphereRadius);
    }
}