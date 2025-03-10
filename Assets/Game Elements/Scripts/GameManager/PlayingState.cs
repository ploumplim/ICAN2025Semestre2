// PlayingState.cs
using UnityEngine;

public class PlayingState : GameState
{
    private GameManagerSM gameManager;

    public override void Enter()
    {
        Debug.Log("PlayingState Enter");

        gameManager = GetComponent<GameManagerSM>();

        if (gameManager != null)
        {
            gameManager.ChangeState(GetComponent<EndGameState>());
        }
        
    }

    public override void Tick()
    {
        base.Tick();
    }
}