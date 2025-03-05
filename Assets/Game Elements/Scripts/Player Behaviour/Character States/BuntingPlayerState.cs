using UnityEngine;

public class BuntingPlayerState : PlayerState
{
    [HideInInspector] public float timer;
    public override void Enter()
    {
        base.Enter();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward * 
            PlayerScript.buntSpherePositionOffset,
            PlayerScript.buntSphereRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.GetComponent<BallSM>())
            {
                hitCollider.GetComponent<BallSM>().ChangeState(hitCollider.GetComponent<BuntedBallState>());
                hitCollider.GetComponent<Rigidbody>().AddForce(transform.up * PlayerScript.buntForce, ForceMode.Impulse);
            }
        }
    }

    public override void Tick()
    {
        base.Tick();
        timer += Time.deltaTime;
        if (timer >= PlayerScript.buntCooldown)
        {
            PlayerScript.ChangeState(GetComponent<NeutralState>());
        }
    }

    public override void Exit()
    {
        base.Exit();
        timer = 0;
    }
}
