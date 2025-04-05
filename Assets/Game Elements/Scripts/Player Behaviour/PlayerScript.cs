using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
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

    #region Variable Region

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
    [Tooltip("The normal linear drag of the player.")]
    public float linearDrag = 3f;
    [Tooltip("The linear drag when the player is hit by a ball.")]
    public float hitLinearDrag = 0f;
    
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


    //---------------------------------------------------------------------------------------
    [HideInInspector] public GameObject MultiplayerManager;
    
    [Header("Scene References")]
    public CameraScript playerCamera;

    public GameObject playerHand;
    
    [Header("Events")]
    // ------------------------------ EVENTS ------------------------------
    public UnityEvent OnHitButtonPressed;
    public UnityEvent<float> OnPlayerHitReleased;
    public UnityEvent<float> OnBallHitByPlayer;
    public UnityEvent OnPlayerHitByBall;
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
    [SerializeField] public bool isReady;
    
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
    
    // ------------------------------ INPUT BUFFERING ------------------------------
    private InputAction _bufferedAction;
    private float _bufferedActionTime;
    [SerializeField] [Tooltip("Time window for input buffering")]private float _bufferDuration = 0.1f;
    

    #endregion
   
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ METHODS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public void Start()
    {
        SetPlayerParameters();
        col = GetComponent<CapsuleCollider>();

        SetPlayerParameters();
        col = GetComponent<CapsuleCollider>();

        // Récupérer le PlayerSoundScript
        PlayerSoundScript soundScript = GetComponent<PlayerSoundScript>();

        // Lier les sons aux événements
        OnHitButtonPressed.AddListener(soundScript.StartChargeSound);
        OnPlayerHitReleased.AddListener((float chargeValue) => soundScript.StopChargeSound());
        OnPlayerHitReleased.AddListener((float chargeValue) => soundScript.PlayHitSound());
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
        
        
        
    }
    

    // ------------------------------ STATE MANAGEMENT ------------------------------

    public void ChangeState(PlayerState newState)
    {
        //buffering inputs
        if (_bufferedAction != null && Time.time - _bufferedActionTime < _bufferDuration)
        {
            switch (_bufferedAction.name)
            {
                case "Attack":
                    Debug.Log("Buffered attack");
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
                // Debug.Log("Ball hit player");
                if (currentState is not KnockbackState &&
                    currentState is not DeadState)
                {
                    PlayerEndedDash?.Invoke();
                    ChangeState(GetComponent<KnockbackState>());
                    // Push the player in the opposite direction of the ball
                    Vector3 direction = transform.position - other.transform.position;
                    rb.AddForce(
                        direction.normalized * other.gameObject.GetComponent<Rigidbody>().linearVelocity.magnitude * knockbackForce,
                        ForceMode.Impulse);
                    // Apply an opposite force to the ball
                    other.gameObject.GetComponent<Rigidbody>().AddForce(
                        -direction.normalized * other.gameObject.GetComponent<Rigidbody>().linearVelocity.magnitude * knockbackForce,
                        ForceMode.Impulse);
                }
                
            }
            else if (ballSM.currentState is LethalBallState)
            {
                if (currentState is not KnockbackState &&
                    currentState is not DeadState)
                {
                    PlayerEndedDash?.Invoke();
                    ChangeState(GetComponent<DeadState>());
                    // Apply an opposite force to the ball
                    Vector3 direction = transform.position - other.transform.position;
                    other.gameObject.GetComponent<Rigidbody>().AddForce(
                        -direction.normalized * other.gameObject.GetComponent<Rigidbody>().linearVelocity.magnitude * knockbackForce,
                        ForceMode.Impulse);
                }
            }

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
    
    
    // ------------------------------ CHARGE ATTACK ------------------------------
    public void OnChargeAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (currentState is NeutralState)
            {
                OnHitButtonPressed?.Invoke();
                ChangeState(GetComponent<ChargingState>());
            }
            else if (currentState is not ChargingState && currentState is not ReleaseState)
            {
                BufferInput(context.action);
            }
        }
        else if (currentState is ChargingState && context.canceled) 
        { 
            ChangeState(GetComponent<ReleaseState>()); 
        }

        if (context.performed)
        {
            if (currentState is not NeutralState && currentState is not ChargingState
                && currentState is not ReleaseState)
            {
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
        
        // Draw the hit sphere
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * hitDetectionOffset, hitDetectionRadius);
        
    }
}