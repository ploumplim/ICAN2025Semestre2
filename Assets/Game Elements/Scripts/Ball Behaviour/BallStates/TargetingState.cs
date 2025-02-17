using UnityEngine;

public class TargetingState : BallState
{
    private Collider[] _targetColliders = new Collider[10]; // Pre-allocated array for potential targets
    public override void Enter()
    {
        BallSm.bounces += 1;
        base.Enter();

        // Detect potential targets within the detection radius
        int size = Physics.OverlapSphereNonAlloc(BallSm.transform.position, BallSm.speedModifiedDetectionRadius,
            _targetColliders);

        // Filter targets to include only those with the "Target" tag
        Collider[] validTargets = System.Array.FindAll(_targetColliders,
            collider => collider != null && collider.CompareTag("Target"));

        if (validTargets.Length > 0)
        {
            
            
            // Sort the colliders by distance to the ball, so the closest one is the first one in the array
            System.Array.Sort(validTargets, (x, y) =>
                Vector3.Distance(x.transform.position, BallSm.transform.position)
                    .CompareTo(Vector3.Distance(y.transform.position, BallSm.transform.position)));

            // Calculate the direction to the target
            Vector3 directionToTarget = (validTargets[0].transform.position - BallSm.transform.position).normalized;

            // Set the ball facing the target using Quaternion.LookRotation
            BallSm.transform.rotation = Quaternion.LookRotation(directionToTarget);

            // Push the ball towards the target
            BallSm.Bounce();
            
            // Debug all data from the current bounce
            Debug.Log("Target found: "+ validTargets[0].name + " Distance: " + Vector3.Distance(validTargets[0].transform.position, BallSm.transform.position));
            
            // Change state to "MidAir"
            BallSm.ChangeState(BallSm.GetComponent<MidAirState>());
        }
        else
        {
            // If no targets are found, change state to "MidAir"
            BallSm.ChangeState(BallSm.GetComponent<MidAirState>());
        }

    }
    
    public override void Exit()
    {
        base.Exit();
        // reset the target colliders array
        _targetColliders = new Collider[10];
    }

}


