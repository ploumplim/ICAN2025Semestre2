using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;

public class MultiplayerManager : MonoBehaviour
{
    
    public Dictionary<Gamepad, GameObject> controllerToPlayer = new Dictionary<Gamepad, GameObject>();
    
    
    public List<GameObject> availablePlayers = new List<GameObject>();
    public List<GameObject> connectedPlayers = new List<GameObject>(); // Liste des joueurs déjà associés
    public HashSet<Gamepad> pendingGamepads = new HashSet<Gamepad>();
    public GameObject ChargeVisualObject;
    public GameObject ParryTimeVisual;
    public GameObject playerPrefab;
    public GameObject spawnObject;
    [HideInInspector] public Vector3 spawnPosition;
    public new Camera camera;
    

    void Start()
    {
        
        // // Trouve tous les joueurs dans la scène avec le tag "Player"
        // availablePlayers = GameObject.FindGameObjectsWithTag("Player").ToList();
        
        // Ajoute toutes les manettes déjà connectées à la liste d'attente
        foreach (var gamepad in Gamepad.all)
        {
            pendingGamepads.Add(gamepad);
        }
        
        spawnPosition = spawnObject.transform.position;
    }
    void Update()
    {
        // Vérifie si une manette en attente appuie sur un bouton
        foreach (Gamepad gamepad in pendingGamepads.ToList())
        {
            if (gamepad.buttonSouth.wasReleasedThisFrame)
            {
                SpawnNewPlayer();
                AssignControllerToPlayer(gamepad);
                pendingGamepads.Remove(gamepad);
                return; // Évite de traiter plusieurs manettes en une frame
            }
        }
    }

    private void AssignControllerToPlayer(Gamepad gamepad)
    {
        
        if (availablePlayers.Count == 0)
        {
            Debug.LogWarning("Aucun joueur disponible pour être associé à la manette.");
            return;
        }
        
        // Prend le premier joueur disponible
        GameObject player = availablePlayers[0];
        
        availablePlayers.RemoveAt(0); // Retire ce joueur de la liste des disponibles
        connectedPlayers.Add(player); // Ajoute ce joueur à la liste des occupés
        
        player.SetActive(true);
        
        // Associe la manette à ce joueur
        
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

    private void SpawnNewPlayer()
    {
        // Instantiate a new player object at a specified position and rotation
        GameObject newPlayer = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        
        
        // Add the new player to the list of available players
        availablePlayers.Add(newPlayer);

        // Debug.Log($"New player spawned at position {spawnPosition}");
    }

    private void AssignValuesToPlayer(GameObject player)
    {
        PlayerScript playerScript = player.GetComponent<PlayerScript>();
        PlayerVisuals playerVisuals = player.GetComponent<PlayerVisuals>();
        
        // ---- Assign values to the player ----
        
        // ---- Visual values ----
        
        // Generate a random color.
        Color randomColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        
        // Assign the random color to the player's mesh.
        playerVisuals.ChangePlayerColor(randomColor);
        
    }
}
