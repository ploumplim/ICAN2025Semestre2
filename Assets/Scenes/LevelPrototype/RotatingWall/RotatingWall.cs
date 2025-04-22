using UnityEngine;

public class RotatingWall : MonoBehaviour
{
    
    [SerializeField] private float rotationSpeed = 100f; // Vitesse de rotation
    [SerializeField] private float rotationDuration = 2f; // Durée de rotation dans un sens

    private float timer = 0f;
    private int rotationDirection = 1; // 1 pour sens horaire, -1 pour sens antihoraire

    void Update()
    {
        // Rotation de l'objet
        transform.Rotate(Vector3.up * (rotationSpeed * rotationDirection * Time.deltaTime));

        // Mise à jour du timer
        timer += Time.deltaTime;

        // Changement de direction après la durée définie
        if (timer >= rotationDuration)
        {
            rotationDirection *= -1; // Inverse la direction
            timer = 0f; // Réinitialise le timer
        }
    }
}
