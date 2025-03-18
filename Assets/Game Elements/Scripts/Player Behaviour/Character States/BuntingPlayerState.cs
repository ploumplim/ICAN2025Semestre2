using System.Collections;
using UnityEngine;

public class BuntingPlayerState : PlayerState
{
    [HideInInspector] public float timer;
    [HideInInspector] public bool ballBunted;
    [HideInInspector] public GameObject ballToBunt;
    
    //---------------------------------------------------------------------------------
    public override void Enter()
    {
        base.Enter();
        Bunt();
        ballBunted = false;
    }

    public void Bunt()
    {
        PlayerScript.OnPlayerPerformedBunt?.Invoke();
        PlayerScript.buntTimer = PlayerScript.buntDuration;
        StartCoroutine(BuntTime());
    }

    IEnumerator BuntTime()
    {
        yield return new WaitForSeconds(PlayerScript.buntDuration);
        PlayerScript.ChangeState(GetComponent<NeutralState>());
    }
    
    // ---------------------------------------------------------------------------------
    
    public override void Tick()
    {
        base.Tick();
        timer += Time.deltaTime;
        PlayerScript.Move(PlayerScript.speed * PlayerScript.buntSpeedModifier, PlayerScript.chargeLerpTime);
        BuntBox();
        BuntTheBall();
        
        if (timer >= PlayerScript.buntCooldown)
        {
            PlayerScript.ChangeState(GetComponent<NeutralState>());
        }
    }

    public void BuntBox()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward * 
            PlayerScript.buntSpherePositionOffset,
            PlayerScript.buntSphereRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.GetComponent<BallSM>())
            {
                ballToBunt = hitCollider.gameObject;
            }
        }
    }

    public void BuntTheBall()
    {
        if (ballToBunt && !ballBunted)
        {
            PlayerScript.OnPlayerBuntBall?.Invoke();
            ballToBunt.GetComponent<BallSM>().ChangeState(ballToBunt.GetComponent<BuntedBallState>());
            ballToBunt.GetComponent<Rigidbody>().AddForce(transform.up * PlayerScript.buntForce, ForceMode.Impulse);
            ballBunted = true;
            ballToBunt = null;
            
        }
    }

    
    // ---------------------------------------------------------------------------------
    public override void Exit()
    {
        base.Exit();
        timer = 0;
        ballToBunt = null;
    }
}
