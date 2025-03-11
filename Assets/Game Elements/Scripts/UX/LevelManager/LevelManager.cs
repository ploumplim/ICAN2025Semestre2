using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelManager : MonoBehaviour
{
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ PRIVATE VARIABLES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private LevelSM _levelSM; // Reference to the Level State Machine
    private LevelState _currentState; // Reference to the current state of the level
    private MultiplayerManager _multiplayerManager; // Reference to the Multiplayer Manager
    [HideInInspector] public List<GameObject> players; // List of players in the level
    [HideInInspector] public GameObject gameBall; // Reference to the game ball
    [HideInInspector] public List<GameObject> pointWalls;
    [HideInInspector] public List<GameObject> neutralWalls;
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ PUBLIC VARIABLES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [Header("Prefab settings")]
    [Tooltip("Insert the ball prefab here to spawn it when the level starts and on" +
             "each subsequent round.")]
    public GameObject ballPrefab;
    [Tooltip("Insert the wall prefab here that will provide points to the player.")]
    public GameObject pointWallPrefab;
    [Tooltip("Insert the neutral wall prefab here, which will not provide points to the player.")]
    public GameObject neutralWallPrefab;
    // --------------------------------------------------------------------------------
    [Header("Rounds Manager")]
    public int currentRound; // The current round of the game
    public int maxRounds; // The maximum number of rounds in the game
    
    //--------------------------------------------------------------------------------
    [Header("Level Manager Lists")]
    [Tooltip("This list holds all the positions of the point walls.")]
    public List<Position> pointWallPositions;
    [Tooltip("This list holds all the positions of the neutral walls.")]
    public List<Position> neutralWallPositions;
    
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
    // ------------------------ MANAGE ROUNDS 🃦 🃧 🃨 🃩  ------------------------

    public void RoundCheck()
    {
        currentRound += 1;
        
        // If the current round is greater than the maximum rounds, end the game.
        if (currentRound < maxRounds)
        {
            _levelSM.ChangeState(GetComponent<InRoundState>());

        }
        else
        {
            _levelSM.ChangeState(GetComponent<ExitLevelState>());
        }
    }
    
    // ------------------------ MANAGE BALL ♚♛  ------------------------
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
            // Change the gameBall's object name
            gameBall.name = "Ball";
            
        }
    }
    
    // ------------------------ MANAGE WALLS ♜♜  ------------------------
    public void SpawnPointWall(Vector3 position)
    {
        if (pointWallPrefab)
        {
            Instantiate(pointWallPrefab, position, Quaternion.identity);
            // Add the point wall to the list of point walls
            pointWalls.Add(pointWallPrefab);
        }
    }
    
    public void SpawnNeutralWall(Vector3 position)
    {
        if (neutralWallPrefab)
        {
            Instantiate(neutralWallPrefab, position, Quaternion.identity);
            // Add the neutral wall to the list of neutral walls
            neutralWalls.Add(neutralWallPrefab);
        }
    }
    public void DestroyAllPointWalls()
    {
        foreach (GameObject wall in pointWalls)
        {
            Destroy(wall);
        }
    }
    
    public void DestroyAllNeutralWalls()
    {
        foreach (GameObject wall in neutralWalls)
        {
            Destroy(wall);
        }
    }
    
}
