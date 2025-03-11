using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class LevelManager : MonoBehaviour
{
    [Serializable]
    public class Round
    {
        public string roundName;
        [Tooltip("This list holds all the positions of the point walls.")]
        public List<Transform> pointWallPositions;
        [Tooltip("This list holds all the positions of the neutral walls.")]
        public List<Transform> neutralWallPositions;

    }
    
    
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ PRIVATE VARIABLES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private LevelSM _levelSM; // Reference to the Level State Machine
    private LevelState _currentState; // Reference to the current state of the level
    private MultiplayerManager _multiplayerManager; // Reference to the Multiplayer Manager
    [HideInInspector] public List<GameObject> players; // List of players in the level
    [HideInInspector] public int globalScore; // Global score of the level
    [HideInInspector] public GameObject gameBall; // Reference to the game ball
    [HideInInspector] public List<GameObject> pointWalls; // List of point walls
    [HideInInspector] public List<GameObject> neutralWalls; // List of neutral walls
    [HideInInspector] public int currentRound;
    [HideInInspector] public int totalRounds;
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ PUBLIC VARIABLES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [Header("Prefab settings")]
    [Tooltip("Insert the ball prefab here to spawn it when the level starts and on" +
             "each subsequent round.")]
    public GameObject ballPrefab;
    [Tooltip("Insert the ball spawn position here (empty game object with transform)")]
    public Transform ballSpawnPosition;
    [Tooltip("Insert the wall prefab here that will provide points to the player.")]
    public GameObject pointWallPrefab;
    [Tooltip("Insert the neutral wall prefab here, which will not provide points to the player.")]
    public GameObject neutralWallPrefab;

    //--------------------------------------------------------------------------------
    [Header("Round Manager")]
    [Tooltip("This list holds all the rounds in the level.")]
    public List<Round> rounds;

    //--------------------------------------------------------------------------------
    [Header("Ingame GUI Manager")]
    public IngameGUIManager ingameGUIManager;

    
    
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ EVENTS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ METHODS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    
    // Call initialize when level is starting.
    
    public void Initialize()
    {
        _multiplayerManager = FindObjectOfType<MultiplayerManager>();
        // Using the multiplayer manager, fill my list of players with the players in the scene.
        if (_multiplayerManager)
        {
            if (players.Count != _multiplayerManager.connectedPlayers.Count)
            {
                players = _multiplayerManager.connectedPlayers;
            }

            _levelSM = GetComponent<LevelSM>();
            _levelSM.Init();
            
        }
        else
        {
            Debug.LogError("Multiplayer Manager not found in the scene.");
        }
        
        totalRounds = rounds.Count;
    }

    public void Update()
    {
        if (_levelSM && _levelSM.currentState)
        {
            _currentState = _levelSM.currentState;
        }

        if (_multiplayerManager)
        {
            if (players.Count != _multiplayerManager.connectedPlayers.Count)
            {
                players = _multiplayerManager.connectedPlayers;
            }
        }


    }
    // ------------------------ MANAGE ROUNDS 🃦 🃧 🃨 🃩  ------------------------

    public bool RoundCheck()
    {
        // Check if the current round is less than the total rounds.
        // If it is, increment the current round and change the state to InRoundState.
        // If it is not, change the state to OutOfLevelState.
        if (currentRound < totalRounds)
        {
            currentRound++;
            return false;
        }
        else
        {
            return true;
        }
    }
    
    public void StartRound()
    {
        // Increment the current round
        currentRound++;
        // Reset the global score
        globalScore = 0;
        // Spawn the ball
        SpawnBall();
        // Spawn the point walls
        foreach (Transform pointWallPosition in rounds[currentRound].pointWallPositions)
        {
            SpawnPointWall(pointWallPosition.position);
        }
        // Spawn the neutral walls
        foreach (Transform neutralWallPosition in rounds[currentRound].neutralWallPositions)
        {
            SpawnNeutralWall(neutralWallPosition.position);
        }
    }
    
    public void EndRound(GameObject winningPlayer)
    {
        // Add the global score to the winning player's individual score
        AddScoreToPlayer(globalScore, winningPlayer);
        // Destroy all point walls
        DestroyAllPointWalls();
        // Destroy all neutral walls
        DestroyAllNeutralWalls();
        // Destroy the ball
        Destroy(gameBall);
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
            
            // Instantiate the ball prefab
            gameBall = Instantiate(ballPrefab, ballSpawnPosition.position, Quaternion.identity);
            // Change the gameBall's object name
            gameBall.name = "Ball";

            
        }
    }
    
    // ------------------------ MANAGE PLAYERS ♟♞  ------------------------
    
    public void InitPlayers()
    {
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerScript>().ChangeState(player.GetComponent<NeutralState>());
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
    // ------------------------ MANAGE SCORES 🏆🏆  ------------------------
    public void AddGlobalScore(int score)
    {
        globalScore += score;
    }
    public void AddScoreToPlayer(int score, GameObject player)
    {
        player.GetComponent<PlayerPointTracker>().AddPoints(score);
    }
    
    public void ResetAllPoints()
    {
        globalScore = 0;
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerPointTracker>().ResetPoints();
        }
    }
}
