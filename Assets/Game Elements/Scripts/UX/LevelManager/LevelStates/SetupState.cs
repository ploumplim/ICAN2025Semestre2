using UnityEngine;

public class SetupState : LevelState
{
    public override void Enter()
    {
        LevelManager.DestroyAllPointWalls();
        LevelManager.DestroyAllNeutralWalls();
        
    }
}
