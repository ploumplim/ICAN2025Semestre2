using UnityEngine;

public class SetupState : LevelState
{
    public override void Enter()
    {
        LevelManager.DestroyAllPointWalls();
        LevelManager.DestroyAllNeutralWalls();
        LevelManager.ResetAllPoints();
        
        // Spawn ball
        LevelManager.SpawnBall();
        LevelManager.InitPlayers();
        
    }
}
