using UnityEngine;

public class ExitLevelState : LevelState
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public override void Enter()
    {
        LevelSM.OnLevelEnded?.Invoke();
        LevelSM.ChangeState(GetComponent<OutOfLevelState>());
        LevelManager.gameIsRunning = false;
        LevelManager.ingameGUIManager.startGameButtonObject.SetActive(false);

    }
}
