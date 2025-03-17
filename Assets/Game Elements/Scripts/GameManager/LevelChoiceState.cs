using System;
using UnityEngine;

public class LevelChoiceState : GameState
{
    private GameManagerSM gameManager;
    public override void Enter()
    {
        Debug.Log("LevelChoice Enter");
        gameManager = GetComponent<GameManagerSM>();

        
    }
        
}
