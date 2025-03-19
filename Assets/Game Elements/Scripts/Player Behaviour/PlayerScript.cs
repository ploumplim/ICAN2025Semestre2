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
    [Tooltip("The modifier when the hit is being charge.")]
    public float chargeSpeedModifier = 0.5f;
    [Tooltip("The modifier when the hit is released.")]
    public float releaseSpeedModifier = 1f;
    [Tooltip("The modifier when the player bunts.")]
    public float buntSpeedModifier = 0.5f;
    [Tooltip("The rate at which speed picks up when the input is being performed.")]
    public float acceleration = 0.1f;
    //---------------------------------------------------------------------------------------
    [Header("Rotation Lerps")]
    [Tooltip("Lerp time for the rotation while not aiming")]
    public float neutralLerpTime = 0.1f;
    [Tooltip("Lerp time for the rotation while charging a hit")]
    public float chargeLerpTime = 0.1f;
    
    //---------------------------------------------------------------------------------------
    [Header("Knockback")]
    [Tooltip("Time where the player loses control after being struck by the ball.")]
    public float knockbackTime = 0.5f;
    [Tooltip("This is the force multiplier applied to the player when hit by a ball.")]
    public float knockbackForce = 10f;
    [Tooltip("This modifier is applied only when the player is knock backed from another player's dash.")]
    public float dashKnockbackModifier = 0.5f;
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
    [Tooltip("The offset of the hit detection sphere.")]
    public float hitDetectionOffset = 0f;
    //---------------------------------------------------------------------------------------
    [Header("Slowmo Settings")]
    [Tooltip("The rate at which the ball slows down when it enters the player's is charging.")]
    public float slowRate = 0.5f;
    [Tooltip("The time the player has to wait between each hit.")]
    public float minimumSpeedPercentage = 0.25f;
    
    
    //---------------------------------------------------------------------------------------
    [Header("Bunt Settings")]
    [Tooltip("The force applied to the ball when bunting.")]
    public float buntForce = 10f;
    [FormerlySerializedAs("buntTime")] [Tooltip("The time the player has to wait between each bunt.")]
    public float buntCooldown = 0.5f;
    [Tooltip("The opportunity window that the bunt has to apply its effect on the ball.")]
    public float buntDuration = 0.5f;
    [Tooltip("The radius of the sphere that will detect the ball when bunting.")]
    public float buntSphereRadius;
    [Tooltip(("The position of the bunt sphere."))]
    public float buntSpherePositionOffset;
    //---------------------------------------------------------------------------------------
    [FormerlySerializedAs("rollSpeed")]
    [Header("Dash Settings")]
    [Tooltip("The dash speed.")]
    public float dashSpeed = 10f;
    [FormerlySerializedAs("rollDuration")] [Tooltip("The duration of the dash.")]
    public float dashDuration = 1f;
    [Tooltip("Cooldown between each dash.")]
    public float dashCooldown = 0.5f;
    [FormerlySerializedAs("dashFeedbackTrail")] [Tooltip("This is the radius of the sphere that will detect the ball when rolling.")]
    public float rollDetectionRadius = 5f;
    [Tooltip("This boolean determines if when dashing the character can pass through ledges.")]
    public bool canPassThroughLedges = false;
    [Tooltip("Force to apply to the ball when dashing into it.")]
    public float ballDashForce = 10f;

    //---------------------------------------------------------------------------------------
    [HideInInspector] public GameObject MultiplayerManager;
    
    [Header("Scene References")]
    public Camera playerCamera;

    public GameObject playerHand;
    
    [FormerlySerializedAs("PlayerPerformedHit")]
    [Header("Events")]
    // ------------------------------ EVENTS ------------------------------
    public UnityEvent OnHitButtonPressed;
    [FormerlySerializedAs("BallParried")]
    public UnityEvent<float> OnPlayerHitReleased;
    public UnityEvent<float> OnBallHitByPlayer;
    public UnityEvent OnPlayerHitByBall;
    [FormerlySerializedAs("PlayerPerformedBunt")]
    public UnityEvent OnPlayerPerformedBunt;
    public UnityEvent OnPlayerBuntBall;
    
    [FormerlySerializedAs("PlayerDashed")]
    public UnityEvent OnPlayerDash;
    public UnityEvent PlayerEndedDash;
    public UnityEvent OnPlayerDeath;
    
    
    // action events
    public event Action<int,GameObject,BallState> OnBallHit;
    
    // ------------------------------ PRIVATE VARIABLES ------------------------------
    
    private bool _isAiming;
    [HideInInspector] public PlayerInput playerInput;
    [HideInInspector] public InputAction moveAction;
    [HideInInspector] public InputAction throwAction;
    [HideInInspector] public InputAction rollAction;
    [HideInInspector] public InputAction BuntAction;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public CapsuleCollider col;
    [HideInInspector] public int ledgeLayer;
    [HideInInspector] public int playerLayer;
    [HideInInspector] public int ballLayer;
    
    // ------------------------------ BALL ------------------------------
    [HideInInspector] public BallSM ballSM;

    // ------------------------------ CHARGING ------------------------------
    [HideInInspector]public float chargeValueIncrementor = 0f;
    // ------------------------------ HIT ------------------------------
    [FormerlySerializedAs("parryTimer")] [HideInInspector] public float hitTimer = 0f;
    // ------------------------------ BUNT ------------------------------
    [HideInInspector] public float buntTimer = 0f;
    
    // ------------------------------ DASH ------------------------------
    // [HideInInspector]public bool ballCaughtWhileRolling;
    [HideInInspector] public float dashTimer = 0f;
    
    // ------------------------------ MOVE ------------------------------
    [FormerlySerializedAs("moveInput")] [HideInInspector] public Vector2 moveInputVector2;
    
    
    
    
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ METHODS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public void Start()
    {
        SetPlayerParameters();
        col = GetComponent<CapsuleCollider>();
    }
    
    public void SetPlayerParameters()
    {
        MultiplayerManager = GameObject.FindWithTag("MultiPlayerManager");
        playerCamera = MultiplayerManager.GetComponent<MultiplayerManager>().camera;
        
        
        
        rb = GetComponent<Rigidbody>(); 
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        throwAction = playerInput.actions["Attack"];
        rollAction = playerInput.actions["Roll"];
        BuntAction = playerInput.actions["Bunt"];
        
        ledgeLayer = LayerMask.NameToLayer("Ledge");
        playerLayer = gameObject.layer;
        ballLayer = LayerMask.NameToLayer("Ball");
        
        PlayerState[] states = GetComponents<PlayerState>();
        foreach (PlayerState state in states)
        {
            state.Initialize(this);
        }

        currentState = GetComponent<NeutralState>();
        
        dashTimer = dashCooldown;
    }

    // ------------------------------ FIXED UPDATE ------------------------------
    private void FixedUpdate()
    {
        currentState.Tick();
        moveInputVector2 = moveAction.ReadValue<Vector2>();

        // If the player is holding a ball, set the ball's position to the player's hand
        
        
        if (hitTimer > 0)
        {
            hitTimer -= Time.deltaTime;
        }
        
        
        if (dashTimer < dashCooldown)
        {
            dashTimer += Time.deltaTime;
        }
        else if (dashTimer >= dashCooldown)
        {
            dashTimer = dashCooldown;
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
        BallSM ballSM = other.gameObject.GetComponent<BallSM>();
        if (ballSM)
        {
            OnPlayerHitByBall?.Invoke();
            // Debug.Log(currentState);
            if (ballSM.currentState is FlyingState)
            {
                // Debug.Log("Ball hit player");
                if (currentState is not KnockbackState &&
                    currentState is not DashingState &&
                    currentState is not DeadState)
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
            else if (ballSM.currentState is LethalBallState)
            {
                // Debug.Log("Ball hit player");
                if (currentState is not KnockbackState &&
                    currentState is not DashingState &&
                    currentState is not DeadState)
                {
                    PlayerEndedDash?.Invoke();
                    ChangeState(GetComponent<DeadState>());
                    // Debug.Log("Player died");
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
            OnHitButtonPressed?.Invoke();
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
        if (currentState is NeutralState && context.started)
        {
            ChangeState(GetComponent<BuntingPlayerState>());
        }
    }
    
    // ------------------------------ DASH ------------------------------
    
    public void OnDash(InputAction.CallbackContext context)
    {
        if (currentState is NeutralState &&
            context.started && dashTimer >= dashCooldown)
        {
            if (moveInputVector2 != Vector2.zero)
            {
                // Debug.Log("Dashing!");
                dashTimer = 0;
                OnPlayerDash?.Invoke();
                ChangeState(GetComponent<DashingState>());
            }
        }
    }
    
    // ------------------------------ EVENTS ------------------------------
    public void OnBallHitEventMethod(GameObject ball)
    {
        OnBallHit?.Invoke(0,gameObject,ball.GetComponent<BallSM>().currentState);
    }

    // ------------------------------ PLAYER GIZMOS ------------------------------

    // Create a gizmo to show the direction the player is looking at
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * 10);
        // Draw the dash sphere
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rollDetectionRadius);
        
        // Draw the hit sphere
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * hitDetectionOffset, hitDetectionRadius);
        
        // Draw the bunt sphere
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position + transform.forward * buntSpherePositionOffset, buntSphereRadius);
    }
}