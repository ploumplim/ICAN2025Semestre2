using UnityEngine;
using System.Collections;
public class SetupState : LevelState
{
    public override void Enter()
    {
        LevelManagerScript.gameIsRunning = true;

        LevelManagerScript.ResetAllPoints();
        
        LevelManagerScript.StartRound();

        LevelManagerScript.RemovePlayerControl();
        
        LevelManagerScript.OnGameStart?.Invoke();
        
        // Wait for the Setup time to pass before starting the round.
        StartCoroutine(WaitForSetupTime());
        
    }
    
    IEnumerator WaitForSetupTime()
    {
        //Debug.Log("Waiting for setup time.");
        yield return new WaitForSeconds(LevelManagerScript.setupTime);
        LevelSM.ChangeState(LevelManagerScript.GetComponent<InRoundState>());
        LevelManagerScript.ReturnPlayerControl();
       //Debug.Log("Setup time passed. First round starting.");
    }
}
