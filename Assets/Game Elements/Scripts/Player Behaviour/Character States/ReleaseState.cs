using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReleaseState : PlayerState
{
    public override void Enter()
    {
        base.Enter();
        Parry();
    }

    public void Parry()
    {
        // Debug.Log("Parry!");
        PlayerScript.PlayerParried?.Invoke();
        PlayerScript.parryTimer = PlayerScript.parryCooldown;
        StartCoroutine(ParryTime());
    }
    
    IEnumerator ParryTime()
    {
        yield return new WaitForSeconds(PlayerScript.parryCooldown);
        PlayerScript.ChangeState(GetComponent<NeutralState>());
    }
    
    public override void Exit()
    {
        base.Exit();
        PlayerScript.chargeValueIncrementor = 0f;
    }
}
