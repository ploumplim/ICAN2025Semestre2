using UnityEngine;

public class OptionState : GameState
{
    private GameManagerSM gameManager;

    public override void Enter()
    {
        Debug.Log("OptionState Enter");

        gameManager = GetComponent<GameManagerSM>();

        if (gameManager != null)
        {
            gameManager.ChangeState(GetComponent<MenuState>());
        }
        
    }

    public override void Tick()
    {
        base.Tick();
    }
}
