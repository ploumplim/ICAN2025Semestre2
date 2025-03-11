// PlayingState.cs
using UnityEngine;

public class PlayingState : GameState
{
    public LevelSM levelSM;

    private GameManagerSM gameManager;

    public override void Enter()
    {
        Debug.Log("PlayingState Enter");

        gameManager = GetComponent<GameManagerSM>();

        levelSM = GameObject.FindWithTag("LevelManager").GetComponent<LevelSM>();
        levelSM.Init(); // Ensure Init is called before ChangeState
        InitGame();
    }

    public void InitGame()
    {
        Debug.Log(levelSM.currentState);
        levelSM.Init();
    }

    public override void Tick()
    {
        base.Tick();
    }
}