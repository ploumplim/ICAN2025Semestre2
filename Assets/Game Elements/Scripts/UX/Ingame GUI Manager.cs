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
using UnityEngine.SceneManagement;
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
    public float blinkInterval = 1f;
    public GameObject UI_PressStartTutorialtext;
    public GameObject UI_SetReadyInformationText;
    public GameObject tutorialImage;
    
    
    // --------- PRIVATES ----------
    
    private List<GameObject> _playerList;
    
    private int _playerCount;
    [SerializeField] private TextMeshPro _globalPointsText;
    public TextMeshProUGUI _RoundInformationAffichage;
    [SerializeField] private GameObject PlayerInformationGUI;
    [FormerlySerializedAs("playerPrefabScore")] [SerializeField] private GameObject UI_PlayerHud;
    [SerializeField] private GameObject playerPanelSpawnPointParent;
    
    [SerializeField] private TextMeshProUGUI countDownText;
    
    public List<GameObject> UI_PlayerHUD;
    public List<GameObject> UI_PlayerScore;
    
    

    private void Start()
    {
        UI_PauseMenu = GetComponent<UI_PauseMenu>();
        levelManager = GameManager.Instance.levelManager;

    }

    void Update()
    {
        _playerList = levelManager.playersList;
        // Update the global point texts using the levelManager's global points.
        _playerCount = _playerList.Count;
        //UpdateIndividualPlayerScorePanels();
        if (levelManager.currentState == levelManager.GetComponent<OutOfLevelState>())
        {
            tutorialImage.SetActive(true);
        }
        else
        {
            tutorialImage.SetActive(false);
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
  
    // public void EndGameScoreBoardPlayerPanel(GameObject player, GameObject scorePanel, int playerRank)
    // {
    //     TextMeshProUGUI playerNameText = null;
    //     TextMeshProUGUI playerNumberText = null;
    //
    //     foreach (Transform Text in scorePanel.transform)
    //     {
    //         if (Text.name == "PlayerName")
    //         {
    //             playerNameText = Text.GetComponent<TextMeshProUGUI>();
    //         }
    //         if (Text.name == "PlayerNumber")
    //         {
    //             playerNumberText = Text.GetComponent<TextMeshProUGUI>();
    //         }
    //     }
    //     playerNameText.text = player.name;
    //     playerNumberText.text = playerRank.ToString();
    // }
    
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
                countDownText.text = "Cancelled!";
                yield return new WaitForSeconds(1);
                countDownText.text = "";
                countDownText.gameObject.SetActive(false);
                yield break;
            }

            // Update the UI text element with the remaining time
            countDownText.text = remainingTime + "!"; 
            if (GameManager.Instance.countDownTimerEvent != null)
            {
                GameManager.Instance.countDownTimerEvent.Invoke();
            }
            // GameManager.Instance.levelManager.currentRound++;
            yield return new WaitForSeconds(1);
            remainingTime--;
        }
        
        countDownText.text = "Blitz!";
        StartCoroutine(DissapearCountDown());
        StartGame();


    }

    IEnumerator DissapearCountDown()
    {
        yield return new WaitForSeconds(roundInformationDuration);
        countDownText.gameObject.SetActive(false);
    }
   
    
    public void ActivateSetReadyText()
    {
        UI_SetReadyInformationText.SetActive(true);
        
        StartBlinking(UI_SetReadyInformationText, blinkInterval);
    }

    private Coroutine _blinkingCoroutine;

    public void StartBlinking(GameObject uiElement, float blinkInterval)
    {

        if (_blinkingCoroutine == null)
        {
            _blinkingCoroutine = StartCoroutine(BlinkUIElement(uiElement, blinkInterval));
        }
    }

    public void StopBlinking()
    {
        if (_blinkingCoroutine != null)
        {
            StopCoroutine(_blinkingCoroutine);
            _blinkingCoroutine = null;
            UI_SetReadyInformationText.SetActive(false);
        }
    }

    private IEnumerator BlinkUIElement(GameObject uiElement, float blinkInterval)
    {
        while (true)
        {
            uiElement.SetActive(!uiElement.activeSelf);
            yield return new WaitForSeconds(blinkInterval);
        }
    }
    
    public void StartGame()
    {
        GetComponent<EndGameScorePanel>().EndGameScorePanelGO.SetActive(false);
        // When the countdown is finished, you can perform any additional actions here
        GameManager.Instance.levelManager.currentRound++;
        foreach (GameObject goals in GameManager.Instance.levelManager.GoalList)
        {
            goals.GetComponent<PointTracker>()._points = 0;
            for (int i = 0; i < goals.transform.childCount; i++)
            {
                GameObject child = goals.transform.GetChild(i).gameObject;
                var tmp = child.GetComponent<TextMeshPro>();
                if (tmp != null)
                {
                    tmp.text = goals.GetComponent<PointTracker>()._points.ToString();
                }
            }
        }
        GameManager.Instance.levelManager.ColorizeGoalMesh();
        GameManager.Instance.levelManager.StartLevel(); // Call StartLevel when the countdown finishes
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void BackToMainMenu()
    {
        StartCoroutine(LoadMainMenuAsync());
    }

    private IEnumerator LoadMainMenuAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(0);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}