using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayingState : GameState
{
    public GameObject levelSM;
    public GameObject[] canvasMenu;
    public HandleGamePads handleGamePads;
    public Camera mainCamera;
    public override void Enter()
    {
        foreach (var VARIABLE in canvasMenu)
        {
            VARIABLE.SetActive(false);
        }

        // Unload all currently loaded scenes except the active one
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene != SceneManager.GetActiveScene())
            {
                SceneManager.UnloadSceneAsync(scene);
            }
        }

        // Load scene number 2
        SceneManager.LoadScene(2);
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

        Debug.Log(mainCamera.name);
        // foreach (var VARIABLE in handleGamePads.playerReady)
        // {
        //     VARIABLE.GetComponent<PlayerScript>().playerCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        // }

        // Ensure Init is called before ChangeState
        // levelSM.GetComponent<LevelSM>().Init();
    }

    public override void Tick()
    {
        base.Tick();
    }
}