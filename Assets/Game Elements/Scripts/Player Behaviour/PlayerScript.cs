using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    
    // ------------------------------ VARIABLES ------------------------------
    
    [Header("Player Stats")]
    public float speed = 5f;
    
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
    }

    public void OnCollisionEnter(Collision other)
    {
        // if other is a gameobject with the BallSM component, then assign it to the heldBall variable
        if (other.gameObject.GetComponent<BallSM>() && // if the other object has a BallSM component
            other.gameObject.GetComponent<BallSM>().currentState == other.gameObject.GetComponent<DroppedState>() && // if the other object is NOT in the DroppedState
            heldBall == null) // if the player is not already holding a ball
        {
            heldBall = other.gameObject;
            ballSM = heldBall.GetComponent<BallSM>();
            ballSM.player = gameObject;
            // The ball is set to the InHandState.
            ballSM.ChangeState(heldBall.GetComponent<InHandState>());
        }
    }


    // ------------------------------ GAMEPAD INPUTS ------------------------------
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
        rb.linearVelocity = new Vector3(moveDirection.x*speed,rb.linearVelocity.y,moveDirection.z*speed);
    }
    
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
        
        
        // set the player's look direction equal to my input value, so that I always look towards the direction I'm aiming, taking into account the camera angle
        Vector3 lookDirection = cameraForward * input.y + cameraRight * input.x;
        if (lookDirection != Vector3.zero)
        {
            transform.forward = lookDirection;
        }
        
        // lock the rigidbody rotation on Y
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }
    
    public void OnThrow(InputAction.CallbackContext context)
    {
        if (context.started && heldBall)
        {
            heldBall = null;
            ballSM.Throw();
            
        }
    }
    

    // ------------------------------ MOUSE & KEYBOARD INPUTS ------------------------------
    
    // ------------------------------ PLAYER GIZMOS ------------------------------
    
    // Create a gizmo to show the direction the player is looking at
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 10);
    }
    
}
