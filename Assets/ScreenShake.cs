using System;
using UnityEngine;
using UnityEngine.Serialization;

public class ScreenShake : MonoBehaviour
{
    public LevelManager levelManager;
    
    [Header("Little Screen Shake")]
    public float littleEarthQuakeDuration;
    public float littleEarthQuakeMagnitude;
    [FormerlySerializedAs("KillEarthQuakeDuration")] [Header("Goal Screen Shake")]
    public float GoalEarthQuakeDuration;
    [FormerlySerializedAs("KillEarthQuakeMagnitude")] public float GoalEarthQuakeMagnitude;

    [Space(50)]
    public float SpeedMultiplier;

    private void Start()
    {
        levelManager = GameManager.Instance.levelManager;
    }

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
    public void StartGoalScreenShake(float ballSpeed)
    {
        if (levelManager != null)
        {
            levelManager.gameCameraScript.StartShake(GoalEarthQuakeDuration, GoalEarthQuakeMagnitude,SpeedMultiplier,ballSpeed);
        }
        else
        {
            Debug.LogError("LevelManager not assigned in ScreenShake.");
        }
    }
}
