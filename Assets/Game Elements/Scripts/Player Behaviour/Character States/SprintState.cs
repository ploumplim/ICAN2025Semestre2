using UnityEngine;

public class SprintState : PlayerState
{

    public override void Tick()
    {
        base.Tick();
        PlayerScript.Move(PlayerScript.speed * PlayerScript.sprintSpeedModifier, PlayerScript.neutralLerpTime);
        
        // Check if the input is not being held. If so, change to the neutral state.
        if (!PlayerScript.playerInput.actions["Sprint"].inProgress)
        {
            PlayerScript.ChangeState(GetComponent<NeutralState>());
        }
        
    }
}
