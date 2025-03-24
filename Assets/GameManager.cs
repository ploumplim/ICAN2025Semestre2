using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.PlayerLoop; // Nécessaire pour utiliser les coroutines

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public GameManagerSM _gameManagerSM;
    public MultiplayerManager mpManager;
    public GameObject levelManager;
    public List<GameObject> player;

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

        // Si l'état actuel est PlayingState, lance la recherche du LevelManager
        
        
    }

     public void SendPlayerToGame()
    {
        if (_instance._gameManagerSM.currentState == _instance._gameManagerSM.GetComponent<PlayingState>())
        {
            StartCoroutine(FindLevelManagerAfterSceneLoad());

            foreach (var VARIABLE in mpManager.connectedPlayers)
            {
                Debug.Log(VARIABLE.gameObject.name);
            }
            
            if (player.Count>0)
            {
                Debug.Log("Il y a " +player.Count + " joueurs connectés.");
                
            }
            else
            {
                Debug.LogWarning("Aucun joueur connecté dans la liste.");
            }
            
        }
    }

    IEnumerator FindLevelManagerAfterSceneLoad()
    {
        // Attends la fin de la frame actuelle pour garantir que la scène est complètement chargée
        yield return new WaitForEndOfFrame();

        // Vérifie que le levelManager est correctement assigné
        if (levelManager != null)
        {
            levelManager.GetComponent<LevelSM>().Init();
        }
        else
        {
            Debug.LogError("LevelManager est null !");
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
}
