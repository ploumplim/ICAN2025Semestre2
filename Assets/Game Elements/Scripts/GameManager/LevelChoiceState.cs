using System;
using UnityEngine;

public class LevelChoiceState : GameState
{
    private GameManagerSM gameManagerSM;
    
    [HideInInspector]protected HandleGamePads HandleGamePads;
    
    public override void Enter()
    {
        Debug.Log("LevelChoice Enter");
        gameManagerSM = GetComponent<GameManagerSM>();
    }

    public override void Tick()
    {
        
    }

    public void LaunchGame()
    {
        Debug.Log("LaunchGame");
    }

   
}
