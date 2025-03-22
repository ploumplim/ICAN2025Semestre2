using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject MenuCanvas;
    public GameObject LevelChoiceCanvas;
    public Button AllPlayerReadyButton;
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Update()
    {

    }

    public void AllPlayerReady()
    {
        AllPlayerReadyButton.gameObject.SetActive(true);
    }
    public void AllPlayerNotReady()
    {
        AllPlayerReadyButton.gameObject.SetActive(false);
    }

    public void PlayerReadyButton()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>()._gameManagerSM.InitGameSM(GameManager.Instance);
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.GetComponent<GameManagerSM>().currentState == GameManager.Instance.GetComponent<GameManagerSM>().GetComponent<MenuState>())
        {
            MenuCanvas.SetActive(true);
            LevelChoiceCanvas.SetActive(false);
        }
        if (GameManager.Instance.GetComponent<GameManagerSM>().currentState == GameManager.Instance.GetComponent<GameManagerSM>().GetComponent<LevelChoiceState>())
        {
            MenuCanvas.SetActive(false);
            LevelChoiceCanvas.SetActive(true);
        }
    }
}