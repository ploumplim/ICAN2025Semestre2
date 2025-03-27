using UnityEngine;

public class PlayerState : MonoBehaviour
{
    protected PlayerScript PlayerScript;
    
    public void Initialize(PlayerScript playerScript)
    {
        this.PlayerScript = playerScript;
    }
    
    public virtual void Enter(){}
    
    public virtual void Tick(){}
    
    public virtual void Exit(){}
}
