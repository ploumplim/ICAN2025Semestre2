using UnityEngine;

public class ConeDetector : MonoBehaviour
{
    [Header("Détection")]
    public float detectionRadius = 5f; // Distance de détection
    public float detectionAngle = 45f; // Demi-angle du cône
    public float sphereCastRadius = 0.5f; // Rayon du SphereCast
    public LayerMask detectionMask; // Masque de détection

    [Header("Taille du cône")]
    public float scaleX = 1f; // Largeur du cône
    public float scaleY = 1f; // Hauteur du cône
    public float scaleZ = 1f; // Profondeur du cône

    void Update()
    {
        DetectObjectsInCone();
    }

    void DetectObjectsInCone()
    {
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, detectionRadius, detectionMask);

        foreach (Collider obj in objectsInRange)
        {
            Vector3 directionToTarget = (obj.transform.position - transform.position).normalized;
            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

            if (angleToTarget < detectionAngle) // Vérifie si l'objet est dans le cône
            {
                RaycastHit hit;
                if (Physics.SphereCast(transform.position, sphereCastRadius, directionToTarget, out hit, detectionRadius, detectionMask))
                {
                    if (hit.collider == obj) // Vérification si c'est bien la cible détectée
                    {
                        Debug.Log($"Objet validé : {obj.name}");
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Dessine la sphère de détection
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, detectionRadius);

        // Dessine un cône en 3D (lampe torche) avec scale X, Y, Z
        Gizmos.color = Color.yellow;
        Draw3DCone(transform.position, transform.forward, detectionRadius, detectionAngle, scaleX, scaleY, scaleZ);
    }

    void Draw3DCone(Vector3 position, Vector3 direction, float length, float angle, float scaleX, float scaleY, float scaleZ)
    {
        int segments = 20;
        Vector3[] circlePoints = new Vector3[segments];

        for (int i = 0; i < segments; i++)
        {
            float theta = (i / (float)segments) * Mathf.PI * 2f;
            float x = Mathf.Cos(theta) * length * Mathf.Tan(Mathf.Deg2Rad * angle) * scaleX;
            float y = Mathf.Sin(theta) * length * Mathf.Tan(Mathf.Deg2Rad * angle) * scaleY;
            float z = Mathf.Cos(theta) * length * Mathf.Tan(Mathf.Deg2Rad * angle) * scaleZ;
            circlePoints[i] = position + (direction * length) + (transform.right * x) + (transform.up * y) + (transform.forward * z);
        }

        // Dessine la base circulaire et relie les points
        for (int i = 0; i < segments; i++)
        {
            Gizmos.DrawLine(circlePoints[i], circlePoints[(i + 1) % segments]);
            Gizmos.DrawLine(position, circlePoints[i]); // Relie le centre au bord
        }
    }
}
