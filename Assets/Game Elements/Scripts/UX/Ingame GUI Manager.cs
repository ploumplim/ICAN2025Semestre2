using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class IngameGUIManager : MonoBehaviour
{
    [Header("Game Objects Settings and Prefabs")]
    public GameObject ball;
    [Header("Player and Ball Debug Information")]
    public GameObject playerAndBallsDebugObject;
    private TextMeshProUGUI _playerAndBallsText;
    public InputAction pauseAction;
    public GameObject pauseMenu;
    [Header("GUI")]
    public LevelManager levelManager;
    public GameObject startGameButtonObject;
    public GameObject resetPlayersObject;
    private List<GameObject> _playerList;
    [SerializeField] private TextMeshPro _globalPointsText;

    void Start()
    {
        _playerAndBallsText = playerAndBallsDebugObject.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        _playerList = levelManager.players;
        TextUpdate();
        // Update the global point texts using the levelManager's global points.
        _globalPointsText.text = levelManager.potScore.ToString();
    }
    
    public void AssignBall(GameObject ballObject)
    {
        ball = ballObject;
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
                                   
                                   "\n Score pot: " + levelManager.potScore +
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