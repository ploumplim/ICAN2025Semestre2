using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Serialization;

public class PlayerScript : MonoBehaviour
{
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

    #region Variables
    // ------------------------------ PUBLIC VARIABLES ------------------------------
    [Header("Movement Settings")]
    [Tooltip("Choose the player's movement type.")]
    public MoveType movementType = MoveType.Velocity;
    [Tooltip("The base player speed. The speed is dependant on the input.")]
    public float speed = 5f;
    [Tooltip("The speed modifier when the hit is being charged. If 0, the player doesn't move.")]
    public float chargeSpeedModifier = 0.5f;
    [Tooltip("The rate at which speed picks up when the input is being performed.")]
    public float acceleration = 0.1f;
    //---------------------------------------------------------------------------------------
    [Header("Rotation Lerps")]
    [Tooltip("Lerp time for the rotation while not aiming")]
    public float neutralLerpTime = 0.1f;
    [Tooltip("Lerp time for the rotation while charging a hit")]
    public float chargeLerpTime = 0.1f;
    //---------
    [Header("Dash Settings")]
    public float dashBurst;
    public float dashDuration;
    public float dashCooldown;
    
    //---------------------------------------------------------------------------------------
    [Header("Knockback")]
    [Tooltip("Time where the player loses control after being struck by the ball.")]
    public float knockbackTime = 0.5f;
    [Tooltip("This is the force multiplier applied to the player when hit by a ball.")]
    public float knockbackForce = 10f;
    [FormerlySerializedAs("linearDrag")] [Tooltip("The normal linear drag of the player.")]
    public float baseLinearDamping = 3f;
    [FormerlySerializedAs("hitLinearDrag")] [Tooltip("The linear drag when the player is hit by a ball.")]
    public float knockedBackLinearDamping = 0f;
    
    //---------------------------------------------------------------------------------------
    [Header("Hit parameters")]
    [Tooltip("Select the type of hit.")]
    public HitType hitType = HitType.ForwardHit;
    [Tooltip("The rate at which the charge value increases for a hit.")]
    public float chargeRate = 0.5f;
    [Tooltip("The duration that the hit has to apply force to the ball.")]
    public float releaseDuration = 0.5f;
    [Tooltip("How long the player can hold the charge before releasing automatically.")]
    public float chargeTimeLimit = 1f;
    [Tooltip("The speed multiplier on the ball when hit.")]
    public float hitForce = 10f;
    [Tooltip("This number is the minimum value that the charge reaches when tapped.")]
    public float chargeClamp = 0.5f;
    [Tooltip("The radius of the sphere that will detect the ball when hitting.")]
    public float hitDetectionRadius = 3.5f;
    [Tooltip("The offset of the hit detection sphere.")]
    public float hitDetectionOffset = 0f;
    [Tooltip("The window of opportunity to catch the ball at the start of the charge.")]
    public float catchWindow = 0.2f;

    // ----------------------------------------------------------------------------------------
    [Header("Game Objects")] public GameObject playerHand;

    //---------------------------------------------------------------------------------------
    [HideInInspector] public GameObject MultiplayerManager;
    
    [Header("Scene References")]
    public CameraScript playerCamera;
    
    [Header("Events")]
    // ------------------------------ EVENTS ------------------------------
    // unity events
    
    public UnityEvent OnHitButtonPressed;
    public UnityEvent<float> OnPlayerHitReleased;
    public UnityEvent<float> OnBallHitByPlayer;
    public UnityEvent OnPlayerHitByBall; 
    public UnityEvent OnPlayerDeath;
    public UnityEvent OnPlayerDash;
    public UnityEvent OnPlayerEndDash;
    
    // action events
    public event Action<int,GameObject> OnBallHit;
    
    // ------------------------------ PRIVATE VARIABLES ------------------------------
    
    private bool _isAiming;
    [HideInInspector] public PlayerState currentState;
    [HideInInspector] public PlayerInput playerInput;
    [HideInInspector] public InputAction moveAction;
    [HideInInspector] public InputAction throwAction;
    [HideInInspector] public InputAction dashAction;
    [HideInInspector] public InputAction reviveDebug;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public CapsuleCollider col;
    [HideInInspector] public int playerLayer;
    [HideInInspector] public int ballLayer;
    [HideInInspector] public int hazardLayer;
    [HideInInspector] public bool isReady;
    [HideInInspector] public GameObject playerScorePanel;

    // ------------------------------ CHARGING ------------------------------
    [HideInInspector]public float chargeValueIncrementor = 0f;
    // ------------------------------ HIT ------------------------------
    [HideInInspector] public float hitTimer = 0f;
    // ------------------------------ MOVE ------------------------------
    [HideInInspector] public Vector2 moveInputVector2;
    private float _dashTimer;
    
    // ------------------------------ INPUT BUFFERING ------------------------------
    private InputAction _bufferedAction;
    private float _bufferedActionTime;
    [SerializeField] [Tooltip("Time window for input buffering")]
    private float bufferDuration = 0.1f;
    
    #endregion
   
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ METHODS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public void Start()
    {
        SetPlayerParameters();
        col = GetComponent<CapsuleCollider>();
    
        // Subscribe to the "Pause" action with a lambda to pass the context to OnPause
        playerInput.actions["SetPause"].performed += context => 
            GameManager.Instance.levelManager.ingameGUIManager.UI_PauseMenu.OnPause(context);
    }
    
