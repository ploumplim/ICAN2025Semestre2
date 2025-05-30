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
        LevelManagerScript.ResetAllPoints();
        LevelManagerScript.currentRound = 0;
        LevelManagerScript.OnMatchEnd?.Invoke();
        GameManager.Instance.levelManager.ingameGUIManager.GetComponent<EndGameScorePanel>().StartEndGamePanel();
        
        LevelSM.ChangeState(GetComponent<OutOfLevelState>());


    }
}
