using UnityEngine;

public class ExitLevelState : LevelState
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public override void Exit()
    {
        LevelSM.OnLevelEnded?.Invoke();
    }
}
