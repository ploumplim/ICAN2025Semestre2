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
    
    // --------- PRIVATES ----------
    
    private List<GameObject> _playerList;
    [FormerlySerializedAs("_playerScorePanelList")] public List<GameObject> playerScorePanelList;
    private int _playerCount;
    [SerializeField] private TextMeshPro _globalPointsText;
    [FormerlySerializedAs("_startGameText")] [SerializeField] private TextMeshProUGUI _RoundInformationAffichage;
    public List<GameObject> _playerHud;
    
    void Start()
    {
        GameObject PlayerPanelParent = GameObject.FindGameObjectWithTag("PlayerInformationPanel");
        // After
        foreach (var VARIABLE in PlayerPanelParent.GetComponentsInChildren<PlayerInfoGui>())
        {
            _playerHud.Add(VARIABLE.gameObject);
        }
        
    }

    void Update()
    {
        _playerList = levelManager.players;
        // Update the global point texts using the levelManager's global points.
        _globalPointsText.text = levelManager.potScore.ToString();
        _playerCount = _playerList.Count;
        //UpdateIndividualPlayerScorePanels();

    }
    
    public void UpdatePlayerHud(GameObject playerInfoGui,string playerName , string playerScore, string playerState)
    {
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
    
    public void AssignBall(GameObject ballObject)
    {
        ball = ballObject;
    }
    
    void OnEnable()
    {
        pauseAction.Enable(); 
    }
    
    public void OnPauseAction(InputAction.CallbackContext context)
    {
        if (pauseMenu.activeSelf)
        {
            GameManager.Instance.ResumeGame();
            pauseMenu.SetActive(false);
        }
        else
        {
            GameManager.Instance.PauseGame();
            pauseMenu.SetActive(true);
        }
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