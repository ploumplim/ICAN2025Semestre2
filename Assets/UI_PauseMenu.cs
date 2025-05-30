using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_PauseMenu : MonoBehaviour
{
    
    [HideInInspector] public GameManager gameManager;
    [SerializeField] public GameObject pauseMenuUI;
    
    private bool isPaused = false;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        // Check if the current state is PlayingState
        if (gameManager.GetComponent<GameManagerSM>().currentState is PlayingState)
        {
            isPaused = !isPaused;
            if (isPaused)
            {
                // Show the pause menu
                pauseMenuUI.SetActive(true);
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
                // Hide the pause menu
                pauseMenuUI.SetActive(false);
            }
        }
    }
}
