using System;
using UnityEngine;
using UnityEngine.Events;

public class LevelSM : MonoBehaviour
{
    // ----------- PRIVATE VARIABLES -----------
    public LevelManager levelManager;
    public LevelState currentState;
    // ----------- PUBLIC VARIABLES -----------
    
    // ----------- EVENTS -----------

    public UnityEvent OnLevelEnded;
    
    // ----------- METHODS -----------
    

    // Call the init function when the level is loaded, which starts the state machine.
    public void Init()
    {
        levelManager = GetComponent<LevelManager>();
        
        // Initialize each state of the state machine.
        LevelState[] states = GetComponents<LevelState>();
        foreach (LevelState state in states)
        {
            state.Initialize(this, levelManager);
        }
    }
    
    
    public void ChangeState(LevelState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }
        
        currentState = newState;
        currentState.Enter();
        Debug.Log("Changed state to: " + currentState);
    }
    
    public void RestartGameSM()
    {
        ChangeState(GetComponent<OutOfLevelState>());
        GameManager.Instance.gameObject.GetComponent<GameManagerSM>().ChangeState(GameManager.Instance.gameObject.GetComponent<EndGameState>());
    }
    
    // Update is called once per frame
    void Update()
    {
        if (currentState)
        {
            currentState.Tick();
        }
    }
}
