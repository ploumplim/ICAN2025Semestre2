using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public MultiplayerManager multiplayerManager;
    public LevelManager levelManager;
    public HandleGamePads handleGamePads;
    public List<PlayerScript> PlayerScriptList;
    public bool isPaused;
    

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<GameManager>();
                if (_instance == null)
                {
                    GameObject singleton = new GameObject(typeof(GameManager).ToString());
                    _instance = singleton.AddComponent<GameManager>();
                    DontDestroyOnLoad(singleton);
                }
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        SetManagerParameters();
    }

    public void SetManagerParameters()
    {
        if (gameObject.GetComponent<GameManagerSM>().currentState is PlayingState)
        {
            Debug.Log("Current state is PlayingState");
        }
    }
    
   

    public void ResumeGame()
    {
        Debug.Log("ResumeGame");
        Time.timeScale = 1f;
    }
    
    public void GoToGameScene()
    {
        StartCoroutine(LoadGameSceneCoroutine());
    }

    private IEnumerator LoadGameSceneCoroutine()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        GetComponent<GameManagerSM>().ChangeState(GetComponent<PlayingState>());
        levelManager.Initialize();
        multiplayerManager.SetGameParameters();
        levelManager.ingameGUIManager.UI_PressStartTutorialtext.SetActive(true);
    }
    
    public void AllPlayersReady()
    {
        levelManager.ingameGUIManager.CountDownTimer();
    }

    public void PausingGame()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            PauseGame(); 
        }
        else
        {
            UnPauseGame();
        }
        
    }

    public void PauseGame()
    {
        TextMeshProUGUI pausingTextMeshProUGUI = levelManager.ingameGUIManager._RoundInformationAffichage;
        
        pausingTextMeshProUGUI.GetComponent<TextMeshProUGUI>().text = "Pausing";
        pausingTextMeshProUGUI.GetComponent<TextMeshProUGUI>().color = Color.red;
        Debug.Log("PauseGame");
        Time.timeScale = 0f;
    }
    public void UnPauseGame()
    {
        TextMeshProUGUI pausingTextMeshProUGUI = levelManager.ingameGUIManager._RoundInformationAffichage;
        
        pausingTextMeshProUGUI.GetComponent<TextMeshProUGUI>().text = "";
        pausingTextMeshProUGUI.GetComponent<TextMeshProUGUI>().color = Color.white;
        Debug.Log("UNPauseGame");
        Time.timeScale = 1f;
    }
}