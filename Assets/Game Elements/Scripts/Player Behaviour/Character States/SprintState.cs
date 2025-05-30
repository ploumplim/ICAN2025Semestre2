using UnityEngine;

public class SprintState : PlayerState
{
    private float _timer;
    private float _currentSprintSpeed;
    [HideInInspector] public float currentSprintBoost;
    public override void Enter()
    {
        _currentSprintSpeed = PlayerScript.sprintSpeed + currentSprintBoost;
        PlayerScript.OnPlayerDash?.Invoke();
        currentSprintBoost = 0f;
        _timer = 0;
    }
    public override void Tick()
    {
        base.Tick();
        _timer += Time.deltaTime;
        float r = 0;
        if (PlayerScript.sprintBoostDecayTime > 0)
        {
            r = _timer / PlayerScript.sprintBoostDecayTime;
        }
        else
        {
            Debug.LogError("Sprint boost decay time is 0, please set a value in the player prefab.");
        }
        
        r = Mathf.Clamp01(r);
        
        float curveVal = PlayerScript.sprintCurve.Evaluate(r);
        
        _currentSprintSpeed = Mathf.Lerp(_currentSprintSpeed, PlayerScript.sprintSpeed, curveVal);
        
        PlayerScript.Move(_currentSprintSpeed, PlayerScript.neutralLerpTime);

        if (PlayerScript.moveInputVector2 == Vector2.zero)
        {
            PlayerScript.ChangeState(GetComponent<NeutralState>());
        }
        
        
    }

    public override void Exit()
    {
        base.Exit();
        PlayerScript.OnPlayerEndDash?.Invoke();
    }
}
