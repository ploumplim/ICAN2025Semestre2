using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public GameObject ball;

    [Header("Player and Ball Debug Information")]
    public GameObject playerAndBallsDebugObject;
    private TextMeshProUGUI _playerAndBallsText;
    private const string PauseAccess = "PauseAccess";

    public InputActionAsset inputActionAsset;
    public InputAction pauseAction;
    
    public GameObject pauseMenu;

    void Start()
    {
        _playerAndBallsText = playerAndBallsDebugObject.GetComponent<TextMeshProUGUI>();

        //TODO DebugLog tout les actions de l'asset InputActionAsset
        foreach (var action in inputActionAsset)
        {
            if (action.name == PauseAccess)
            {
                pauseAction = action;
                pauseAction.performed += OnPauseAction;
                pauseAction.Enable();
                Debug.Log("PauseAccess found");
            }
        }
    }

    void Update()
    {
        _playerAndBallsText.text = "\n BALL \n State: " + ball.GetComponent<BallSM>().currentState +
                                   "\n Speed: " + ball.GetComponent<Rigidbody>().linearVelocity.magnitude +
                                   "\n Combo Bounces: " + ball.GetComponent<BallSM>().bounces;
    }

    private void OnPauseAction(InputAction.CallbackContext context)
    {
        Debug.Log("Pause action detected");

        if (pauseMenu.activeSelf)
        {
            Debug.Log("Pause menu is already active");
            pauseMenu.SetActive(false);
            // Add logic for when the pause menu is already active
        }
        else
        {
            Debug.Log("Pause menu is not active");
            pauseMenu.SetActive(true);
        }
    }
    
}