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
    public Vector3 spawnPosition;
    public new Camera camera;
    

    void Start()
    {
        
        // Trouve tous les joueurs dans la scène avec le tag "Player"
        availablePlayers = GameObject.FindGameObjectsWithTag("Player").ToList();
        
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
        foreach (var gamepad in pendingGamepads.ToList())
        {
            if (gamepad.allControls.Any(control => control is ButtonControl button && button.wasPressedThisFrame))
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
        
        // Associe la manette à ce joueur
        controllerToPlayer[gamepad] = player;
        
        camera.GetComponent<CameraScript>().AddPlayerToArray(player.gameObject);
        
        Debug.Log($"Manette {gamepad.displayName} assignée au joueur {player.name}");
    }

    private void SpawnNewPlayer()
    {
        // Instantiate a new player object at a specified position and rotation
        GameObject newPlayer = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

        // Add the new player to the list of available players
        availablePlayers.Add(newPlayer);

        Debug.Log($"New player spawned at position {spawnPosition}");
    }

    private void AssignValuesToPlayer()
    {
        
    }
}
