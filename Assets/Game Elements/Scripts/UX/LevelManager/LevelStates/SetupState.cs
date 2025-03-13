using UnityEngine;
using System.Collections;
public class SetupState : LevelState
{
    public override void Enter()
    {
        LevelManager.gameIsRunning = true;

        LevelManager.ResetAllPoints();
        
        LevelManager.StartRound();

        LevelManager.RemovePlayerControl();
        
        // Wait for the Setup time to pass before starting the round.
        StartCoroutine(WaitForSetupTime());
        
    }
    
    IEnumerator WaitForSetupTime()
    {
        Debug.Log("Waiting for setup time.");
        yield return new WaitForSeconds(LevelManager.setupTime);
        LevelSM.ChangeState(LevelManager.GetComponent<InRoundState>());
        LevelManager.ReturnPlayerControl();
        Debug.Log("Setup time passed. First round starting.");
    }
}
