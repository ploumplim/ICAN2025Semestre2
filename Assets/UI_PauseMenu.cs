using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_PauseMenu : MonoBehaviour
{
    
    public GameManager gameManager;


    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        // Check if the current state is PlayingState
        if (gameManager.GetComponent<GameManagerSM>().currentState is PlayingState)
        {
            Debug.Log("PausePressed");
        }
    }
}
