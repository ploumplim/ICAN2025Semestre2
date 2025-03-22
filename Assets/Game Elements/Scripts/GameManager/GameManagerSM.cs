using UnityEngine;

public class GameManagerSM : MonoBehaviour
{
    // ~~VARIABLES~~
    public GameState currentState;
    
    
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void InitGameSM(GameManager gameManager)
    {


        GameState[] states = GetComponents<GameState>();
        foreach (GameState state in states)
        {
            state.Initialize(this, gameManager);
        }

        if (currentState != null && currentState.GetComponent<LevelChoiceState>() != null)
        {
            ChangeState(GetComponent<PlayingState>());
        }
    }
    
    // ~~~~~~~~~~~~~~~~~~~~~~ CHANGE STATE ~~~~~~~~~~~~~~~~~~~~~~
    public void ChangeState(GameState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }
        currentState = newState;
        currentState.Enter();
        // Debug.Log("State changed to: " + newState);
    }
}