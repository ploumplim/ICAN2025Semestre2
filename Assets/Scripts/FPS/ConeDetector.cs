using System;
using UnityEngine;

public class ConeDetector : MonoBehaviour
{
    [Header("Détection")]
    public float detectionAngle = 45f; // Demi-angle du cône
    public float sphereCastRadius = 0.5f; // Rayon du SphereCast
    public LayerMask detectionMask; // Masque de détection

    [Header("Paramètres de la sphère")]
    public float sphereRadius = 5f; // **Taille de la sphère de détection**

    [Header("Paramètres du cône")]
    public float coneDistance = 5f; // **Distance maximale du cône**
    public float coneWidth = 1f; // Largeur du cône (X)
    public float coneHeight = 1f; // Hauteur du cône (Y)

    void Update()
    {
        DetectObjects();
    }

    void DetectObjects()
    {
        Collider[] objectsInSphere = Physics.OverlapSphere(transform.position, sphereRadius, detectionMask);

        foreach (Collider obj in objectsInSphere)
        {
            Vector3 directionToTarget = (obj.transform.position - transform.position);
           

            RaycastHit hit;
            if (Physics.SphereCast(transform.position, sphereCastRadius, directionToTarget, out hit, coneDistance, detectionMask))
            {
                if (hit.collider == obj) // Vérifie si c'est bien la cible détectée
                {
                    Debug.Log($"Objet validé : {obj.name}");
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TRIGGER ENTER");
    }

    // private void OnDrawGizmos()
    // {
    //     // Dessine la sphère de détection (rouge)
    //     Gizmos.color = new Color(1, 0, 0, 0.3f);
    //     Gizmos.DrawSphere(transform.position, sphereRadius);
    //
    //     // Dessine un cône 3D (lampe torche) avec coneDistance ajustable
    //     Gizmos.color = Color.yellow;
    //     Draw3DCone(transform.position, transform.forward, coneDistance, detectionAngle, coneWidth, coneHeight);
    // }
    //
    // void Draw3DCone(Vector3 position, Vector3 direction, float length, float angle, float width, float height)
    // {
    //     int segments = 20;
    //     Vector3[] circlePoints = new Vector3[segments];
    //
    //     for (int i = 0; i < segments; i++)
    //     {
    //         float theta = (i / (float)segments) * Mathf.PI * 2f;
    //         float x = Mathf.Cos(theta) * length * Mathf.Tan(Mathf.Deg2Rad * angle) * width;
    //         float y = Mathf.Sin(theta) * length * Mathf.Tan(Mathf.Deg2Rad * angle) * height;
    //         circlePoints[i] = position + (direction * length) + (transform.right * x) + (transform.up * y);
    //     }
    //
    //     // Dessine la base circulaire et relie les points
    //     for (int i = 0; i < segments; i++)
    //     {
    //         Gizmos.DrawLine(circlePoints[i], circlePoints[(i + 1) % segments]);
    //         Gizmos.DrawLine(position, circlePoints[i]); // Relie le centre au bord
    //     }
    // }
}
