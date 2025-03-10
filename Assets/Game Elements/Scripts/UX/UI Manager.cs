using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public GameObject ball;

    [Header("Player and Ball Debug Information")]
    public GameObject playerAndBallsDebugObject;
    private TextMeshProUGUI _playerAndBallsText;

    public InputActionAsset inputActionAsset;
    public InputAction pauseAction;

    public GameObject pauseMenu;

    public UnityEvent PauseFonction;

    void Start()
    {
        _playerAndBallsText = playerAndBallsDebugObject.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        _playerAndBallsText.text = "\n BALL \n State: " + ball.GetComponent<BallSM>().currentState +
                                   "\n Speed: " + ball.GetComponent<Rigidbody>().linearVelocity.magnitude +
                                   "\n Combo Bounces: " + ball.GetComponent<BallSM>().bounces;
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