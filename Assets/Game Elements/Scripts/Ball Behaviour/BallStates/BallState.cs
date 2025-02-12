using UnityEngine;

public class BallState : MonoBehaviour
{
    protected BallSM BallSm;
    
    //Called on FSM Start
    public void Initialize(BallSM ballSm)
    {
        this.BallSm = ballSm;
    }

    public virtual void Enter(){}
    public virtual void Tick(){}
    public virtual void Exit(){}
}
