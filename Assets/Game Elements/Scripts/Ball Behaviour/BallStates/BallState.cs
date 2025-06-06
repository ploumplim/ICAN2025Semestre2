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

    public virtual void Tick()
    {
        BallSm.SetMaxHeight(BallSm.minHeight, BallSm.flyingMaxHeight);
        BallSm.FixVerticalSpeed();
    }
    public virtual void Exit() {}

    protected void SetParameters(float ballMass, float ballDamp, bool gravityBool)
    {
        BallSm.rb.mass = ballMass;
        BallSm.rb.linearDamping = ballDamp;
        BallSm.rb.useGravity = gravityBool;
    }
    
}
