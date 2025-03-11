using UnityEngine;

public class BufferState : LevelState
{
    private bool _finalRound;
    public override void Enter()
    {
        LevelManager.DestroyAllPointWalls();
        LevelManager.DestroyAllNeutralWalls();
        _finalRound = LevelManager.RoundCheck();
        if (_finalRound)
        {
            LevelSM.ChangeState(GetComponent<ExitLevelState>());
        }
        else
        {
            LevelSM.ChangeState(GetComponent<InRoundState>());
            LevelManager.SpawnBall();
        }
    }

}
