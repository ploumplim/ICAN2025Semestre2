using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class IngameGUIManager : MonoBehaviour
{
    public GameObject ball;

    [Header("Player and Ball Debug Information")]
    public GameObject playerAndBallsDebugObject;
    private TextMeshProUGUI _playerAndBallsText;

    public InputActionAsset inputActionAsset;
    public InputAction pauseAction;

    public GameObject pauseMenu;

    public UnityEvent PauseFonction;
    
    [Header("Score Information")]
    public LevelManager levelManager;
    
    private List<GameObject> _playerList;

    void Start()
    {
        _playerAndBallsText = playerAndBallsDebugObject.GetComponent<TextMeshProUGUI>();
        
    }

    void Update()
    {
        _playerList = levelManager.players;
        TextUpdate();
    }
    
    void OnEnable()
    {
        pauseAction.Enable(); 
    }
    
    public void TextUpdate()
    {
        if(!ball || !levelManager || _playerList.Count == 0)
        {
            if (!ball)
            {
                Debug.Log("Ball is null");
            }
            if (!levelManager)
            {
                Debug.Log("LevelManager is null");
            }
            if (_playerList.Count == 0)
            {
                Debug.Log("PlayerList is empty");
            }
            
            return;
        }
        
        _playerAndBallsText.text = "\n BALL \n State: " + ball.GetComponent<BallSM>().currentState +
                                   "\n Speed: " + ball.GetComponent<Rigidbody>().linearVelocity.magnitude +
                                   "\n Combo Bounces: " + ball.GetComponent<BallSM>().bounces +
                                   "\n Owner: " + ball.GetComponent<BallSM>().ballOwnerPlayer +
                                   
                                   "\n Global Score: " + levelManager.globalScore +
                                   "\n Round: " + levelManager.currentRound + " / " + levelManager.totalRounds;
        
        foreach (GameObject player in _playerList)
        {
            _playerAndBallsText.text += "\n" + player.name + " Score: " + player.GetComponent<PlayerPointTracker>().points;
        }
        
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
}