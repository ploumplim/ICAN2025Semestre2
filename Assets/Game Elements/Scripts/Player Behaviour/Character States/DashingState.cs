using System;
using System.Collections;
using UnityEngine;

public class DashingState : PlayerState
{
    [HideInInspector] public float timer;
    private float dashSpeed;
    private Vector3 currentPosition;
    private Vector3 targetPosition;
    
    public override void Enter()
    {
        base.Enter();
        timer = 0;
        
        Physics.IgnoreLayerCollision(PlayerScript.playerLayer, PlayerScript.hazardLayer, true);
        Physics.IgnoreLayerCollision(PlayerScript.playerLayer, PlayerScript.ballLayer, true);
        
        // Set the current position to the players position.
        currentPosition = PlayerScript.transform.position;
        
        // Create a ray cast that is shot from the forward of the player using the dash distance. If an object is
        // touched by the ray cast, the target position is set to the hit point of the ray cast.
        RaycastHit hit;
        if (Physics.Raycast(PlayerScript.transform.position, PlayerScript.transform.forward, out hit,
                PlayerScript.dashDistance))
        {
            targetPosition = hit.point - PlayerScript.transform.forward * PlayerScript.dashOffset;
        }
        else
        {
            // If the ray cast does not hit anything, the target position is set to the players position + the dash
            // distance.
            targetPosition = PlayerScript.transform.position + PlayerScript.transform.forward *
                            PlayerScript.dashDistance;
        }
        
        PlayerScript.rb.isKinematic = true;
    }

    public override void Tick()
    {
        base.Tick();
        timer += Time.deltaTime;
        float r = 0;
        if (PlayerScript.dashDuration != 0)
        {
            r = timer / PlayerScript.dashDuration;
        }
        else
        {
            Debug.LogError("Dash duration is 0, please set a value in the player prefab.");
        }
        
        r = Mathf.Clamp01(r);
        
        // Evaluate the dash curve using r to obtain the current speed.
        float curveVal = PlayerScript.dashCurve.Evaluate(r);
        
        Vector3 newPosition = Vector3.Lerp(currentPosition, targetPosition, curveVal);
        
        PlayerScript.rb.MovePosition(newPosition);
        // //Apply the movement, decreasing the speed of the player over time.
        // dashSpeed = Mathf.Lerp(dashSpeed, PlayerScript.speed, 1f * Time.deltaTime);
        // PlayerScript.Move(dashSpeed, PlayerScript.neutralLerpTime);

        if (timer >= PlayerScript.dashDuration || PlayerScript.throwAction.triggered)
        {
            timer = 0;
            PlayerScript.OnPlayerEndDash?.Invoke();
            PlayerScript.ChangeState(PlayerScript.GetComponent<NeutralState>());
        }
        
        
    }
    

    public override void Exit()
    {
        
        base.Exit();
        timer = 0;
        
        PlayerScript.rb.isKinematic = false;
        Physics.IgnoreLayerCollision(PlayerScript.playerLayer, PlayerScript.hazardLayer, false);
        Physics.IgnoreLayerCollision(PlayerScript.playerLayer, PlayerScript.ballLayer, false);
        
        
    }
    

    
}
