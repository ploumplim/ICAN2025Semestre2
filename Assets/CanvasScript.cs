using System;
using UnityEngine;

public class CanvasScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject MenuCanvas;
    public GameObject LevelChoiceCanvas;
    private void Awake()
    {
        DontDestroyOnLoad(this);
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
