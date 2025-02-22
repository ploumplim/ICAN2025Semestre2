using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerScript : MonoBehaviour
{

    
    // ------------------------------ PUBLIC VARIABLES ------------------------------
    public enum moveType
    {
        Velocity,
        Force
    };
    
    [HideInInspector] public PlayerState currentState;
    
    [Header("MOVEMENT TYPES: \n " +
        "Velocity: The player's movement is controlled \n" +
        " by changing the velocity of the rigidbody.\n" +
        "Force: The player's movement is controlled \n" +
        " by applying a force to the rigidbody. \n" +
        " This means that all movement variables \n " +
        "should be decreased to avoid the player moving too fast.")]
    [Tooltip("Choose the player's movement type.")]
    public moveType movementType = moveType.Velocity;
    [Header("Movement variables")]
    [Tooltip("The player's speed when he has balls.")]
    public float speed = 5f;
    
    [Tooltip("Multiplies the speed of the player when he has no balls.")]
    public float speedWithoutBallsModifier = 1f;
    
    [Tooltip("The speed at which the player moves when aiming.")]
    public float aimSpeedMod = 0f;
    
    [Header("Knockback")]
    [Tooltip("Time where the player loses control after being struck by the ball.")]
    public float knockbackTime = 0.5f;
    
    [Tooltip("The normal linear drag of the player.")]
    public float linearDrag = 3f;
    [Tooltip("The linear drag when the player is hit by a ball.")]
    public float hitLinearDrag = 0f;
    [Header("Rotation Lerps")]
    [Tooltip("Lerp time for the rotation while not aiming")]
    public float rotationLerpTime = 0.1f;
    
    [FormerlySerializedAs("rotationLerpTime")]
    [Tooltip("Lerp time for the rotation while aiming")]
    public float rotationWhileAimingLerpTime = 0.1f;
    
    [Tooltip("Lerp time for the rotation while rolling")]
    public float rollLerpTime = 0.1f;
    
    [Header("Charge shot")]
    public float chargeRate = 0.5f; // Rate at which the charge value increases
    
    [Header("Parry")]
    [Tooltip("The time the player has to wait between each parry.")]
    public float parryCooldown = 0.5f;
    [Tooltip("The force applied to the ball when parrying.")]
    public float parryForce = 10f;
    [Tooltip("The window of opportunity that the parry will hit the ball.")]
    public float parryWindow = 0.4f;
    [Tooltip("The radius of the sphere that will detect the ball when parrying.")]
    public float parryDetectionRadius = 3.5f;
    
    [Header("Roll")]
    [Tooltip("The initial speed of the roll.")]
    public float rollSpeed = 10f;
    [Tooltip("The duration of the roll. This number HAS to be bigger than the catch window.")]
    public float rollDuration = 1f;
    [Tooltip("The window of opportunity where the player can catch the ball whilst midair.")]
    public float catchWindow = 0.6f;
    [Tooltip("This is the radius of the sphere that will detect the ball when rolling.")]
    public float rollDetectionRadius = 5f;
    [Tooltip("This boolean determines if when dashing the character can pass through ledges.")]
    public bool canPassThroughLedges = false;
    
    public GameObject MultiplayerManager;
    
    
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
    [HideInInspector]public float chargeValueIncrementor = 0.5f;
    private float fixedChargedValue;
    private bool isCharging = false;
    
    // ------------------------------ PARRY ------------------------------
    private ParryPlayer _parryPlayer;
    [FormerlySerializedAs("_canParry")] [HideInInspector] public bool canParry = true;
    [HideInInspector] public float parryTimer = 0f;
    // ------------------------------ ROLL ------------------------------
    // [HideInInspector]public bool ballCaughtWhileRolling;
    
    // ------------------------------ MOVE ------------------------------
    [HideInInspector] public Vector2 moveInput;
    
    
    
    
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
        _parryPlayer = GetComponentInChildren<ParryPlayer>();
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

        currentState = GetComponent<IdleState>();
    }

    private void FixedUpdate()
    {
        currentState.Tick();
        moveInput = moveAction.ReadValue<Vector2>();
        ChargingForce();

        // If the player is holding a ball, set the ball's position to the player's hand
        if (heldBall)
        {
            heldBall.transform.position = playerHand.transform.position;
        }
        
        
        if (parryTimer > 0)
        {
            parryTimer -= Time.deltaTime;
        }
        
    }
    public void ChargingForce()
    {
        if (isCharging)
        {
            chargeValueIncrementor += chargeRate * Time.deltaTime;
            chargeValueIncrementor = Mathf.Clamp(chargeValueIncrementor, 0f, 1f);
            // Debug.Log(chargeValueIncrementor);
            
        }
        else
        {
            chargeValueIncrementor = 0f;
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
        // if other is a gameobject with the BallSM component, then assign it to the heldBall variable
        // if (other.gameObject.GetComponent<BallSM>() && // if the other object has a BallSM component
        //     (other.gameObject.GetComponent<BallSM>().currentState == other.gameObject.GetComponent<DroppedState>() ||
        //      other.gameObject.GetComponent<BallSM>().currentState == other.gameObject.GetComponent<MidAirState>()) && // if the other object is NOT in the DroppedState
        //     heldBall == null) // if the player is not already holding a ball
        
        
        if (other.gameObject.GetComponent<BallSM>() && // if the other object has a BallSM component
            (other.gameObject.GetComponent<BallSM>().currentState == other.gameObject.GetComponent<DroppedState>()))
        {
            heldBall = other.gameObject;
            ballSM = heldBall.GetComponent<BallSM>();
            ballSM.player = gameObject;
            // The ball is set to the InHandState.
            ballSM.ChangeState(heldBall.GetComponent<InHandState>());
        }
        if (other.gameObject.GetComponent<BallSM>())
        {
            // Debug.Log(currentState);
            if (other.gameObject.GetComponent<BallSM>().currentState==other.gameObject.GetComponent<MidAirState>())
            {
                if (GetComponent<RollingState>().timer > catchWindow || currentState is not MomentumState)
                {
                    PlayerEndedDash?.Invoke();
                    ChangeState(GetComponent<MomentumState>());
                    _parryPlayer.parryTimer = 0;
                    // Push the player in the opposite direction of the ball
                    Vector3 direction = transform.position - other.transform.position;
                    rb.AddForce(
                        direction.normalized * other.gameObject.GetComponent<Rigidbody>().linearVelocity.magnitude,
                        ForceMode.Impulse);
                    // Set ball to dropped state
                    other.gameObject.GetComponent<BallSM>().ChangeState(other.gameObject.GetComponent<DroppedState>());
                }
                
            }
            // if (currentState is RollingState)
            // {
            //     // ballCaughtWhileRolling = true;
            //     Debug.Log("Ball touched while rolling");
            //     GetComponent<RollingState>().CheckCatch(other.gameObject);
            // }
        }
    }

    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ INPUTS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // ------------------------------ MOVE ------------------------------
    public void OnMove(InputAction.CallbackContext context)
    {
        if (currentState is not MomentumState &&
            currentState is not RollingState &&
            currentState is not AimingState)
        {
            ChangeState(GetComponent<MovingState>());
        }
    }

    public void Move(bool isAiming)
    {
        // Apply movement
        if (moveInput != Vector2.zero)
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
            Vector3 moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;

            // Determine the speed and rotation lerp time based on the player's state
            float currentSpeed = isAiming ? speed * aimSpeedMod : (heldBall ? speed : speed * speedWithoutBallsModifier);
            float currentLerpTime = isAiming ? rotationWhileAimingLerpTime : rotationLerpTime;

            // Apply movement and set the player's direction
            ApplyMovement(moveDirection, currentSpeed);
            transform.forward = Vector3.Slerp(transform.forward, moveDirection, currentLerpTime);
        }

        // Change state to Idle if no movement input and not aiming
        if (moveAction.ReadValue<Vector2>() == Vector2.zero && !isAiming)
        {
            ChangeState(GetComponent<IdleState>());
        }
    }

    public void SteerRoll()
    {
        // Get the camera's forward and right vectors
        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 cameraRight = playerCamera.transform.right;

        // Flatten the vectors to the ground plane
        cameraForward.y = 0;
        cameraRight.y = 0;

        // Normalize the vectors
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate the movement direction lerping using the rollLerpTime
        Vector3 moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;
        
        // change the player's forward direction to the direction of the movement
        transform.forward = Vector3.Slerp(transform.forward, moveDirection, rollLerpTime);
        
        //Apply the movement, decreasing the speed of the player over time.
        float decreasingSpeed = Mathf.Lerp(rollSpeed, 0, GetComponent<RollingState>().timer / rollDuration);
        ApplyMovement(moveDirection, decreasingSpeed);
        
    }
    
    private void ApplyMovement(Vector3 moveDirection, float finalSpeed)
    {

        switch (movementType)
        {
            case moveType.Force:
                rb.AddForce(moveDirection * finalSpeed, ForceMode.VelocityChange);
                break;
            case moveType.Velocity:
                rb.linearVelocity = new Vector3(moveDirection.x * finalSpeed,
                rb.linearVelocity.y,
                moveDirection.z * finalSpeed);
            break;
            
                
        }
        
    }
    
    // ------------------------------ THROW & PARRY ------------------------------
    
    // ------------------------------ THROW ------------------------------
    public void OnThrow(InputAction.CallbackContext context)
    {
        if (heldBall && currentState is not MomentumState && currentState is not RollingState)
        {
            ChangeState(GetComponent<AimingState>());
            if (context.performed)
            {
                isCharging = true;
                chargeValueIncrementor = 0f;
                // Debug.Log(chargeValueIncrementor);
            }
            else if (context.canceled)
            {
                isCharging = false;
                if (chargeValueIncrementor > fixedChargedValue)
                {
                    fixedChargedValue = chargeValueIncrementor;
                    ballSM.ChangeState(heldBall.GetComponent<TargetingState>());
                    ballSM.Throw(fixedChargedValue);
                    heldBall = null;
                    // Reset après avoir utilisé la charge
                    fixedChargedValue = 0f;
                    ChangeState(GetComponent<IdleState>());

                }

            }
        }
        if (!heldBall && context.performed &&
            currentState is not MomentumState &&
            currentState is not RollingState)
        {
            Parry();
        }

    }
    // ------------------------------ PARRY ------------------------------
    public void Parry()
    {
        if (canParry)
        {
            // Debug.Log("Parry!");
            PlayerParried?.Invoke();
            canParry = false;
            parryTimer = parryCooldown;
            StartCoroutine(ParryTime());
        }
    }

    IEnumerator ParryTime()
    {
        _parryPlayer.playerHasParried = true;
        yield return new WaitForSeconds(parryCooldown);
        _parryPlayer.playerHasParried = false;
        canParry = true;
    }
    
    // ------------------------------ ROLL ------------------------------
    
    public void OnRoll(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (currentState is MovingState && !heldBall && canParry)
            {
                PlayerDashed?.Invoke();
                ChangeState(GetComponent<RollingState>());
            }
        }
    }
    
    public Vector3 RollPush()
    {
// Get the camera's forward and right vectors
        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 cameraRight = playerCamera.transform.right;

        // Flatten the vectors to the ground plane
        cameraForward.y = 0;
        cameraRight.y = 0;

        // Normalize the vectors
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate the movement direction
        Vector3 moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;
        return moveDirection;
    }

    // ------------------------------ PLAYER GIZMOS ------------------------------

    // Create a gizmo to show the direction the player is looking at
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 10);
    }
    
    
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ DEPRECATED CODE ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ------------------------------ LOOK ------------------------------
    // public void OnLook(InputAction.CallbackContext context)
    // {
    //     // unlock the rigidbody rotation on Y
    //     rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    //
    //     Vector2 input = context.ReadValue<Vector2>();
    //
    //     // Get the camera's forward and right vectors
    //     Vector3 cameraForward = playerCamera.transform.forward;
    //     Vector3 cameraRight = playerCamera.transform.right;
    //
    //     // Flatten the vectors to the ground plane
    //     cameraForward.y = 0;
    //     cameraRight.y = 0;
    //
    //     // Normalize the vectors
    //     cameraForward.Normalize();
    //     cameraRight.Normalize();
    //
    //     if (playerInput.currentControlScheme == "Gamepad")
    //     {
    //         Cursor.visible = true;
    //         if (Cursor.lockState != CursorLockMode.None)
    //         {
    //             Cursor.lockState = CursorLockMode.None;
    //         }
    //         // set the player's look direction equal to my input value, so that I always look towards the direction I'm aiming, taking into account the camera angle
    //         Vector3 lookDirection = cameraForward * input.y + cameraRight * input.x;
    //         if (lookDirection != Vector3.zero)
    //         {
    //             transform.forward = lookDirection;
    //         }
    //     }
    //     // if I'm using the keyboard and mouse, lock it.
    //     if (playerInput.currentControlScheme == "Keyboard&Mouse")
    //     {
    //         Cursor.visible = false;
    //         if (Cursor.lockState == CursorLockMode.None)
    //         {
    //             Cursor.lockState = CursorLockMode.Locked;
    //         }
    //         // set the player's look direction equal to my input value, so that I always look towards the direction I'm aiming, taking into account the camera angle
    //         Vector3 lookDirection = cameraForward * input.y + cameraRight * input.x;
    //         if (lookDirection != Vector3.zero)
    //         {
    //             transform.forward = Vector3.Slerp(transform.forward, lookDirection, mouseRotationSmoothSpeed * Time.deltaTime);
    //         }
    //     }
    //
    //     // lock the rigidbody rotation on Y
    //     rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    // }
}