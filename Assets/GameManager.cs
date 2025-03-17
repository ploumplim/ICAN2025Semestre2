using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public GameManagerSM _gameManagerSM;

    public MultiplayerManager mpManager;

    public void OnEnable()
    {
        _gameManagerSM = GetComponent<GameManagerSM>();
        _gameManagerSM.InitGameSM(this);
        if (_gameManagerSM.currentState != _gameManagerSM.GetComponent<MenuState>())
        {
            mpManager = _gameManagerSM.GetComponent<MultiplayerManager>();
        }
    }

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
    }

    public void PauseGame()
    {
        Debug.Log("PauseGame");
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Debug.Log("ResumeGame");
        Time.timeScale = 1f;
    }

    public void SetReady()
    {
        if (GameManager.Instance._gameManagerSM.currentState ==
            GameManager.Instance._gameManagerSM.GetComponent<LevelChoiceState>())
        {

        }
    }
}