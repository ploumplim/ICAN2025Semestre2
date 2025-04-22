using System;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public LevelManager levelManager;
    
    
    public float littleEarthQuakeDuration;
    public float littleEarthQuakeMagnitude;
    
    public float KillEarthQuakeDuration;
    public float KillEarthQuakeMagnitude;

    public void StartLitleScreenShake()
    {
        if (levelManager != null)
        {
            levelManager.gameCameraScript.StartShake(littleEarthQuakeMagnitude, littleEarthQuakeDuration);
        }
        else
        {
            Debug.LogError("LevelManager not assigned in ScreenShake.");
        }
    }
    public void StartKillScreenShake()
    {
        if (levelManager != null)
        {
            levelManager.gameCameraScript.StartShake(KillEarthQuakeDuration, KillEarthQuakeMagnitude);
        }
        else
        {
            Debug.LogError("LevelManager not assigned in ScreenShake.");
        }
    }
}
