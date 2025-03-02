using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject ball;
    
    [Header("Player and Ball Debug Information")]
    public GameObject playerAndBallsDebugObject;
    private TextMeshProUGUI _playerAndBallsText;
    
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
}
