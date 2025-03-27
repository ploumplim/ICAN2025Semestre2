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
    public float speed = 5f;

    private void Start()
    {
        playerInputs = GetComponent<PlayerInput>();
        _moveAction = playerInputs.actions["Move"];
    }
    
    public void Update()
    {
        _moveInput = _moveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(_moveInput.x, 0, _moveInput.y) * (speed * Time.deltaTime);
        transform.Translate(move, Space.World);
    }
}
