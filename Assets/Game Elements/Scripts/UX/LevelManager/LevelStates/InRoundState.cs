using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

public class InRoundState : LevelState
{
    private List<GameObject> _playersAlive;
    [HideInInspector] public GameObject winningPlayer;
    private bool _roundEnded;
    public override void Enter()
    {
        winningPlayer = null;
        _playersAlive = new List<GameObject>();
        foreach (GameObject player in LevelManagerScript.players)
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
        
        foreach (GameObject player in LevelManagerScript.players)
        {
            PlayerScript playerScript = player.GetComponent<PlayerScript>();
            
            if (playerScript.currentState == player.GetComponent<DeadState>())
            {
                // Remove the player from the list of players that are alive.
                _playersAlive.Remove(player);
            }
        }

        var levelManager = GameManager.Instance.levelManager;
        
        foreach (var pointTrackerList in levelManager.PointTrackers)
        {
            if (pointTrackerList._points == levelManager.pointNeededToWin)
            {
                _roundEnded = true;
                
                PlayerScript topPlayer = null;
                int highestPoints = int.MinValue;

                foreach (var player in GameManager.Instance.PlayerScriptList)
                {
                    if (player.playerPoint > highestPoints)
                    {
                        highestPoints = player.playerPoint;
                        topPlayer = player;
                    }
                }

                if (topPlayer != null)
                {
                    winningPlayer = topPlayer.gameObject;
                    LevelManagerScript.EndRound(winningPlayer);
                }
                Debug.Log(topPlayer.name + " won");
                Rigidbody ballrb = LevelManagerScript.gameBall.GetComponent<BallSM>().rb;
                ballrb.linearVelocity = Vector3.zero;
                StartCoroutine(VictoryDelay());
            }
        }
        
    }

    IEnumerator VictoryDelay()
    {
        yield return new WaitForSeconds(LevelManagerScript.roundVictoryDelay);
        
        LevelSM.ChangeState(LevelManagerScript.GetComponent<BufferState>());
        
    }
    
    public override void Exit()
    {
        //Debug.Log("Exiting InRoundState.");
        LevelManagerScript.OnRoundEnded?.Invoke(winningPlayer.name);
        // Reset the list of players that are alive.
        _playersAlive = null;
        _roundEnded = false;
    }
    
}
