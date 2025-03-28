using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPSplaygroundcontroller : MonoBehaviour
{
    // Add the input map.
    public PlayerInput playerInputs;
    private Vector2 _moveInput;
    private InputAction _moveAction;
    private InputAction _rotateLeft;
    private InputAction _rotateRight;
    public float speed = 5f;
    public float rotateValue = 15;
    public Camera playerCamera;

    private void Start()
    {
        playerInputs = GetComponent<PlayerInput>();
        playerCamera = GetComponentInChildren<Camera>();
        _moveAction = playerInputs.actions["Move"];
        _rotateLeft = playerInputs.actions["TurnLeft"];
        _rotateRight = playerInputs.actions["TurnRight"];
    }
    
    public void Update()
    {
        _moveInput = _moveAction.ReadValue<Vector2>();
        
        // Get the camera's forward and right vectors
        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 cameraRight = playerCamera.transform.right;

        // Flatten the vectors to the ground plane and normalize
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate the movement direction
        Vector3 moveDirection = (cameraForward * _moveInput.y + cameraRight * _moveInput.x);
        transform.Translate(moveDirection, Space.World);
    }
    
    public void OnTurnLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            transform.Rotate(Vector3.up, rotateValue);
        }
    }
    public void OnTurnRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            transform.Rotate(Vector3.up, -rotateValue);
        }
    }
}
