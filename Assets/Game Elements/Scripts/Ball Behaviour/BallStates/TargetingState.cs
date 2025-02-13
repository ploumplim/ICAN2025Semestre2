using UnityEngine;

public class TargetingState : BallState
{
    private float _targetTimer;
    
    
    public override void Enter()
    {
        base.Enter();
        _targetTimer = 0;
    }


    public override void Tick()
    {
        base.Tick();
        // Run the targetTimer. Once its over, change state to "MidAir".
        _targetTimer += Time.deltaTime;
        
        // Also, create a sphere that detects any "Target" objects within its detection radius.
        Collider[] targetColliders = new Collider[1];
        var size = Physics.OverlapSphereNonAlloc(BallSm.transform.position, BallSm.detectionRadius, targetColliders);
        // remove all targets that don't have the "Target" tag
        targetColliders = System.Array.FindAll(targetColliders, x => x.CompareTag("Target"));
        
        if (targetColliders.Length > 0)
        {
            // Debug.Log("Target detected");
            // Sort the colliders by distance to the ball, so the closest one is the first one in the array.
            System.Array.Sort(targetColliders, (x, y) =>
                Vector3.Distance(x.transform.position, BallSm.transform.position)
                    .CompareTo(Vector3.Distance(y.transform.position, BallSm.transform.position)));
            
            // set the ball facing the target
            BallSm.transform.LookAt(targetColliders[0].transform);
            
            // throw the ball towards the target
            BallSm.Bounce();
            
            // change state to "MidAir"
            BallSm.ChangeState(BallSm.GetComponent<MidAirState>());
            
            
        }
        if (_targetTimer >= BallSm.targetingTime)
        {
            BallSm.ChangeState(BallSm.GetComponent<MidAirState>());
        }


    }

    public override void Exit()
    {
        base.Exit();
        _targetTimer = 0;
    }
}
