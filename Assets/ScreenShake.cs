using System;
using UnityEngine;
using UnityEngine.Serialization;

public class ScreenShake : MonoBehaviour
{
    public LevelManager levelManager;
    
    [Header("Little Screen Shake")]
    public float littleEarthQuakeDuration;
    public float littleEarthQuakeMagnitude;
    [Header("Goal Screen Shake")]
    public float GoalEarthQuakeDuration;
    public float GoalEarthQuakeMagnitude;
     
    [Header("Shake Curves")]
    [SerializeField] private AnimationCurve goalShakeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve littleShakeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

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
            levelManager.gameCameraScript.StartShake(littleShakeCurve,littleEarthQuakeDuration, littleEarthQuakeMagnitude, SpeedMultiplier,ballSpeed);
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
            levelManager.gameCameraScript.StartShake(goalShakeCurve, GoalEarthQuakeDuration, GoalEarthQuakeMagnitude, SpeedMultiplier, ballSpeed);
        }
        else
        {
            Debug.LogError("LevelManager not assigned in ScreenShake.");
        }
    }
}
