using System.Collections;
using System.Collections.Generic;
using Eflatun.SceneReference;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public MultiplayerManager multiplayerManager;
    public LevelManager levelManager;
    public HandleGamePads handleGamePads;
    public List<PlayerScript> PlayerScriptList;

    public List<SceneReference> scenesToLoad;
    public List<SceneReference> NextSceneToPlay;

    public int currentSceneID;
    
    public UnityEvent countDownTimerEvent;
    

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

        scenesToLoad = scenesToLoad ?? new List<SceneReference>();
        NextSceneToPlay = NextSceneToPlay ?? new List<SceneReference>();

        SetManagerParameters();
    }

    public void SetManagerParameters()
    {
        var gameManagerSM = gameObject.GetComponent<GameManagerSM>();
        if (gameManagerSM != null && gameManagerSM.currentState is PlayingState)
        {
            Debug.Log("Current state is PlayingState");
        }
    }
    
    public void GoToGameScene()
    {
        StartCoroutine(LoadGameSceneCoroutine());
        
    }

    private IEnumerator LoadGameSceneCoroutine()
    {
        StartLoadArenaScene();

        while (!SceneManager.GetSceneByBuildIndex(1).isLoaded || !SceneManager.GetSceneByBuildIndex(2).isLoaded)
        {
            yield return null;
        }

        var gameManagerSM = GetComponent<GameManagerSM>();
        if (gameManagerSM != null)
        {
            gameManagerSM.ChangeState(GetComponent<PlayingState>());
        }

        if (levelManager != null)
        {
            levelManager.Initialize();
        }

        if (multiplayerManager != null)
        {
            multiplayerManager.SetGameParameters();
        }

        if (levelManager != null && levelManager.ingameGUIManager != null)
        {
            // levelManager.ingameGUIManager.UI_PressStartTutorialtext.SetActive(true);
        }

        levelManager.ingameGUIManager.ActivateSetReadyText();
    }

    public void StartLoadArenaScene()
    {
        StartCoroutine(LoadFirstArenaScene());
    }

    private IEnumerator LoadFirstArenaScene()
    {
        AsyncOperation asyncLoadContainer = SceneManager.LoadSceneAsync(1);

        while (!asyncLoadContainer.isDone)
        {
            yield return null;
        }

        foreach (SceneReference sceneReference in scenesToLoad)
        {
            if (sceneReference != null && sceneReference.BuildIndex >= 0)
            {
                int sceneBuildIndex = sceneReference.BuildIndex;
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Additive);
                
                
                while (!asyncLoad.isDone)
                {
                    yield return null;
                }

                if (sceneBuildIndex > 2)
                {
                    Scene loadedScene = SceneManager.GetSceneByBuildIndex(sceneBuildIndex);
                    foreach (GameObject rootObject in loadedScene.GetRootGameObjects())
                    {
                        rootObject.SetActive(false);
                    }
                }
            }
            else
            {
                Debug.LogWarning("Invalid SceneReference or BuildIndex.");
            }
        }

        LevelLoader();
    }
    
    public void LevelLoader()
    {
        //levelManager.LoadGoalsForScene();
        if (NextSceneToPlay != null && NextSceneToPlay.Count > 0)
        {
            currentSceneID++;
            // int randomIndex = Random.Range(0, NextSceneToPlay.Count);
            // RandomLevelSelection(randomIndex);
            // NextSceneToPlay.RemoveAt(randomIndex);
            
            RandomLevelSelection(currentSceneID);
        }
    }
    
    public void RandomLevelSelection(int levelIndex)
    {
        int preserveIndex1 = 1;
        int preserveIndex2 = -1;

        if (levelIndex >= 0 && levelIndex < NextSceneToPlay.Count)
        {
            preserveIndex2 = NextSceneToPlay[levelIndex].BuildIndex;
        }
        else
        {
            Debug.LogWarning("Invalid levelIndex provided for NextSceneToPlay.");
        }

        int sceneCount = SceneManager.sceneCount;
        for (int i = 0; i < sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            int buildIndex = scene.buildIndex;

            if (buildIndex != preserveIndex1 && buildIndex != preserveIndex2)
            {
                foreach (GameObject rootObject in scene.GetRootGameObjects())
                {
                    rootObject.SetActive(false);
                }
            }
            else
            {
                foreach (GameObject rootObject in scene.GetRootGameObjects())
                {
                    rootObject.SetActive(true);
                }
            }
        }
    }

    public void AllPlayersReady()
    {
        if (levelManager != null && levelManager.ingameGUIManager != null)
        {
            levelManager.ingameGUIManager.CountDownTimer();
        }
    }
    

    public void PauseGame()
    {
        if (levelManager != null && levelManager.ingameGUIManager != null)
        {
            TextMeshProUGUI pausingTextMeshProUGUI = levelManager.ingameGUIManager._RoundInformationAffichage;
            pausingTextMeshProUGUI.text = "Pausing";
            pausingTextMeshProUGUI.color = Color.red;
        }

        Debug.Log("PauseGame");
        Time.timeScale = 0f;
    }

    public void UnPauseGame()
    {
        if (levelManager != null && levelManager.ingameGUIManager != null)
        {
            TextMeshProUGUI pausingTextMeshProUGUI = levelManager.ingameGUIManager._RoundInformationAffichage;
            pausingTextMeshProUGUI.text = "";
            pausingTextMeshProUGUI.color = Color.white;
        }

        Debug.Log("UNPauseGame");
        Time.timeScale = 1f;
    }
}