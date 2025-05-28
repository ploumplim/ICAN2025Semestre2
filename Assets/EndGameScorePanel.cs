using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EndGameScorePanel : MonoBehaviour
{
    public GameObject EndGameScorePanelGO;

    public Dictionary<PlayerScript, int> playerData;
    
    public void StartEndGamePanel()
    {
        playerData = GameManager.Instance.PlayerScriptList
            .ToDictionary(player => player, player => player.playerPoint);

        // foreach (var playerEntry in playerData)
        // {
        //     Debug.Log($"Player: {playerEntry.Key.name}, Points: {playerEntry.Value}");
        // }

        ShowEndGameScorePanel();
    }

    private void ShowEndGameScorePanel()
    {
        EndGameScorePanelGO.SetActive(true);
    }
}