// PlayingState.cs
using UnityEngine;
using System.Linq;

public class PlayingState : GameState
{
    public LevelSM levelSM;

    private GameManagerSM gameManager;

    public override void Enter()
    {
        gameManager = GetComponent<GameManagerSM>();
        
        // levelSM = GameObject.FindWithTag("LevelManager").GetComponent<LevelSM>();
        // InitGame();
    }

    public void InitGame()
    {
        Debug.Log(levelSM.currentState);
        levelSM.ChangeState(GetComponent<SetupState>());
    }

    public override void Tick()
    {
        base.Tick();
    }
}