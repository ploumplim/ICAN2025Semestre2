using UnityEngine;

public class LevelChoiceState : GameState
{
    private GameManagerSM gameManager;
    public override void Enter()
    {
        Debug.Log("LevelChoice Enter");
        gameManager = GetComponent<GameManagerSM>();

        if (gameManager != null)
        {
            gameManager.ChangeState(GetComponent<LoadingState>());
        }
    }
}
