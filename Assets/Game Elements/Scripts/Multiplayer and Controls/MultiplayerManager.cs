using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;

public class MultiplayerManager : MonoBehaviour
{
    public HandleGamePads handleGamePads;
    public Dictionary<Gamepad, GameObject> controllerToPlayer = new Dictionary<Gamepad, GameObject>();
    public List<Transform> spawnPoints = new List<Transform>();
    public LevelManager levelManager;
    public GameObject playerToConnect;
    // public List<GameObject> availablePlayers = new List<GameObject>();
    public List<GameObject> connectedPlayers = new List<GameObject>(); // Liste des joueurs déjà associés
    public GameObject ChargeVisualObject;
    [FormerlySerializedAs("ParryTimeVisual")]
    public GameObject HitTimeVisual;
    public GameObject playerPrefab;
    [FormerlySerializedAs("spawnObject")] public GameObject spawnParent;
    public new Camera camera;



    void Awake()
    { 
    // // Trouve tous les joueurs dans la scène avec le tag "Player"
    // availablePlayers = GameObject.FindGameObjectsWithTag("Player").ToList();
    levelManager = GetComponentInParent<LevelManager>();
    fillSpawnPoints();
    handleGamePads = FindFirstObjectByType<HandleGamePads>();

    if (handleGamePads)
    {
        //subscribe to the OnSouthButtonPressed event
        handleGamePads.OnSouthButtonPressed += AtoJoin;
    }
    
    }

    void fillSpawnPoints()
    {
        foreach (Transform child in spawnParent.transform)
        {
            spawnPoints.Add(child);
        }
    }

    void AtoJoin()
    {
        if (handleGamePads.AssignedGamepads.Count < spawnPoints.Count)
        {
            foreach (Gamepad gamepad in handleGamePads.PendingGamepads.ToList())
            {
                if (gamepad.buttonSouth.wasReleasedThisFrame)
                {
                    SpawnNewPlayer(spawnPoints[handleGamePads.AssignedGamepads.Count]); // Spawn un joueur à la position correspondante.
                    AssignControllerToPlayer(gamepad); // Assign the gamepad to a player.
                    return; // Évite de
                }
            }
        }
    }

    void Update()
    {
        if (handleGamePads)
        {
            handleGamePads.CheckGamepadAssignments();
        }
        else
        {
            Debug.LogWarning("HandleGamePads not found in the scene.");
        }
    }

    private void SpawnNewPlayer(Transform spawnPosition)
    {
        // Instantiate a new player object at a specified position and rotation
        GameObject newPlayer = Instantiate(playerPrefab, spawnPosition.position, Quaternion.identity);
        
        // Change the player's name to include the player's number
        newPlayer.name = $"Player {connectedPlayers.Count + 1}";
        
        // Add the new player to the list of available players
        // availablePlayers.Add(newPlayer);
        playerToConnect = newPlayer;

        // Debug.Log($"New player spawned at position {spawnPosition}");
    }
    
    
    private void AssignControllerToPlayer(Gamepad gamepad)
    {
        
        // if (availablePlayers.Count == 0)
        // {
        //     Debug.LogWarning("Aucun joueur disponible pour être associé à la manette.");
        //     return;
        // }
        
        if (!playerToConnect)
        {
            Debug.LogWarning("No player to connect to the controller.");
            return;
        }
        
        // // Prend le premier joueur disponible
        // GameObject player = availablePlayers[0];
        //
        // availablePlayers.RemoveAt(0); // Retire ce joueur de la liste des disponibles
        
        GameObject player = playerToConnect;
        playerToConnect = null;
        connectedPlayers.Add(player);
        player.SetActive(true);
        
        // Assign the gamepad to the player's input manager
        PlayerInput playerInput = player.GetComponent<PlayerInput>();
        
        if (playerInput != null)
        {
            playerInput.SwitchCurrentControlScheme(gamepad);
        }
        else
        {
            Debug.LogError("PlayerInput component not found on the player.");
        }
        
        controllerToPlayer[gamepad] = player;
        camera.GetComponent<CameraScript>().AddPlayerToArray(player.gameObject);
        AssignValuesToPlayer(player.gameObject);
        Debug.Log($"Manette {gamepad.displayName} assignée au joueur {player.name}");
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
