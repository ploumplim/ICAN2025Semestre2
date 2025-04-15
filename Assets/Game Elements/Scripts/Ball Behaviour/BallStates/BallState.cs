using UnityEngine;

public class BallState : MonoBehaviour
{
    protected BallSM BallSm;
    
    //Called on FSM Start
    public void Initialize(BallSM ballSm)
    {
        this.BallSm = ballSm;
    }

    public virtual void Enter() {}
    public virtual void Tick(){}
    public virtual void Exit() {}

    protected void SetParameters(float ballMass, float ballDamp, bool gravityBool)
    {
        BallSm.rb.mass = ballMass;
        BallSm.rb.linearDamping = ballDamp;
        BallSm.rb.useGravity = gravityBool;
    }

    protected void SetBallSpeedMinimum(float currentSpeed, Vector3 ballDirection)
    {
        switch (currentSpeed)
        {
            case > 0f when currentSpeed < BallSm.ballSpeedFloor:
                BallSm.rb.linearVelocity = ballDirection * BallSm.ballSpeedFloor;
                break;
            
            case > 0f when currentSpeed > BallSm.ballSpeedFloor:
                BallSm.ballSpeedFloor = currentSpeed;
                break;
        }
        
        
    }
}
