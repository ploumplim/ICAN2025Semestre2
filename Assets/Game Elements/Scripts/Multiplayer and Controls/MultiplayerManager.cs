using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;

public class MultiplayerManager : MonoBehaviour
{
    public LevelManager levelManager;
    public GameObject playerToConnect;
    // public List<GameObject> availablePlayers = new List<GameObject>();
    public List<GameObject> connectedPlayers = new List<GameObject>(); // Liste des joueurs déjà associés
    public GameObject ChargeVisualObject;
    [FormerlySerializedAs("ParryTimeVisual")]
    public GameObject HitTimeVisual;
    public GameObject playerPrefab;
    public new CameraScript camera;
    public int playerCount;


    public event Action OnPlayerJoin;

    void Awake()
    { 
    DontDestroyOnLoad(this);
    GameManager.Instance.multiplayerManager = this;
    }

    public void SetGameParameters()
    {
        camera = GameManager.Instance.levelManager.gameCameraScript;
    }

    public void PlayerJoin()
    {
        FillSpawnPoints();
        if (GameManager.Instance.handleGamePads)
        {
            // Subscribe to the OnSouthButtonPressed event
            GameManager.Instance.handleGamePads.OnSouthButtonPressed += AtoJoin;
        }
    }

    public void WaitForPlayersReady()
    {
        int readyPlayers = 0;
        
        foreach (GameObject player in connectedPlayers)
        {
            if (player.GetComponent<PlayerScript>()._isReady)
            {
                readyPlayers++;  
            }
        }
        
        if(readyPlayers==connectedPlayers.Count)
        {
            GameManager.Instance.AllPlayersReady();
        }
    }
    

    void FillSpawnPoints()
    {
        if (GameManager.Instance.levelManager)
        {
            foreach (Transform child in GameManager.Instance.levelManager.PlayerSpawnParent.transform)
            {
                GameManager.Instance.levelManager._playerSpawnPoints.Add(child);
                playerCount = GameManager.Instance.levelManager._playerSpawnPoints.Count;
            }
        }
    }

    void AtoJoin(Gamepad gamepad)
    {
       
        if (GameManager.Instance.handleGamePads.AssignedGamepads.Count < playerCount)
        {
                    SpawnNewPlayer(GameManager.Instance.levelManager._playerSpawnPoints[GameManager.Instance.handleGamePads.AssignedGamepads.Count]); // Spawn un joueur à la position correspondante.
                    connectedPlayers.Add(playerToConnect);
                    HandleGamePads.AssignControllerToPlayer(gamepad, playerToConnect); // Assign the gamepad to a player.
                    camera.GetComponent<CameraScript>().AddObjectToArray(playerToConnect.gameObject);
                    AssignValuesToPlayer(playerToConnect);
                    playerToConnect = null;
                    OnPlayerJoin?.Invoke();
        }
    }
    
    // private void OnSelectButtonPressed(Gamepad gamepad)
    // {
    //     Debug.Log($"Gamepad {gamepad.displayName}: Select button pressed");
    //    
    //     foreach (var player in connectedPlayers)
    //     {
    //         var playerInput = player.GetComponent<PlayerInput>();
    //         if (playerInput != null && playerInput.devices.Contains(gamepad))
    //         {
    //             Debug.Log($"Player associated with gamepad {gamepad.displayName}: {player.name}");
    //             break;
    //         }
    //     }
    // }


    private void SpawnNewPlayer(Transform spawnPosition)
    {
        // Instantiate a new player object at a specified position and rotation
        GameObject newPlayer = Instantiate(playerPrefab, spawnPosition.position, Quaternion.identity);
        
        // Change the player's name to include the player's number
        newPlayer.name = $"Player {connectedPlayers.Count + 1}";
        
        playerToConnect = newPlayer;

    }
    
    
    private void AssignValuesToPlayer(GameObject player)
    {
        // PlayerScript playerScript = player.GetComponent<PlayerScript>();
        PlayerVisuals playerVisuals = player.GetComponent<PlayerVisuals>();
        
        // ---- Assign values to the player ----
        
        // ---- Visual values ----
        
        // Generate a random color.
        Color randomColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        
        // Assign the random color to the player's mesh.
        playerVisuals.ChangePlayerColor(randomColor);
        
    }
}
