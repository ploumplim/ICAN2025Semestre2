using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class IngameGUIManager : MonoBehaviour
{
    
    // ---------- PUBLICS -----------
    
    [Header("Game Objects Settings and Prefabs")]
    public GameObject ball;
    [Header("Player and Ball Debug Information")]
    public InputAction pauseAction;
    public GameObject pauseMenu;
    [Header("GUI")]
    public LevelManager levelManager;
    public GameObject startGameButtonObject;
    public GameObject resetPlayersObject;
    public float roundInformationDuration = 1.5f;
    public GameObject ScorePlayerUIEndGame;
    public UI_PauseMenu UI_PauseMenu;
    
    // --------- PRIVATES ----------
    
    private List<GameObject> _playerList;
    
    private int _playerCount;
    [SerializeField] private TextMeshPro _globalPointsText;
    public TextMeshProUGUI _RoundInformationAffichage;
    [SerializeField] private GameObject PlayerInformationGUI;
    [FormerlySerializedAs("playerPrefabScore")] [SerializeField] private GameObject UI_PlayerHud;
    [SerializeField] private GameObject playerPanelSpawnPointParent;
    
    public List<GameObject> UI_PlayerHUD;
    public List<GameObject> UI_PlayerScore;

    private void Start()
    {
        UI_PauseMenu = GetComponent<UI_PauseMenu>();
        
    }

    void Update()
    {
        _playerList = levelManager.players;
        // Update the global point texts using the levelManager's global points.
        _globalPointsText.text = levelManager.potScore.ToString();
        _playerCount = _playerList.Count;
        //UpdateIndividualPlayerScorePanels();

    }
    
    public void SetPlayerHud(GameObject playerInfoGui,string playerName , string playerScore, string playerState)
    {
        Debug.Log(playerName);
            TextMeshProUGUI playerNameText = null;
            TextMeshProUGUI playerScoreText = null;
            TextMeshProUGUI playerStateText = null;

            
            foreach (var textMesh in playerInfoGui.GetComponentsInChildren<TextMeshProUGUI>())
            {
                switch (textMesh.gameObject.name)
                {
                    case "PlayerName":
                        playerNameText = textMesh;
                        break;
                    case "PlayerScore":
                        playerScoreText = textMesh;
                        break;
                    case "PlayerState":
                        playerStateText = textMesh;
                        break;
                }
            }

            if (playerNameText != null)
            {
                playerNameText.text = playerName;
            }

            if (playerScoreText != null)
            {
                playerScoreText.text = playerScore;
            }

            if (playerStateText != null)
            {
                playerStateText.text = playerState;
            }
    }
    
    public void UpdatePlayerState(GameObject playerInfoGui, bool isReady)
    {
        TextMeshProUGUI playerStateText = null;

        foreach (var textMesh in playerInfoGui.GetComponentsInChildren<TextMeshProUGUI>())
        {
            if (textMesh.gameObject.name == "PlayerState")
            {
                playerStateText = textMesh;
                break;
            }
        }

        if (playerStateText != null)
        {
            playerStateText.text = isReady ? "Ready" : "Not Ready";
        }
    }
    public void UpdatePlayerScore(GameObject playerInfoGui, int score)
    {
        TextMeshProUGUI playerScoreText = null;

        foreach (var textMesh in playerInfoGui.GetComponentsInChildren<TextMeshProUGUI>())
        {
            if (textMesh.gameObject.name == "PlayerScore")
            {
                playerScoreText = textMesh;
                break;
            }
        }

        if (playerScoreText != null)
        {
            playerScoreText.text = score.ToString();
        }
    }
    
    public void AssignBall(GameObject ballObject)
    {
        ball = ballObject;
    }
    
    void OnEnable()
    {
        pauseAction.Enable(); 
    }
    
    // ------------------------ ROUND INFORMATION FUNCTIONS
    
    
    public void EndGameScoreBoardPlayerPanel(GameObject player, GameObject scorePanel, int playerRank)
    {
        TextMeshProUGUI playerScoreText = null;
        TextMeshProUGUI playerNameText = null;
        TextMeshProUGUI playerNumberText = null;

        foreach (Transform Text in scorePanel.transform)
        {
            if (Text.name == "PlayerName")
            {
                playerNameText = Text.GetComponent<TextMeshProUGUI>();
            }
            if (Text.name == "PlayerNumber")
            {
                playerNumberText = Text.GetComponent<TextMeshProUGUI>();
            }
            if (Text.name == "PlayerScore")
            {
                playerScoreText = Text.GetComponent<TextMeshProUGUI>();
            }
        }

        playerNameText.text = player.name;
        playerScoreText.text = player.GetComponent<PlayerPointTracker>().points.ToString();
        playerNumberText.text = playerRank.ToString();
    }
    
    public GameObject SpawnPlayerScorePanel(PlayerScript player)
    {
        // Create a list of the children of playerPanelSpawnPointParent
        List<Transform> spawnPoints = new List<Transform>();
        foreach (Transform child in playerPanelSpawnPointParent.transform)
        {
            spawnPoints.Add(child);
        }

        // Determine the position for the new panel based on the number of panels already spawned
        int panelIndex = UI_PlayerHUD.Count;
        if (panelIndex >= spawnPoints.Count)
        {
            Debug.LogError("Not enough spawn points for player score panels.");
            return null;
        }

        // Instantiate the new panel at the corresponding spawn point
        GameObject playerScorePanel = Instantiate(UI_PlayerHud, spawnPoints[panelIndex].position, Quaternion.identity, PlayerInformationGUI.transform);
        TextMeshProUGUI playerNameText = null;
        foreach (Transform Text in (playerScorePanel.transform))
        {
            if (Text.name == "PlayerName")
            {
                playerNameText = Text.GetComponent<TextMeshProUGUI>();
            }
            playerNameText.text = player.name;
        }
        
        UI_PlayerHUD.Add(playerScorePanel);
        return playerScorePanel;
    }
    public void ChangeColorOfPlayerScorePanel(GameObject playerScorePanel, Color color)
    {
        Image image = playerScorePanel.GetComponent<Image>();
        if (image != null)
        {
            image.color = color;
        }
    }
    public void CountDownTimer()
    {
        StartCoroutine(StartCountdown(3)); // Start a 5-second countdown
    }
    
    private IEnumerator StartCountdown(int duration)
    {
        _RoundInformationAffichage.gameObject.SetActive(true);
        int remainingTime = duration;
        while (remainingTime > 0)
        {
            // Check if all players are still ready
            bool allPlayersReady = true;
            foreach (GameObject player in GameManager.Instance.multiplayerManager.connectedPlayers)
            {
                if (!player.GetComponent<PlayerScript>().isReady)
                {
                    allPlayersReady = false;
                    break;
                }
            }

            if (!allPlayersReady)
            {
                _RoundInformationAffichage.text = "Queue Canceled";
                yield return new WaitForSeconds(1);
                _RoundInformationAffichage.text = "";
                _RoundInformationAffichage.gameObject.SetActive(false);
                yield break;
            }

            // Update the UI text element with the remaining time
            _RoundInformationAffichage.text = remainingTime.ToString();
            //Debug.Log(remainingTime);
            yield return new WaitForSeconds(1);
            remainingTime--;
        }

        // When the countdown is finished, you can perform any additional actions here
        _RoundInformationAffichage.text = "";
        _RoundInformationAffichage.gameObject.SetActive(false);
        GameManager.Instance.levelManager.StartLevel(); // Call StartLevel when the countdown finishes
    }
}