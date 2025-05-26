using System;
using System.Collections;
using System.Collections.Generic;
using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class LevelManager : MonoBehaviour
{
    
    #region Variables
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ PRIVATE VARIABLES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private LevelSM _levelSM; // Reference to the Level State Machine
    [FormerlySerializedAs("_currentState")] public LevelState currentState; // Reference to the current state of the level
    public List<GameObject> players; // List of players in the level
    public List<Transform> _playerSpawnPoints; // List of player spawn points
    [FormerlySerializedAs("globalScore")] [HideInInspector] public int potScore; // Global score of the level
    public GameObject gameBall; // Reference to the game ball
    [HideInInspector] public int currentRound; // Current round of the level
    [HideInInspector] public int totalRounds; // Total rounds of the level
    public bool gameIsRunning; // Boolean to check if the game is running

    
    
    public int pointNeededToWin;
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ PUBLIC VARIABLES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [Header("Prefab settings")]
    [Tooltip("Insert the ball prefab here to spawn it when the level starts and on" +
             "each subsequent round.")]
    public GameObject ballPrefab;
    [Tooltip("Insert the ball spawn position here (empty game object with transform)")]
    public Transform ballSpawnPosition;
    
    public List<PointTracker> PointTrackers;
    public List<GameObject> GoalList;
    public GoalSpawner goalSpawner;

    public GameObject centerPoint;
    // [Tooltip("Insert the wall prefab here that will provide points to the player.")]
    // public GameObject pointWallPrefab;
    // [Tooltip("Insert the neutral wall prefab here, which will not provide points to the player.")]
    // public GameObject neutralWallPrefab;

    //--------------------------------------------------------------------------------
    [Header("Round Manager")]
    
    [Tooltip("This float value determines the time it takes to set up the level before the first round.")]
    public float setupTime = 1.5f;
    
    [Tooltip("This float value determines the time it takes to buffer between rounds.")]
    public float roundBufferTime = 1.5f;
    
    [Tooltip("This value represents the delay after the last player is dead during a round, before the buffer" +
             "is called.")]
    public float roundVictoryDelay = 2f;

    public List<SceneReference> levels;

    //--------------------------------------------------------------------------------
    [Header("Score Settings")]
    // [Tooltip("This int value determines the score the player gets when they hit the flying ball.")]
    // public int ballHitScore = 1;
    // [Tooltip("This int value determines the score the player gets when they hit the bunted ball.")]
    // public int buntedBallHitScore = 2;
    [Tooltip("This int value determines the score the player gets when they hit the point wall.")]
    public int pointWallHitScore = 3;
    
    //--------------------------------------------------------------------------------
    [Header("UX Manager")]
    public IngameGUIManager ingameGUIManager;
    public CameraScript gameCameraScript;
    //[SerializeField]public MultiplayerManager multiplayerManager; // Reference to the Multiplayer Manager
    [FormerlySerializedAs("PlayerSpawnPoint")] public GameObject PlayerSpawnParent;
    
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ EVENTS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public UnityEvent OnGameStart;
    public UnityEvent<string> OnGameEnd;
    public UnityEvent<int> OnRoundStarted;
    public UnityEvent<string> OnRoundEnded;
    public UnityEvent OnPlayerSpawn;

    #endregion
    
   

    private void Awake()
    {
        GameManager.Instance.levelManager = this;
        foreach (var player in GameManager.Instance.PlayerScriptList)
        {
            players.Add(player.gameObject);
        }
        gameBall = GameObject.FindGameObjectWithTag("Ball");
        // _levelSM= GetComponent<LevelSM>();
        // _levelSM.levelManager = this;
        // GameManager.Instance.GetComponent<PlayingState>().levelSM = _levelSM;
        
        
        gameBall.transform.position = ballSpawnPosition.position;


    }
    

    public void LevelManagerSetup()
    {
        _playerSpawnPoints.Clear();
        PlayerSpawnParent = GameObject.FindGameObjectWithTag("SpawnParent");
        
    }
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ METHODS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    
    // Call initialize to set up the level manager.
    public void Initialize()
    {
        _levelSM = GetComponent<LevelSM>();
        _levelSM.Init();
        
    }

    public void Update()
    {
        // Update the current state of the level
        if (_levelSM && _levelSM.currentState)
        {
            currentState = _levelSM.currentState;
        }

        // Add players to the scene if outside of level.
        if (GameManager.Instance.handleGamePads)
        {
            
            if (currentState is OutOfLevelState && !gameIsRunning)
            {
                
                GameManager.Instance.handleGamePads.CheckGamepadAssignments();
            }
            else if (!gameIsRunning)
            {
                if (!GameManager.Instance.handleGamePads)
                {
                    //Debug.LogWarning("HandleGamePads not found in the scene.");
                }
                if (currentState is not OutOfLevelState)
                {
                    //Debug.LogWarning("Current state is not OutOfLevelState.");
                }
            }
        }
        else
        {
            Debug.LogWarning("MultiplayerManager or HandleGamePads is null.");
        }

        // Update the player list
        if (GameManager.Instance.multiplayerManager)
        {
            if (players.Count != GameManager.Instance.multiplayerManager.connectedPlayers.Count)
            {
                players = GameManager.Instance.multiplayerManager.connectedPlayers;
            }
        }
    }
    // ------------------------ MANAGE ROUNDS 🃦 🃧 🃨 🃩  ------------------------
    // ReSharper disable Unity.PerformanceAnalysis
    public void StartLevel() // CALL THIS METHOD TO START THE LEVEL
    {
        if (players.Count >= 2 && !gameIsRunning)
        {
            _levelSM.ChangeState(GetComponent<SetupState>());
            // Disable the game start button in the GUI
            // ingameGUIManager.startGameButtonObject.SetActive(false);
            // ingameGUIManager.resetPlayersObject.SetActive(false);
            
            // Check how many players have joined the game and determine the number of rounds depending on that.
            if (players.Count == 2)
            {
                totalRounds = 3;
            }
            else if (players.Count == 3)
            {
                totalRounds = 5;
            }
            else if (players.Count >= 4)
            {
                totalRounds = 7;
            }
            
            foreach (var panel in ingameGUIManager.UI_PlayerScore)
            {
                Destroy(panel.gameObject); // Détruit chaque enfant
            }
            ingameGUIManager.UI_PlayerScore.Clear();
            GameManager.Instance.NextSceneToPlay.Clear();
            GameManager.Instance.NextSceneToPlay = new List<SceneReference>(GameManager.Instance.scenesToLoad);
            ingameGUIManager.StopBlinking();
            ingameGUIManager.UI_PressStartTutorialtext.SetActive(false);
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

        foreach (var players in players)
        {
            players.GetComponent<PlayerScript>().isReady = false;
        }
        
    }
    
    public bool RoundCheck()
    {
        if (currentRound < totalRounds)
        {
            // Check all player's points. If there is a player with 3 points, return true.
            foreach (GameObject player in players)
            {
                if (player.GetComponent<PlayerPointTracker>().points >= GameManager.Instance.PlayerScriptList.Count+2)
                {
                    return true;
                }
            }
            // If no player has 3 points, return false.
            return false;
        }
        else
        {
            return true;
        }
    }
    
    public void StartRound()
    {
        SpawnBall();
        // Init the players
        InitPlayers();
        // Spawn the point walls
        //GameManager.Instance.LoadNextLevel();
        
    }
    
    public void EndRound(GameObject winningPlayer)
    {
        // Add the global score to the winning player's individual score
        winningPlayer.GetComponent<PlayerPointTracker>().AddPoints(1);
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
            // //suscribe to the OnPointWallHit event
            // gameBall.GetComponent<BallSM>().pointWallHit.AddListener(AddGlobalScore);
            // Add the ball to the list of lockpoints in the camera script.
            gameCameraScript.AddObjectToArray(gameBall);
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
    
    // public void SpawnCurrentRoundWalls()
    // {
    //     // Spawn the point walls
    //     int i = 0;
    //     int j = 0;
    //     foreach (Transform pointWallPosition in rounds[currentRound].pointWallPositions)
    //     {
    //         SpawnPointWall(pointWallPosition.position);
    //         
    //         // Make it a child of the pointWallPosition's game object.
    //
    //         if (pointWalls[i])
    //         {
    //             pointWalls[i].transform.parent = pointWallPosition;
    //         }
    //
    //         i++;
    //     }
    //     // Spawn the neutral walls
    //     foreach (Transform neutralWallPosition in rounds[currentRound].neutralWallPositions)
    //     {
    //         SpawnNeutralWall(neutralWallPosition.position);
    //         // Make it a child of the neutralWallPosition's game object.
    //         if (neutralWalls[j])
    //         {
    //             neutralWalls[j].transform.parent = neutralWallPosition;
    //         }
    //         j++;
    //     }
    // }
    
    // public void SpawnPointWall(Vector3 position)
    // {
    //     if (pointWallPrefab)
    //     {
    //         GameObject pointWall = Instantiate(pointWallPrefab, position, Quaternion.identity);
    //         // Add the point wall to the list of point walls
    //         pointWalls.Add(pointWall);
    //     }
    // }
    //
    // public void SpawnNeutralWall(Vector3 position)
    // {
    //     if (neutralWallPrefab)
    //     {
    //         GameObject neutralWall = Instantiate(neutralWallPrefab, position, Quaternion.identity);
    //         // Add the neutral wall to the list of neutral walls
    //         neutralWalls.Add(neutralWall);
    //     }
    // }
    // public void DestroyAllPointWalls()
    // {
    //     foreach (GameObject wall in pointWalls)
    //     {
    //         Destroy(wall);
    //     }
    // }
    //
    // public void DestroyAllNeutralWalls()
    // {
    //     foreach (GameObject wall in neutralWalls)
    //     {
    //         Destroy(wall);
    //     }
    // }
    // ------------------------ MANAGE SCORES 🏆🏆  ------------------------
    // public void AddGlobalScore(int score)
    // {
    //     potScore += score;
    // }
    public void AddScoreToPlayer(int score, GameObject player)
    {
    }

    public void EndGameScore()
    {
       //ingameGUIManager.gameObject.GetComponent<EndGameScorePanel>().StartEndGamePanel();
        
       
       // Debug.Log("HELP");
       
       // List<(GameObject player, int score)> playerScores = new List<(GameObject player, int score)>();
        //
        // foreach (var player in players)
        // {
        //     int score = player.GetComponent<PlayerPointTracker>().points;
        //     playerScores.Add((player, score));
        // }
        //
        // playerScores.Sort((x, y) => y.score.CompareTo(x.score));
        //
        // for (int i = 0; i < playerScores.Count; i++)
        // {
        //     var playerScore = playerScores[i];
        //     // Debug.Log($"Player: {playerScore.player.name}, Score: {playerScore.score}");
        //     GameObject playerScorePanelParent = null;
        //     foreach (Transform child in ingameGUIManager.transform)
        //     {
        //         if (child.CompareTag("ScorePlayerPanel"))
        //         {
        //             playerScorePanelParent = child.gameObject;
        //         }
        //     }
        //     GameObject scorePanel = Instantiate(ingameGUIManager.ScorePlayerUIEndGame, playerScorePanelParent.gameObject.transform);
        //     ingameGUIManager.StartBlinking(ingameGUIManager.UI_SetReadyInformationText,ingameGUIManager.blinkInterval);
        //     ingameGUIManager.UI_PlayerScore.Add(scorePanel);
        //     scorePanel.SetActive(true);
        //     //ingameGUIManager.EndGameScoreBoardPlayerPanel(playerScore.player, scorePanel, i + 1);
        // }
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
