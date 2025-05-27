using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

                if (topPlayer != null) Debug.Log(topPlayer.name + " won");

                foreach (var goals in levelManager.PointTrackers)
                {
                    goals._points = 0;
                    for (int i = 0; i < goals.transform.childCount; i++)
                    {
                        GameObject child = goals.transform.GetChild(i).gameObject;
                        var tmp = child.GetComponent<TextMeshPro>();
                        if (tmp != null)
                        {
                            tmp.text = goals._points.ToString();
                        }
                    }
                }
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