    public void SetPlayerParameters()
    {
        MultiplayerManager = GameObject.FindWithTag("MultiPlayerManager");
        playerCamera = MultiplayerManager.GetComponent<MultiplayerManager>().camera;
        
        rb = GetComponent<Rigidbody>(); 
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        throwAction = playerInput.actions["Attack"];
        reviveDebug = playerInput.actions["DebugRevive"];
        dashAction = playerInput.actions["Sprint"];
        
        playerLayer = gameObject.layer;
        ballLayer = LayerMask.NameToLayer("Ball");
        hazardLayer = LayerMask.NameToLayer("LevelHazard");
        
        PlayerState[] states = GetComponents<PlayerState>();
        foreach (PlayerState state in states)
        {
            state.Initialize(this); 
        }

        currentState = GetComponent<NeutralState>();
        
    }
    

    // ------------------------------ FIXED UPDATE ------------------------------
    private void FixedUpdate()
    {
        currentState.Tick();
        moveInputVector2 = moveAction.ReadValue<Vector2>();

        // Timers
        if (hitTimer > 0)
        {
            hitTimer -= Time.deltaTime;
        }
        
        if (_dashTimer > 0)
        {
            _dashTimer -= Time.deltaTime;
        }
        
    }
    

    // ------------------------------ STATE MANAGEMENT ------------------------------

    public void ChangeState(PlayerState newState)
    {
        //buffering inputs
        if (_bufferedAction != null && Time.time - _bufferedActionTime < bufferDuration)
        {
            switch (_bufferedAction.name)
            {
                case "Attack":
                    if (newState != GetComponent<ChargingState>() && newState != GetComponent<ReleaseState>())
                    {
                        if (throwAction.triggered)
                        {
                            newState = GetComponent<ChargingState>();
                        }
                        else
                        {
                            chargeValueIncrementor = chargeClamp;
                            newState = GetComponent<ReleaseState>();
                        }
                    }
                    
                    
                    if (throwAction.triggered)
                    {
                        newState = GetComponent<ChargingState>();
                    }
                    else
                    {
                        chargeValueIncrementor = chargeClamp;
                        newState = GetComponent<ReleaseState>();
                    }
                    
                    break;
                case "Sprint":
                        newState = GetComponent<DashingState>();
                    break;
            }
        }
        
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
        
        
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
                // If the ball is not lethal, push the player in the opposite direction of the ball
                Vector3 direction = transform.position - other.transform.position;
                // Debug.Log("Ball hit player");
                if (currentState is not KnockbackState &&
                    currentState is not DeadState)
                {
                    ChangeState(GetComponent<KnockbackState>());
                    // Push the player in the opposite direction of the ball
                    
                    rb.AddForce(
                        direction.normalized * knockbackForce,
                        ForceMode.Impulse);
                }
                
                // Set the ball's speed to currentBallSpeedVec3.
                ballSM.rb.linearVelocity = ballSM.currentBallSpeedVec3.magnitude * -direction.normalized;
            }
            else if (ballSM.currentState is LethalBallState)
            {
                if (currentState is not KnockbackState &&
                    currentState is not DeadState)
                {
                    ChangeState(GetComponent<DeadState>());
                }
            }
        }
        
        // If the object is on the collision layer "LevelHazard", change state to "deadstate"
        if (other.gameObject.layer == hazardLayer)
        {
            ChangeState(GetComponent<DeadState>());
        }
    }

    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ INPUTS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    
    // ------------------------------ INPUT BUFFERING ------------------------------

    private void BufferInput(InputAction action)
    {
        _bufferedAction = action;
        _bufferedActionTime = Time.time;
    }


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
    // ------------------------------ SPRINT ------------------------------
    public void OnDash(InputAction.CallbackContext context)
    {
        switch (currentState)
        {
            case NeutralState:
                if ((context.started || context.performed) &&
                    _dashTimer <= 0)
                {
                    // If the player is not moving, sprinting will not work
                    if (moveInputVector2 != Vector2.zero)
                    {
                        _dashTimer = dashCooldown;
                        ChangeState(GetComponent<DashingState>());
                    }
                }
                break;
            // case DashingState:
            //     if (context.canceled)
            //     {
            //         ChangeState(GetComponent<NeutralState>());
            //     }
            //     break;
            case ReleaseState:
                if ((context.started || context.performed) &&
                    _dashTimer <= 0)
                {
                    _dashTimer = dashCooldown;
                    BufferInput(context.action);
                }

                break;
        }
    }
    
    // ------------------------------ CHARGE ATTACK ------------------------------
    public void OnChargeAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        
        {
            if (currentState is NeutralState || currentState is DashingState)
            {
                GetComponent<DashingState>().timer = 0;
                OnHitButtonPressed?.Invoke();
                ChangeState(GetComponent<ChargingState>());
            }
            else if (currentState is not ChargingState && currentState is not ReleaseState)
            {
                GetComponent<DashingState>().timer = 0;
                BufferInput(context.action);
            }
        }
        
        else if (currentState is ChargingState && context.canceled) 
       
        { 
            ChangeState(GetComponent<ReleaseState>()); 
        }
        
    }
    
    
    // ------------------------------ ISREADY ------------------------------
    
    public void OnReady(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (GameManager.Instance.levelManager.currentState == GameManager.Instance.levelManager.GetComponent<OutOfLevelState>())
            {
                isReady = !isReady;
            }
            GameManager.Instance.multiplayerManager.WaitForPlayersReady();
        }
    }
    // ------------------------------ DEBUG ------------------------------

    public void OnDebugRevive(InputAction.CallbackContext context)
    {
        ChangeState(GetComponent<NeutralState>());
    }

    // ------------------------------ EVENT METHODS ------------------------------

    // ------------------------------ PLAYER GIZMOS ------------------------------

    // Create a gizmo to show the direction the player is looking at
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * 10);
        
        // Draw the hit sphere
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * hitDetectionOffset, hitDetectionRadius);
        
    }
}