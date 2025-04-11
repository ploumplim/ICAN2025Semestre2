using UnityEngine;

public class LevelChoiceState : GameState
{
    private GameManagerSM gameManagerM;
    public override void Enter()
    {
        Debug.Log("LevelChoice Enter");
        gameManagerM = GetComponent<GameManagerSM>();

        if (gameManagerM != null)
        {
            gameManagerM.ChangeState(GetComponent<LoadingState>());
        }
    }
}
