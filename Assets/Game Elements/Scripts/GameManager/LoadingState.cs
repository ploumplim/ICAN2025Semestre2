using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingState : GameState
{

    public override void Enter()
    {
        Debug.Log("LoadingState Enter");

        GameManagerSM = GetComponent<GameManagerSM>();

        if (GameManagerSM != null)
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
        GameManagerSM.ChangeState(GetComponent<PlayingState>());
    }

    public override void Tick()
    {
        base.Tick();
    }
    
    public override void Exit()
    {
        // Debug.Log("LoadingState Exit");

        foreach (var VARIABLE in  GameManager.mpManager.connectedPlayers)
        {
            VARIABLE.GetComponent<PlayerScript>().InMenu = false;
        }
       

    }
}
