using UnityEngine;
using System.Collections;
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
            
            Debug.Log("That was the final round. Changing state to ExitLevelState.");
        }
        else
        {
            StartCoroutine(WaitForBufferTime());
            LevelManager.SpawnBall();
        }
    }
    
    IEnumerator WaitForBufferTime()
    {
        Debug.Log("Waiting for buffer time.");
        yield return new WaitForSeconds(LevelManager.roundBufferTime);
        LevelSM.ChangeState(LevelManager.GetComponent<InRoundState>());
        Debug.Log("Buffer time passed. Next round starting.");
    }

}
