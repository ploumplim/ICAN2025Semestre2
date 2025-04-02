using UnityEngine;

public class LevelState : MonoBehaviour
{
    protected LevelSM LevelSM;
    protected LevelManager LevelManagerScript;
    
    public void Initialize(LevelSM levelSm, LevelManager levelManager)
    {
        this.LevelSM = levelSm;
        this.LevelManagerScript = levelManager;
    }
    
    public virtual void Enter(){}
    public virtual void Tick(){}
    public virtual void Exit(){}
}
