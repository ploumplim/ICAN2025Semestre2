// EndGameState.cs
using UnityEngine;

public class EndGameState : GameState
{
    private GameManagerSM gameManager;

    public override void Enter()
    {
        Debug.Log("EndGame Enter");

        gameManager = GetComponent<GameManagerSM>();

        if (gameManager != null)
        {
            gameManager.ChangeState(GetComponent<MenuState>());
        }
        else
        {
            Debug.LogError("GameManagerSM not found on the GameObject.");
        }
    }

    public override void Tick()
    {
        base.Tick();
    }
}