using UnityEngine;

public class OutOfLevelState : LevelState
{
    public override void Enter()
    {
        base.Enter();
        // enable all button game objects
        LevelManager.ingameGUIManager.startGameButtonObject.SetActive(true);
        LevelManager.ingameGUIManager.resetPlayersObject.SetActive(true);
        
        LevelManager.InitPlayers();
        LevelManager.SpawnBall();
    }
}
