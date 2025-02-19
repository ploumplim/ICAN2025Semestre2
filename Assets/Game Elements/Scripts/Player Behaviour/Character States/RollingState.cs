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
        if (timer >= PlayerScript.rollDuration)
        {
            timer = 0;
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

    public void CheckCatch(GameObject caughtObject)
    {
        if (timer <= PlayerScript.catchWindow)
        {
            PlayerScript.heldBall = caughtObject;
            caughtObject.GetComponent<BallSM>().ChangeState(caughtObject.GetComponent<InHandState>());
            Debug.Log("ball caught!");
        }
    }
    
}
