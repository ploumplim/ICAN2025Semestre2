using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Anim : MonoBehaviour
{
    [HideInInspector] public PlayerInput playerInput;
    [HideInInspector] public InputAction moveAction;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public PlayerState currentState;

    public Vector2 moveDirection;

    public PlayerScript playerScript;
    public PlayerVisuals playerVisuals;


    //// ------------------------------ STATE MANAGEMENT ------------------------------
    //public void ChangeState(PlayerState newState)
    //{
    //    currentState.Exit();
    //    currentState = newState;
    //    currentState.Enter();
    //    // Debug.Log("State changed to: " + newState);
    //}


    Animator myAnimator;


    public void Start()
    {
        playerScript = GetComponent<PlayerScript>();
        

        rb = GetComponent<Rigidbody>();
       
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        

        PlayerState[] states = GetComponents<PlayerState>();
        

        currentState = GetComponent<IdleState>();

        myAnimator = GetComponent<Animator>();
    }

    

    public void FixedUpdate()
    {
        moveDirection = playerScript.moveInput;
    }

    public void Update()
    {
        AnimateCharacter();
    }

    public void AnimateCharacter()
    {
        if (moveDirection.magnitude > 0.1f) // Si le joueur bouge
        {
            myAnimator.SetBool("IsWalking", true);
            myAnimator.SetBool("IsIdle", false);
        }
        else // Si le joueur ne bouge pas
        {
            myAnimator.SetBool("IsWalking", false);
            myAnimator.SetBool("IsIdle", true);
        }
    }




}
