using System;
using System.Collections.Generic;
using UnityEngine;

public class InRoundState : LevelState
{
    private List<GameObject> _playersAlive;
    [HideInInspector] public GameObject winningPlayer;
    public override void Enter()
    {
        winningPlayer = null;
        _playersAlive = new List<GameObject>();
        foreach (GameObject player in LevelManager.players)
        {
            // Add the player to the list of players that are alive.
            _playersAlive.Add(player);
        }
    }


    public override void Tick()
    {
        // Check the state of every player.
        // If all players except for one are dead, then the round is over.
        
        foreach (GameObject player in LevelManager.players)
        {
            PlayerScript playerScript = player.GetComponent<PlayerScript>();
            if (playerScript.currentState == player.GetComponent<DeadState>())
            {
                // Remove the player from the list of players that are alive.
                _playersAlive.Remove(player);
            }
        }
        
        if (_playersAlive.Count == 1)
        {
            // If there is only one player left, then the round is over.
            // Change the state to BufferState.
            winningPlayer = _playersAlive[0];
            LevelSM.ChangeState(LevelManager.GetComponent<BufferState>());
        }
        
    }
    
    public override void Exit()
    {
        LevelManager.EndRound(winningPlayer);
        // Reset the list of players that are alive.
        _playersAlive = null;
    }
    
}
