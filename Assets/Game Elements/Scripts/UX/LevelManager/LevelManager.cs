using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
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
    [HideInInspector] public List<GameObject> players; // List of players in the level
    private List<Transform> _playerSpawnPoints; // List of player spawn points
    [FormerlySerializedAs("globalScore")] [HideInInspector] public int potScore; // Global score of the level
    [HideInInspector] public GameObject gameBall; // Reference to the game ball
    [HideInInspector] public List<GameObject> pointWalls; // List of point walls
    [HideInInspector] public List<GameObject> neutralWalls; // List of neutral walls
    [HideInInspector] public int currentRound; // Current round of the level
    [HideInInspector] public int totalRounds; // Total rounds of the level
    [HideInInspector] public bool gameIsRunning; // Boolean to check if the game is running
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
    
    [Tooltip("This float value determines the time it takes to set up the level before the first round.")]
    public float setupTime = 1.5f;
    
    [Tooltip("This float value determines the time it takes to buffer between rounds.")]
    public float roundBufferTime = 1.5f;
    
    [Tooltip("This list holds all the rounds in the level. Modify the level design of each round here.")]
    public List<Round> rounds;

    //--------------------------------------------------------------------------------
    [Header("Score Settings")]
    [Tooltip("This int value determines the score the player gets when they hit the flying ball.")]
    public int ballHitScore = 1;
    [Tooltip("This int value determines the score the player gets when they hit the bunted ball.")]
    public int buntedBallHitScore = 2;
    [Tooltip("This int value determines the score the player gets when they hit the point wall.")]
    public int pointWallHitScore = 3;
    
    //--------------------------------------------------------------------------------
    [Header("UX Manager")]
    public IngameGUIManager ingameGUIManager;
    [SerializeField]private MultiplayerManager multiplayerManager; // Reference to the Multiplayer Manager


    
    
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ EVENTS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ METHODS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    public void Start()
    {
        Initialize(); // Delete this when Game State machine is implemented.
    }

    // Call initialize to set up the level manager.
    public void Initialize()
    {
        if (multiplayerManager)
        {
            _playerSpawnPoints = multiplayerManager.spawnPoints;
        }
        
        _levelSM = GetComponent<LevelSM>();
        _levelSM.Init();
        totalRounds = rounds.Count;
    }

    public void Update()
    {
        // Update the current state of the level
        if (_levelSM && _levelSM.currentState)
        {
            _currentState = _levelSM.currentState;
        }

        // Add players to the scene if outside of level.
        if (multiplayerManager.handleGamePads && _currentState is OutOfLevelState
            && !gameIsRunning)
        {
            multiplayerManager.handleGamePads.CheckGamepadAssignments();
        }
        else if (!gameIsRunning)
        {
            if (!multiplayerManager.handleGamePads)
            {
                Debug.LogWarning("HandleGamePads not found in the scene.");
            }
            if (_currentState is not OutOfLevelState)
            {
                Debug.LogWarning("Current state is not OutOfLevelState.");
            }
        }
        
        
        // Update the player list
        if (multiplayerManager)
        {
            if (players.Count != multiplayerManager.connectedPlayers.Count)
            {
                players = multiplayerManager.connectedPlayers;
            }
        }


    }
    // ------------------------ MANAGE ROUNDS 🃦 🃧 🃨 🃩  ------------------------
    public void StartLevel()
    {
        if (players.Count >= 2 && !gameIsRunning)
        {
            _levelSM.ChangeState(GetComponent<SetupState>());
            // Disable the game start button in the GUI
            ingameGUIManager.startGameButtonObject.SetActive(false);
            
            foreach (GameObject player in players)
            {
                // subscribe to the OnBallHitEvent
                player.GetComponent<PlayerScript>().OnBallHit += AddScoreToPlayer;
            }
        }
        else
        {
            if (players.Count < 2)
            {
                Debug.LogWarning("Not enough players to start the level.");
            }
            if (gameIsRunning)
            {
                Debug.LogWarning("The game is already running.");
            }
        }
    }
    
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
        DestroyAllPointWalls();
        DestroyAllNeutralWalls();
        // Reset the global score
        potScore = 0;
        // Spawn the ball
        SpawnBall();
        // Init the players
        InitPlayers();
        // Spawn the point walls
        SpawnCurrentRoundWalls();
        // Increment the current round
    }
    
    public void EndRound(GameObject winningPlayer)
    {
        // Add the global score to the winning player's individual score
        AddScoreToPlayer(potScore, winningPlayer);
    }
    
    // ------------------------ MANAGE BALL ♚♛  ------------------------
    public void SpawnBall()
    {
        if (ballPrefab)
        {
            // Destroy the ball if it exists
            
            GameObject existingBall = GameObject.FindWithTag("Ball");
            
            if (existingBall)
            {
                // unsuscribe to all the events
                existingBall.GetComponent<BallSM>().pointWallHit.RemoveAllListeners();
                Destroy(GameObject.FindWithTag("Ball"));
            }
            
            // Instantiate the ball prefab
            gameBall = Instantiate(ballPrefab, ballSpawnPosition.position, Quaternion.identity);
            // Change the gameBall's object name
            gameBall.name = "Ball";
            // Assign the ball to the GUI
            ingameGUIManager.AssignBall(gameBall);
            // Assign the ball value.
            gameBall.GetComponent<BallSM>().pointWallPoints = pointWallHitScore;
            //suscribe to the OnPointWallHit event
            gameBall.GetComponent<BallSM>().pointWallHit.AddListener(AddGlobalScore);
        }
    }
    
    // ------------------------ MANAGE PLAYERS ♟♞  ------------------------
    
    public void InitPlayers()
    {
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerScript>().ChangeState(player.GetComponent<NeutralState>());
        }
        // Put players in the correct spawn point.
        for (int i = 0; i < players.Count; i++)
        {
            players[i].transform.position = _playerSpawnPoints[i].position;
        }
    }
    
    public void RemovePlayerControl()
    {
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerInput>().DeactivateInput();
        }
    }
    
    public void ReturnPlayerControl()
    {
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerInput>().ActivateInput();
        }
    }
    
    // ------------------------ MANAGE WALLS ♜♜  ------------------------
    
    public void SpawnCurrentRoundWalls()
    {
        // Spawn the point walls
        int i = 0;
        int j = 0;
        foreach (Transform pointWallPosition in rounds[currentRound].pointWallPositions)
        {
            SpawnPointWall(pointWallPosition.position);
            // Make it a child of the pointWallPosition's game object.
            pointWalls[i].transform.parent = pointWallPosition;
            i++;
        }
        // Spawn the neutral walls
        foreach (Transform neutralWallPosition in rounds[currentRound].neutralWallPositions)
        {
            SpawnNeutralWall(neutralWallPosition.position);
            // Make it a child of the neutralWallPosition's game object.
            neutralWalls[j].transform.parent = neutralWallPosition;
            j++;
        }
    }
    
    public void SpawnPointWall(Vector3 position)
    {
        if (pointWallPrefab)
        {
            GameObject pointWall = Instantiate(pointWallPrefab, position, Quaternion.identity);
            // Add the point wall to the list of point walls
            pointWalls.Add(pointWall);
        }
    }
    
    public void SpawnNeutralWall(Vector3 position)
    {
        if (neutralWallPrefab)
        {
            GameObject neutralWall = Instantiate(neutralWallPrefab, position, Quaternion.identity);
            // Add the neutral wall to the list of neutral walls
            neutralWalls.Add(neutralWall);
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
        potScore += score;
    }
    public void AddScoreToPlayer(int score, GameObject player, BallState ballState = null)
    {
        if (!ballState)
        {
            player.GetComponent<PlayerPointTracker>().AddPoints(score);
        }
        else
        {
            switch (ballState)
            {
                case FlyingState:
                    player.GetComponent<PlayerPointTracker>().AddPoints(ballHitScore);
                    break;
                case BuntedBallState:
                    player.GetComponent<PlayerPointTracker>().AddPoints(buntedBallHitScore);
                    break;
                
            }
        }
    }
    
    public void ResetAllPoints()
    {
        potScore = 0;
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerPointTracker>().ResetPoints();
        }
    }
}
