using UnityEngine;

public class OutOfLevelState : LevelState
{
    public override void Enter()
    {
        base.Enter();
        // enable all button game objects
        LevelManagerScript.ingameGUIManager.startGameButtonObject.SetActive(true);
        LevelManagerScript.ingameGUIManager.resetPlayersObject.SetActive(true);
        
        LevelManagerScript.InitPlayers();
        LevelManagerScript.SpawnBall();
    }
   
}
