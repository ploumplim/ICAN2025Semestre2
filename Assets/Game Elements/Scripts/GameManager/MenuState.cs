using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuState : GameState
{
    public override void Enter()
    {
        Debug.Log("MenuState Enter");
        // foreach (var VARIABLE in GameManager.mpManager.connectedPlayers)
        // {
        //     VARIABLE.GetComponent<PlayerScript>().InMenu = true;
        // }
        GameManagerSM = GetComponent<GameManagerSM>();
        
    }

    public void GoToLevelChoiceState()
    {
        GameManagerSM.ChangeState(GetComponent<LevelChoiceState>());
        SceneManager.LoadScene(1); 
    }
}