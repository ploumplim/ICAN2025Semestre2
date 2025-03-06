using UnityEngine;

public class GameState : MonoBehaviour
{
    protected GameManagerSM BallSm;
    
    //Called on FSM Start
    public void Initialize(GameManagerSM ballSm)
    {
        this.BallSm = ballSm;
    }

    public virtual void Enter(){}
    public virtual void Tick(){}
    public virtual void Exit(){}
}
