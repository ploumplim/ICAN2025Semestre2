using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_PauseMenu : MonoBehaviour
{
    
    [HideInInspector] public GameManager gameManager;
    [SerializeField] public GameObject pauseMenuUI;
    [SerializeField] private LevelManager levelManager;
    
    private bool isPaused = false;

    private void Start()
    {
        gameManager = GameManager.Instance;
        if (!levelManager)
        {
            Debug.LogError("Game manager is null! Drag and drop from the Container scene.");
        }
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
                levelManager.OnPauseStart?.Invoke();
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
                levelManager.OnPauseEnd?.Invoke();
                // Hide the pause menu
                pauseMenuUI.SetActive(false);
            }
        }
    }
}
