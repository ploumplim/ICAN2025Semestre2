using System;
using UnityEngine;

public class RollingState : PlayerState
{
    public float timer;
    public override void Enter()
    {
        base.Enter();
        timer = 0;
        // push the player in the direction of the moveInput.
        Vector3 direction = PlayerScript.RollPush();
        PlayerScript.rb.AddForce(direction * PlayerScript.rollSpeed, ForceMode.Impulse);
    }

    public override void Tick()
    {
        base.Tick();
        timer += Time.deltaTime;
        
        Catch();
        
        if (timer >= PlayerScript.rollDuration)
        {
            timer = 0;
            PlayerScript.PlayerEndedDash?.Invoke();
            if (PlayerScript.moveInput == Vector2.zero)
            {
                PlayerScript.ChangeState(PlayerScript.GetComponent<IdleState>());
            }
            else
            {
                PlayerScript.ChangeState(PlayerScript.GetComponent<MovingState>());
            }
        }
    }

    public void Catch()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, PlayerScript.rollDetectionRadius);
        // Remove any collider in my hitColliders that are not tagged "Ball"
        hitColliders = Array.FindAll(hitColliders, hitCollider => hitCollider.CompareTag("Ball"));
        
        if (hitColliders.Length == 0) return;
        
        GameObject caughtBall = hitColliders[0].gameObject;

        if (caughtBall)
        {
            BallSM ballSM = caughtBall.GetComponent<BallSM>();
            if (ballSM && ballSM.currentState == ballSM.GetComponent<MidAirState>())
            {
                if (timer <= PlayerScript.catchWindow)
                {
                    PlayerScript.heldBall = caughtBall;
                    ballSM.ChangeState(ballSM.GetComponent<InHandState>());
                }
            }
                    
        }
    }
    


    // public void CheckCatch(GameObject caughtObject)
    // {
    //     if (timer <= PlayerScript.catchWindow)
    //     {
    //         PlayerScript.heldBall = caughtObject;
    //         caughtObject.GetComponent<BallSM>().ChangeState(caughtObject.GetComponent<InHandState>());
    //         // Debug.Log("ball caught!");
    //     }
    // }
    
}
