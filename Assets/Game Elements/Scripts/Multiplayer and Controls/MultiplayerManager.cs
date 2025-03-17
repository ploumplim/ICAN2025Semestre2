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
    FillSpawnPoints();
    handleGamePads = FindFirstObjectByType<HandleGamePads>();

    if (handleGamePads)
    {
        //subscribe to the OnSouthButtonPressed event
        handleGamePads.OnSouthButtonPressed += AtoJoin;
    }
    
    }

    void FillSpawnPoints()
    {
        foreach (Transform child in spawnParent.transform)
        {
            spawnPoints.Add(child);
        }
    }

    void AtoJoin(Gamepad gamepad)
    {
        if (handleGamePads.AssignedGamepads.Count < spawnPoints.Count)
        {
                    SpawnNewPlayer(spawnPoints[handleGamePads.AssignedGamepads.Count]); // Spawn un joueur à la position correspondante.
                    connectedPlayers.Add(playerToConnect);
                    HandleGamePads.AssignControllerToPlayer(gamepad, playerToConnect); // Assign the gamepad to a player.
                    camera.GetComponent<CameraScript>().AddObjectToArray(playerToConnect.gameObject);
                    AssignValuesToPlayer(playerToConnect);
                    playerToConnect = null;
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
