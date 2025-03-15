using UnityEngine;

public class ExitLevelState : LevelState
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public override void Enter()
    {
        base.Enter();
        LevelSM.OnLevelEnded?.Invoke();
        LevelManager.gameIsRunning = false;
        LevelManager.DestroyAllNeutralWalls();
        LevelManager.DestroyAllPointWalls();
        LevelManager.ResetAllPoints();
        LevelManager.currentRound = 0;
        LevelSM.ChangeState(GetComponent<OutOfLevelState>());


    }
}
