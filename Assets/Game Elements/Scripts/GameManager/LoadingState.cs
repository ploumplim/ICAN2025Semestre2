using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingState : GameState
{
    private GameManagerSM gameManager;

    public override void Enter()
    {
        Debug.Log("LoadingState Enter");

        gameManager = GetComponent<GameManagerSM>();

        if (gameManager != null)
        {
            // Start the coroutine to change to PlayingState after 2 seconds
            StartCoroutine(ChangeToPlayingStateAfterDelay(2f));
        }
    }

    private IEnumerator ChangeToPlayingStateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(1);
        yield return null; // Wait for the scene to load
        gameManager.ChangeState(GetComponent<PlayingState>());
    }

    public override void Tick()
    {
        base.Tick();
    }
}
