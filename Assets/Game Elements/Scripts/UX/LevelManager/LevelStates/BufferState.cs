using UnityEngine;
using System.Collections;
using Eflatun.SceneReference;
using UnityEngine.SceneManagement;

public class BufferState : LevelState
{
    private bool _finalRound;
    public override void Enter()
    {
        Debug.Log("Fin de Round " + LevelManagerScript.currentRound);   
        LevelManagerScript.currentRound++;
        _finalRound = LevelManagerScript.RoundCheck();
        if (_finalRound)
        {
            LevelSM.ChangeState(GetComponent<ExitLevelState>());
        }
        else
        {
            //Remove control from players
            LevelManagerScript.RemovePlayerControl();
            GameManager.Instance.LevelLoader();
            LevelManagerScript.StartRound();
            StartCoroutine(WaitForBufferTime());
        }
        foreach (var player in GameManager.Instance.PlayerScriptList)
        {
            GameObject playerScorePanel = player.GetComponent<PlayerScript>().playerScorePanel;
            int points = player.GetComponent<PlayerPointTracker>().points;
            
            GameManager.Instance.levelManager.ingameGUIManager.UpdatePlayerScore(playerScorePanel, points);
        }
        

    }
    
    IEnumerator WaitForBufferTime()
    {
        yield return new WaitForSeconds(LevelManagerScript.roundBufferTime);
        //Return control to players
        
        LevelManagerScript.ReturnPlayerControl();
        LevelSM.ChangeState(LevelManagerScript.GetComponent<InRoundState>());
        
    }

    

}
