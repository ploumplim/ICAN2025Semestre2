using UnityEngine;
using System.Collections;
using Eflatun.SceneReference;
using UnityEngine.SceneManagement;

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
            LevelLoader();
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

    public void LevelLoader()
    {
        ArenaLoader loader = GameManager.Instance.GetComponent<ArenaLoader>();
        int currentSceneIndex = loader.currentSceneIndex;
        int nextSceneID = currentSceneIndex + 1;

        // Vérifie si l'ID de la prochaine scène est valide
        if (nextSceneID < loader.sceneList.Count)
        {
            // Charge la scène depuis la liste sceneList
            string nextSceneName = loader.sceneList[nextSceneID].Name;
            SceneManager.LoadScene(nextSceneName);

            // Met à jour l'index de la scène actuelle
            loader.currentSceneIndex = nextSceneID;
        }
        else
        {
            Debug.LogWarning("Aucune scène suivante disponible dans la liste ArenaLoader.sceneList.");
        }
    }

}
