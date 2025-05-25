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
    [FormerlySerializedAs("playerGoalToAttack1")] [FormerlySerializedAs("playerGoalToDefend")] [Header("Player Goal Settings")]
    public GameObject playerGoalToAttack;
    [FormerlySerializedAs("playerGoalToAttack")] public GameObject playerGoalToDefend;
    public int playerPoint;
    
    //---------------------------------------------------------------------------------------
    [Header("Rotation Lerps")]
    [Tooltip("Lerp time for the rotation while not aiming")]
    public float neutralLerpTime = 0.1f;

    //---------
    [Header("Sprint Settings")]

    public float sprintMaxInitialBoost = 1.5f;
    public float sprintSpeed = 1.5f;
    public float sprintBoostDecayTime = 0.2f;
    public AnimationCurve sprintCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public float sprintBoostRecoveryRate = 0.5f;
    
    //---------------------------------------------------------------------------------------
    [FormerlySerializedAs("knockbackTime")]
    [Header("Knockback settings")]
    [Tooltip("Time where the player loses control after being struck by the ball.")]
    public float knockbackStunMultiplier = 0.5f;
    [FormerlySerializedAs("knockbackForce")] [Tooltip("This is the force multiplier applied to the player when hit by a ball.")]
    public float knockbackForceMultiplier = 10f;
    [Tooltip("The normal linear drag of the player.")]
    public float baseLinearDamping = 3f;
    [Tooltip("The linear drag when the player is hit by a ball.")]
    public float knockedBackLinearDamping = 0f;

    [HideInInspector] public float trueStunTime;
    [HideInInspector] public float truePushForce;
    
    //---------------------------------------------------------------------------------------
    [Header("Hit parameters")]
    [Tooltip("Select the type of hit.")]
    public HitType hitType = HitType.ForwardHit;
    [Tooltip("The speed multiplier on the ball when hit.")]
    public float hitForce = 10f;
    [Tooltip("The radius of the sphere that will detect the ball when hitting.")]
    public float hitDetectionRadius = 3.5f;
    [Tooltip("The offset of the hit detection sphere.")]
    public float hitDetectionOffset = 0f;
    [Tooltip("The window of opportunity to catch the ball at the start of the charge.")]
    public float hitCooldown = 0.3f;
    public float hitWindow = 0.5f;
    
    [Header("Grab Parameters")]
    public int maxGrabAngle = 180;
    public int minGrabAngle = 30;
    public float grabDetectionRadius = 3.5f;
    public float grabTimeLimit = 0.5f;
    public AnimationCurve grabShrinkCurve;
    [Tooltip("Rate at which the grab is discharged.")]
    public float grabDischargeRate = 1f;
    [Tooltip("Time until grab is fully charged.")]
    public float grabRechargeRate = 1f;
    [Tooltip("Total amount of charge the player has available.")]
    public float grabTotalCharge = 1f;
    [HideInInspector]public float grabCurrentCharge;
    
    
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
    public UnityEvent OnPlayerHitReleased;
    public UnityEvent OnBallHitByPlayer;
    public UnityEvent OnPlayerHitByBall; 
    public UnityEvent OnPlayerDeath;
    public UnityEvent OnPlayerDash;
    public UnityEvent OnPlayerEndDash;
    public UnityEvent OnPlayerCatch;
    public UnityEvent OnGrabStateEntered;
    public UnityEvent OnGrabStateExited;
    public Action<PlayerState> OnPlayerStateChanged;
    
    // action events
    public event Action<int,GameObject> OnBallHit;
    
    // ------------------------------ PRIVATE VARIABLES ------------------------------
    
    private bool _isAiming;
    [HideInInspector] public PlayerState currentState;
    [HideInInspector] public PlayerInput playerInput;
    [HideInInspector] public InputAction moveAction;
    [FormerlySerializedAs("throwAction")] [HideInInspector] public InputAction hitAction;
    [HideInInspector] public InputAction chargeAction;
    [HideInInspector] public InputAction dashAction;
    [HideInInspector] public InputAction reviveDebug;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public CapsuleCollider col;
    [HideInInspector] public int playerLayer;
    [HideInInspector] public int ballLayer;
    [HideInInspector] public int hazardLayer;
    [HideInInspector] public bool isReady;
    [HideInInspector] public GameObject playerScorePanel;

    [HideInInspector] public GameObject playerSpawnPoint;
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
        hitTimer = hitCooldown;
        // Subscribe to the "Pause" action with a lambda to pass the context to OnPause
        playerInput.actions["SetPause"].performed += context => 
            GameManager.Instance.levelManager.ingameGUIManager.UI_PauseMenu.OnPause(context);
        grabCurrentCharge = grabTotalCharge;
        
        int playerId = GameManager.Instance.PlayerScriptList.IndexOf(this);
        
        GameManager.Instance.levelManager.goalSpawner.LinkGoalToPlayer(playerId);
    }
    
    public void SetPlayerParameters()
    {
        MultiplayerManager = GameObject.FindWithTag("MultiPlayerManager");
        playerCamera = MultiplayerManager.GetComponent<MultiplayerManager>().camera;
        
        rb = GetComponent<Rigidbody>(); 
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        hitAction = playerInput.actions["Attack"];
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
    

    // ------------------------------ UPDATES ------------------------------
    private void FixedUpdate()
    {
        currentState.Tick();
        moveInputVector2 = moveAction.ReadValue<Vector2>();
    }

    private void Update()
    {
        // Timers
        if (hitTimer > 0)
        {
            hitTimer -= Time.deltaTime;
        }
        
        
        if (currentState is not GrabbingState && 
            grabCurrentCharge < grabTotalCharge)
        {
            grabCurrentCharge += grabRechargeRate * Time.deltaTime;
        }

        if (currentState is not SprintState)
        {
            // From the SprintState, the currentSprintBoost should be set to its maximum value (sprintMaxInitialBoost)
            if (GetComponent<SprintState>().currentSprintBoost < sprintMaxInitialBoost)
            {
                GetComponent<SprintState>().currentSprintBoost += Time.deltaTime * sprintBoostRecoveryRate;
            }
        }
        
        // Update the player score panel
        playerPoint = playerGoalToDefend.GetComponent<PointTracker>()._points;
    }
    

    // ------------------------------ STATE MANAGEMENT ------------------------------

    public void ChangeState(PlayerState newState)
    {
        //buffering inputs
        if (currentState is not KnockbackState)
        {
            if (_bufferedAction != null && Time.time - _bufferedActionTime < bufferDuration)
            {
                switch (_bufferedAction.name)
                {
                    case "Attack":
                        if (currentState is not ReleaseState)
                        {
                            newState = GetComponent<ReleaseState>();
                        }
                        break;
                    case "Sprint":
                        newState = GetComponent<SprintState>();
                        break;
                    case "Charge":
                        newState = GetComponent<GrabbingState>();
                        break;
                }
            }
        }

        OnPlayerStateChanged?.Invoke(newState);
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
                
                truePushForce = ballSM.rb.linearVelocity.magnitude * knockbackForceMultiplier;
                trueStunTime = ballSM.rb.linearVelocity.magnitude * knockbackStunMultiplier;
                
                ChangeState(GetComponent<KnockbackState>());
                // Push the player in the opposite direction of the ball
                    
                rb.AddForce(direction.normalized * truePushForce,
                    ForceMode.Impulse);
                
                // Set the ball's speed to currentBallSpeedVec3.
                ballSM.rb.linearVelocity = ballSM.currentBallSpeedVec3.magnitude * -direction.normalized;
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
            Vector3 cameraForward = playerCamera.transform.up;
            Vector3 cameraRight = playerCamera.transform.right;

            // Flatten the vectors to the ground plane and normalize
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            // Calculate the movement direction
            Vector3 moveDirection = (cameraForward * moveInputVector2.y + cameraRight * moveInputVector2.x);

            if (moveDirection != Vector3.zero)
            {
                transform.forward = Vector3.Slerp(transform.forward, moveDirection, lerpMoveSpeed);
            }
            
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


    }
    // ------------------------------ DASH ------------------------------
    public void OnSprint(InputAction.CallbackContext context)
    {
        // If the current state is NOT sprinting, then change state to sprinting.
        if (context.started && currentState is not SprintState)
        {
            ChangeState(GetComponent<SprintState>());
        }
        
        // If the current state IS the sprinting state and the context is released, then go to neutral state.
        if (context.canceled && currentState is SprintState)
        {
            ChangeState(GetComponent<NeutralState>());
        }
    }
    
    // ------------------------------ CHARGE ATTACK ------------------------------
    public void OnChargeAttack(InputAction.CallbackContext context)
    { 
        if (context.canceled)
        {
            ChangeState(GetComponent<NeutralState>());
        }
        
        if (context.started || context.performed)
        {
            ChangeState(GetComponent<GrabbingState>());
        }

        if (context.canceled && currentState is GrabbingState)
        {
            ChangeState(GetComponent<NeutralState>());
        }
        
        
    }
    
    // ------------------------------ HIT ------------------------------
    public void OnHitAttack(InputAction.CallbackContext context)
    {
        if (context.started && hitTimer <= 0f)
        {
            if (currentState is NeutralState || currentState is GrabbingState)
            {
                hitTimer = hitCooldown;
                ChangeState(GetComponent<ReleaseState>());
            }
            if (currentState is not NeutralState && currentState is not KnockbackState)
            {
                hitTimer = hitCooldown;
                BufferInput(context.action);
            }
            
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
    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.green;
    //     Gizmos.DrawRay(transform.position, transform.forward * 10);
    //     
    //     // Draw the hit sphere
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(transform.position + transform.forward * hitDetectionOffset, hitDetectionRadius);
    //     
    // }
}