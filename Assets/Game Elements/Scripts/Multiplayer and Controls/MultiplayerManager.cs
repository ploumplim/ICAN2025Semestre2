using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;

public class MultiplayerManager : MonoBehaviour
{

    public GameObject playerToConnect;

    // public List<GameObject> availablePlayers = new List<GameObject>();
    public List<GameObject> connectedPlayers = new List<GameObject>(); // Liste des joueurs déjà associés
    public GameObject ChargeVisualObject;

    [FormerlySerializedAs("ParryTimeVisual")]
    public GameObject HitTimeVisual;

    public GameObject playerPrefab;
    public new CameraScript camera;
    [FormerlySerializedAs("playerCount")] public int maxPlayerCount;


    public event Action OnPlayerJoin;

    private GameManager gm;

    void Awake()
    {
        DontDestroyOnLoad(this);
        gm = GameManager.Instance;
        gm.multiplayerManager = this;
    }

    public void SetGameParameters()
    {
        camera = gm.levelManager.gameCameraScript;
    }

    public void PlayerJoin()
    {
        FillSpawnPoints();
        if (gm.handleGamePads)
        {
            // Subscribe to the OnSouthButtonPressed event
            gm.handleGamePads.OnSouthButtonPressed += AtoJoin;
        }
    }

    public void WaitForPlayersReady()
    {
        int readyPlayers = 0;

        foreach (GameObject player in connectedPlayers)
        {
            if (player.GetComponent<PlayerScript>().isReady)
            {
                readyPlayers++;
            }
        }

        if (readyPlayers == connectedPlayers.Count)
        {
            gm.AllPlayersReady();
        }
    }


    void FillSpawnPoints()
    {
        if (gm.levelManager)
        {
            foreach (Transform child in gm.levelManager.PlayerSpawnParent.transform)
            {
                gm.levelManager._playerSpawnPoints.Add(child);
                maxPlayerCount = gm.levelManager._playerSpawnPoints.Count;
            }
        }
    }

    void AtoJoin(Gamepad gamepad)
    {
        int currentPlayerCount = gm.handleGamePads.AssignedGamepads.Count;
        if (currentPlayerCount < maxPlayerCount)
        {
            SpawnPlayer(gamepad, currentPlayerCount);
        }
    }

    private void SpawnPlayer(Gamepad gamepad, int currentPlayerCount)
    {
        // Spawn un joueur à la position correspondante.
        SpawnNewPlayerAtPos(gm.levelManager._playerSpawnPoints[currentPlayerCount]);
        connectedPlayers.Add(playerToConnect);
        HandleGamePads.AssignControllerToPlayer(gamepad, playerToConnect); // Assign the gamepad to a player.
        camera.GetComponent<CameraScript>().AddObjectToArray(playerToConnect.gameObject);
        AssignValuesToPlayer(playerToConnect);
        
        GameManager.Instance.levelManager.ingameGUIManager.ChangeColorOfPlayerScorePanel(
            playerToConnect.GetComponent<PlayerScript>().playerScorePanel,
            playerToConnect.GetComponent<PlayerVisuals>().playerCapMaterial.color);
        playerToConnect = null;
        
        OnPlayerJoin?.Invoke();
    }
    
    private void SpawnNewPlayerAtPos(Transform spawnPosition)
    {
        // Instantiate a new player object at a specified position and rotation
        GameObject newPlayer = Instantiate(playerPrefab, spawnPosition.position, Quaternion.identity);
        newPlayer.GetComponent<PlayerScript>().playerSpawnPoint = spawnPosition.gameObject;
        DontDestroyOnLoad(newPlayer.gameObject);

        // Change the player's name to include the player's number
        newPlayer.name = $"Player {connectedPlayers.Count + 1}";

        playerToConnect = newPlayer;
        newPlayer.GetComponent<PlayerScript>().playerScorePanel =
            gm.levelManager.ingameGUIManager.SpawnPlayerScorePanel(newPlayer.GetComponent<PlayerScript>());
        GameManager.Instance.PlayerScriptList.Add(newPlayer.GetComponent<PlayerScript>());
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