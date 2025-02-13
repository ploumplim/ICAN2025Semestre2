using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    
    // ------------------------------ VARIABLES ------------------------------
    
    [Header("Player Stats")]
    public float speed = 5f;

    public float mouseRotationSmoothSpeed = 10f;
    
    [Header("Scene References")]
    public Camera playerCamera;
    
    [HideInInspector]public PlayerInput playerInput;
    [HideInInspector]public InputAction moveAction;
    [HideInInspector]public InputAction throwAction;
    [HideInInspector]public InputAction lookAction;
    [HideInInspector]public Rigidbody rb;
    public GameObject playerHand;
    private bool _isAiming;
    
    
    // ------------------------------ BALL ------------------------------
    [HideInInspector] public GameObject heldBall;
    [HideInInspector] public BallSM ballSM;
    
    
    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        throwAction = playerInput.actions["Attack"];
        lookAction = playerInput.actions["Look"];
    }

    public void FixedUpdate()
    {
        if (heldBall)
        {
            heldBall.transform.position = playerHand.transform.position;
        }

        if (playerInput.currentControlScheme == "Keyboard&Mouse")
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

                // Move the player
                rb.linearVelocity = new Vector3(moveDirection.x * speed, rb.linearVelocity.y, moveDirection.z * speed);
            }
        }
    }

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
        
        Vector2 input = context.ReadValue<Vector2>();

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
        Vector3 moveDirection = (cameraForward * input.y + cameraRight * input.x).normalized;

        // Move the player
        rb.linearVelocity = new Vector3(moveDirection.x * speed, rb.linearVelocity.y, moveDirection.z * speed);
    }
    
    // ------------------------------ LOOK ------------------------------
    public void OnLook(InputAction.CallbackContext context)
    {
        // unlock the rigidbody rotation on Y
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        
        
        Vector2 input = context.ReadValue<Vector2>();
        
        // Get the camera's forward and right vectors
        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 cameraRight = playerCamera.transform.right;

        // Flatten the vectors to the ground plane
        cameraForward.y = 0;
        cameraRight.y = 0;

        // Normalize the vectors
        cameraForward.Normalize();
        cameraRight.Normalize();
        
        if (playerInput.currentControlScheme == "Gamepad")
        {
            Cursor.visible = true;
            if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            // set the player's look direction equal to my input value, so that I always look towards the direction I'm aiming, taking into account the camera angle
            Vector3 lookDirection = cameraForward * input.y + cameraRight * input.x;
            if (lookDirection != Vector3.zero)
            {
                transform.forward = lookDirection;
            }
        }
        // if I'm using the keyboard and mouse, lock it.
        if (playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            Cursor.visible = false; 
            if (Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            // set the player's look direction equal to my input value, so that I always look towards the direction I'm aiming, taking into account the camera angle
            Vector3 lookDirection = cameraForward * input.y + cameraRight * input.x;
            if (lookDirection != Vector3.zero)
            {
                transform.forward = Vector3.Slerp(transform.forward, lookDirection, mouseRotationSmoothSpeed * Time.deltaTime);
            }
        }
        
        
        // lock the rigidbody rotation on Y
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }
    
    public void OnThrow(InputAction.CallbackContext context)
    {
        if (context.started && heldBall)
        {            
            ballSM.ChangeState(heldBall.GetComponent<TargetingState>());
            heldBall = null;
            ballSM.Throw();
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
