using UnityEngine;

public class KnockbackState : PlayerState
{
    private float _timer;
    public override void Enter()
    {
        base.Enter();
        _timer = 0;
        PlayerScript.rb.linearDamping = PlayerScript.hitLinearDrag;
    }

    public override void Tick()
    {
        base.Tick();
        _timer += Time.deltaTime;
        if (_timer >= PlayerScript.knockbackTime)
        {
            PlayerScript.ChangeState(GetComponent<NeutralState>());
        }
    }

    public override void Exit()
    {
        base.Exit();
        PlayerScript.rb.linearDamping = PlayerScript.linearDrag;
    }
}
