using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerScript : MonoBehaviour
{
    // ------------------------------ VARIABLES ------------------------------

    public PlayerState currentState;

    [Header("Player Stats")]
    public float speed = 5f;
    // public float mouseRotationSmoothSpeed = 10f;
    [Tooltip("The speed at which the player moves when aiming.")]
    public float aimSpeedMod = 0f;
    
    [Tooltip("Lerp time for the rotation while not aiming")]
    public float rotationLerpTime = 0.1f;
    
    [FormerlySerializedAs("rotationLerpTime")] [Tooltip("Lerp time for the rotation while aiming")]
    public float rotationWhileAimingLerpTime = 0.1f;
    [Header("Scene References")]
    public Camera playerCamera;
    
    [HideInInspector] public PlayerInput playerInput;
    [HideInInspector] public InputAction moveAction;
    [HideInInspector] public InputAction throwAction;
    [HideInInspector] public InputAction lookAction;
    [HideInInspector] public Rigidbody rb;
    public GameObject playerHand;
    private bool _isAiming;

    // ------------------------------ BALL ------------------------------
    [HideInInspector] public GameObject heldBall;
    [HideInInspector] public BallSM ballSM;

    // ------------------------------ CHARGING ------------------------------
    [HideInInspector]public float chargeValueIncrementor = 0.5f;
    private float fixedChargedValue;
    private bool isCharging = false;
    private const float chargeRate = 0.5f; // Rate at which the charge value increases

    public void Start()
    {
        rb = GetComponent<Rigidbody>();

        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        throwAction = playerInput.actions["Attack"];
        lookAction = playerInput.actions["Look"];

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

        if (heldBall)
        {
            heldBall.transform.position = playerHand.transform.position;
        }
        
        ChargingForce();
        
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
        Debug.Log("State changed to: " + newState);
    }

    // ------------------------------ COLLISIONS ------------------------------

    public void OnCollisionEnter(Collision other)
    {
        // if other is a gameobject with the BallSM component, then assign it to the heldBall variable
        if (other.gameObject.GetComponent<BallSM>() && // if the other object has a BallSM component
            (other.gameObject.GetComponent<BallSM>().currentState == other.gameObject.GetComponent<DroppedState>() ||
             other.gameObject.GetComponent<BallSM>().currentState == other.gameObject.GetComponent<MidAirState>()) && // if the other object is NOT in the DroppedState
            heldBall == null) // if the player is not already holding a ball
        {
            heldBall = other.gameObject;
            ballSM = heldBall.GetComponent<BallSM>();
            ballSM.player = gameObject;
            // The ball is set to the InHandState.
            ballSM.ChangeState(heldBall.GetComponent<InHandState>());
        }
    }

    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ INPUTS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // ------------------------------ MOVE ------------------------------
    public void OnMove(InputAction.CallbackContext context)
    {
        if (currentState is IdleState)
        {
            ChangeState(GetComponent<MovingState>());
        }
    }

    public void Move(bool isAiming)
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        // Apply movement
        if (moveInput != Vector2.zero)
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

            // Move the player. 
            if (!isAiming)
            {
                rb.linearVelocity = new Vector3(moveDirection.x * speed,
                    rb.linearVelocity.y,
                    moveDirection.z * speed);
                //Set the player's direction to the direction of the movement using a lerp
                transform.forward = Vector3.Slerp(transform.forward, moveDirection, rotationLerpTime);
            }
            else
            {
                
                rb.linearVelocity = new Vector3(moveDirection.x * speed * aimSpeedMod,
                    rb.linearVelocity.y,
                    moveDirection.z * speed * aimSpeedMod);
                //Set the player's direction to the direction of the movement using a lerp
                transform.forward = Vector3.Slerp(transform.forward, moveDirection, rotationWhileAimingLerpTime);
            }
        }
        
        
        if (moveAction.ReadValue<Vector2>() == Vector2.zero && !isAiming)
        {
            ChangeState(GetComponent<IdleState>());
        }
    }

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

    // ------------------------------ THROW ------------------------------
    public void OnThrow(InputAction.CallbackContext context)
    {
        if (heldBall)
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

    }

    // ------------------------------ PLAYER GIZMOS ------------------------------

    // Create a gizmo to show the direction the player is looking at
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 10);
    }
}