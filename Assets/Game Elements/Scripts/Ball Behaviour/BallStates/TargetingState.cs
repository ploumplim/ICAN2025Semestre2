using UnityEngine;

public class TargetingState : BallState
{
    private float _targetTimer;
    private Collider[] _targetColliders = new Collider[10]; // Pre-allocated array for potential targets

    public override void Enter()
    {
        base.Enter();
        _targetTimer = 0;

        // make the radius of the sphere collider bigger depending on the speed of the ball
        BallSm.detectionRadius = BallSm.rb.linearVelocity.magnitude * BallSm.detectionRadiusMultiplier;
        
        
        // Detect potential targets within the detection radius
        int size = Physics.OverlapSphereNonAlloc(BallSm.transform.position, BallSm.detectionRadius, _targetColliders);

        // Filter targets to include only those with the "Target" tag
        Collider[] validTargets = System.Array.FindAll(_targetColliders, collider => collider != null && collider.CompareTag("Target"));

        if (validTargets.Length > 0)
        {
            // Sort the colliders by distance to the ball, so the closest one is the first one in the array
            System.Array.Sort(validTargets, (x, y) =>
                Vector3.Distance(x.transform.position, BallSm.transform.position)
                    .CompareTo(Vector3.Distance(y.transform.position, BallSm.transform.position)));

            // Set the ball facing the target
            BallSm.transform.LookAt(validTargets[0].transform);

            // Push the ball towards the target
            BallSm.Bounce();

            // Change state to "MidAir"
            BallSm.ChangeState(BallSm.GetComponent<MidAirState>());
        }
    }

    public override void Tick()
    {
        base.Tick();
        _targetTimer += Time.deltaTime;

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