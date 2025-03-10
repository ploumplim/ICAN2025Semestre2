using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ PRIVATE VARIABLES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private LevelSM _levelSM;
    private LevelState _currentState;
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ PUBLIC VARIABLES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ EVENTS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ METHODS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    
    // Call initialize when level is starting.
    
    public void Initialize()
    {
        _levelSM = GetComponent<LevelSM>();
        _levelSM.Init(); 
    }

    public void Update()
    {
        if (_levelSM && _levelSM.currentState)
        {
            _currentState = _levelSM.currentState;
        }
    }
}
