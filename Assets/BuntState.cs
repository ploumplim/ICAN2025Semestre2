using UnityEngine;

public class BuntState : BallState
{
    public override void Enter()
    {
        base.Enter();
        
        // Réinitialiser les forces et appliquer un premier coup vers le haut
        Rigidbody rb = BallSm.GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(Vector3.up * 100, ForceMode.Impulse);
        
        // Écouter les collisions avec le sol
        BallSm.OnBallHitFloor += ApplyGroundImpulse;
    }

    public override void Exit()
    {
        // Arrêter d'écouter l'événement quand on quitte l'état
        BallSm.OnBallHitFloor -= ApplyGroundImpulse;
    }

    private void ApplyGroundImpulse()
    {
        Debug.LogWarning("Test");
        // Ajouter une impulsion supplémentaire quand la balle touche le sol
        Rigidbody rb = BallSm.GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
    }
}