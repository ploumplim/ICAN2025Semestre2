using UnityEngine;
using System.Collections;
public class BufferState : LevelState
{
    private bool _finalRound;
    public override void Enter()
    {
        LevelManager.currentRound++;
        _finalRound = LevelManager.RoundCheck();
        if (_finalRound)
        {
            LevelSM.ChangeState(GetComponent<ExitLevelState>());
            // Debug.Log("That was the final round. Changing state to ExitLevelState.");
        }
        else
        {
            StartCoroutine(WaitForBufferTime());
            
        }
    }
    
    IEnumerator WaitForBufferTime()
    {
        // Debug.Log("Waiting for buffer time.");
        //Remove control from players
        LevelManager.RemovePlayerControl();
        LevelManager.StartRound();
        yield return new WaitForSeconds(LevelManager.roundBufferTime);
        //Return control to players
        LevelManager.ReturnPlayerControl();
        LevelSM.ChangeState(LevelManager.GetComponent<InRoundState>());
        // Debug.Log("Buffer time passed. Next round starting.");
    }

}
