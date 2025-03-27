using UnityEngine;

public class ExitLevelState : LevelState
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public override void Enter()
    {
        base.Enter();
        LevelSM.OnLevelEnded?.Invoke();
        LevelManagerScript.OnGameEnd?.Invoke("");
        LevelManagerScript.gameIsRunning = false;
        LevelManagerScript.DestroyAllNeutralWalls();
        LevelManagerScript.DestroyAllPointWalls();
        LevelManagerScript.ResetAllPoints();
        LevelManagerScript.currentRound = 0;
        LevelSM.ChangeState(GetComponent<OutOfLevelState>());


    }
}
