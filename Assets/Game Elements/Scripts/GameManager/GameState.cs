using UnityEngine;

public class GameState : MonoBehaviour
{
    public GameManagerSM GameManagerSM;
    protected GameManager GameManager;
    //Called on FSM Start
    public void Initialize(GameManagerSM ballSm, GameManager gameManager)
    {
        this.GameManagerSM = ballSm;
        this.GameManager = gameManager;
    }

    public virtual void Enter(){}
    public virtual void Tick(){}
    public virtual void Exit(){}
}
