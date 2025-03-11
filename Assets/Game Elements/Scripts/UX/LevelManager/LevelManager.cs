using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ PRIVATE VARIABLES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private LevelSM _levelSM; // Reference to the Level State Machine
    private LevelState _currentState; // Reference to the current state of the level
    private MultiplayerManager _multiplayerManager; // Reference to the Multiplayer Manager
    [HideInInspector] public List<GameObject> players; // List of players in the level
    [HideInInspector] public GameObject gameBall; // Reference to the game ball
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ PUBLIC VARIABLES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [Header("Level Manager Settings")]
    [Tooltip("Insert the ball prefab here to spawn it when the level starts and on" +
             "each subsequent round.")]
    public GameObject ballPrefab;
    [Tooltip("Insert the wall prefab here that will provide points to the player.")]
    public GameObject pointWallPrefab;
    [Tooltip("Insert the neutral wall prefab here, which will not provide points to the player.")]
    public GameObject neutralWallPrefab;
    
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ EVENTS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ METHODS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    
    // Call initialize when level is starting.
    
    public void Initialize()
    {
        _levelSM = GetComponent<LevelSM>();
        _levelSM.Init(); 
    }

    public void Update()
    {
        if (_levelSM && _levelSM.currentState)
        {
            _currentState = _levelSM.currentState;
        }
    }
    // ------------------------ MANAGE ENTITIES ♚ ♛  ------------------------
    public void SpawnBall()
    {
        if (ballPrefab)
        {
            // Destroy the ball if it exists
            if (GameObject.FindWithTag("Ball"))
            {
                Destroy(GameObject.FindWithTag("Ball"));
            }
            
            gameBall = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
        }
    }
    
}
