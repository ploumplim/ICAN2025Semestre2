// PlayingState.cs
using UnityEngine;

public class PlayingState : GameState
{
    public LevelSM levelSM;

    private GameManagerSM gameManager;

    public override void Enter()
    {
        Debug.Log("Passage playingState");
        GameManager.Instance.multiplayerManager.PlayerJoin();
    }
    

    public override void Tick()
    {
        base.Tick();
    }
}