using UnityEngine;
using System.Collections;
public class BufferState : LevelState
{
    private bool _finalRound;
    public override void Enter()
    {
        LevelManagerScript.currentRound++;
        _finalRound = LevelManagerScript.RoundCheck();
        if (_finalRound)
        {
            LevelSM.ChangeState(GetComponent<ExitLevelState>());
            // Debug.Log("That was the final round. Changing state to ExitLevelState.");
        }
        else
        {
            //Remove control from players
            LevelManagerScript.RemovePlayerControl();
            LevelManagerScript.StartRound();
            StartCoroutine(WaitForBufferTime());
        }
    }
    
    IEnumerator WaitForBufferTime()
    {
        yield return new WaitForSeconds(LevelManagerScript.roundBufferTime);
        //Return control to players
        LevelManagerScript.ReturnPlayerControl();
        LevelSM.ChangeState(LevelManagerScript.GetComponent<InRoundState>());
        // Debug.Log("Buffer time passed. Next round starting.");
    }

}
