using System;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public LevelManager levelManager;
    
    
    public float littleEarthQuakeDuration;
    public float littleEarthQuakeMagnitude;
    
    public float KillEarthQuakeDuration;
    public float KillEarthQuakeMagnitude;

    public float SpeedMultiplier;
    
    public void StartLitleScreenShake(float ballSpeed)
    {
        if (levelManager != null)
        {
            levelManager.gameCameraScript.StartShake(littleEarthQuakeMagnitude, littleEarthQuakeDuration, SpeedMultiplier,ballSpeed);
        }
        else
        {
            Debug.LogError("LevelManager not assigned in ScreenShake.");
        }
    }
    public void StartKillScreenShake(float ballSpeed)
    {
        if (levelManager != null)
        {
            levelManager.gameCameraScript.StartShake(KillEarthQuakeDuration, KillEarthQuakeMagnitude,SpeedMultiplier,ballSpeed);
        }
        else
        {
            Debug.LogError("LevelManager not assigned in ScreenShake.");
        }
    }
}
