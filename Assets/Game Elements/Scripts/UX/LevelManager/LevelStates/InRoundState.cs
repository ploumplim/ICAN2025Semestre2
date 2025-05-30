using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

public class InRoundState : LevelState
{
    private List<GameObject> _playersAlive;
    public LevelManager levelManager;
    public override void Enter()
    {
        GameManager.Instance.levelManager.winningPlayer = null;
        _playersAlive = new List<GameObject>();
        foreach (GameObject player in LevelManagerScript.playersList)
        {
            // Add the player to the list of players that are alive.
            _playersAlive.Add(player);
            // Debug.Log("Player added to the list of players that are alive." + player.name);
        }
        
        LevelManagerScript.OnRoundStarted?.Invoke(LevelManagerScript.currentRound);
    }


    public override void Tick()
    {
        // Check the state of every player.
        // If all players except for one are dead, then the round is over.
        
        foreach (GameObject player in LevelManagerScript.playersList)
        {
            PlayerScript playerScript = player.GetComponent<PlayerScript>();
            
            if (playerScript.currentState == player.GetComponent<DeadState>())
            {
                // Remove the player from the list of players that are alive.
                _playersAlive.Remove(player);
            }
        }

        levelManager = GameManager.Instance.levelManager;
        
        foreach (var pointTrackerList in levelManager.PointTrackers)
        {
            if (pointTrackerList._points == levelManager.pointNeededToWin)
            {
                SetWinningPlayer();
                pointTrackerList._points = 0;
                GameManager.Instance.levelManager.gameCameraScript.RemoveObjectFromArray(levelManager.gameBall);
                Destroy(levelManager.gameBall);
                StartCoroutine(VictoryDelay());
            }
        }
        
    }

    public void SetWinningPlayer()
    {
    
                levelManager.winningPlayer = null;
                
                // Trouver le joueur avec le plus de playerGlobalPoint
                levelManager.winningPlayer = GameManager.Instance.PlayerScriptList
                    .OrderByDescending(player => player.playerGlobalPoint)
                    .FirstOrDefault();
    
                if ( levelManager.winningPlayer != null)
                {
                    Debug.Log( levelManager.winningPlayer.name + " a le plus de points globaux : " +  levelManager.winningPlayer.playerGlobalPoint);
                    LevelManagerScript.EndRound(GameManager.Instance.levelManager.winningPlayer);
                }
                
                Rigidbody ballrb = LevelManagerScript.gameBall.GetComponent<BallSM>().rb;
                ballrb.linearVelocity = Vector3.zero;
    }

    IEnumerator VictoryDelay()
    {
        
        yield return new WaitForSeconds(LevelManagerScript.roundVictoryDelay);
        
        LevelSM.ChangeState(LevelManagerScript.GetComponent<BufferState>());
        
    }
    
    public override void Exit()
    {
        
        LevelManagerScript.OnRoundEnded?.Invoke(GameManager.Instance.levelManager.winningPlayer);
        // Reset the list of players that are alive.
        _playersAlive = null;
    }
    
}
