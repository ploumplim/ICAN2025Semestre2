using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public GameObject ballPrefab;
    public Transform handPosition;
    public Camera playerCamera;
    public float throwForce = 10f;
    public float mouseSensitivity = 2f;
    public float minYRotation = -90f, maxYRotation = 90f;
    public PhysicMaterial bouncyMaterial;
    
    private CharacterController controller;
    private GameObject heldBall;
    private float xRotation = 0f;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SpawnBall();
    }

    void Update()
    {
        MovePlayer();
        RotatePlayer();
        if (Input.GetMouseButtonDown(0))
        {
            ThrowBall();
        }
    }

    void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal") * speed;
        float moveZ = Input.GetAxis("Vertical") * speed;
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * Time.deltaTime);
    }

    void RotatePlayer()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        transform.Rotate(Vector3.up * mouseX);
        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minYRotation, maxYRotation);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void SpawnBall()
    {
        heldBall = Instantiate(ballPrefab, handPosition.position, Quaternion.identity);
        heldBall.transform.SetParent(handPosition);
        Rigidbody rb = heldBall.GetComponent<Rigidbody>();
        rb.isKinematic = true; // La balle reste fixe tant qu'elle est tenue
        Collider ballCollider = heldBall.GetComponent<Collider>();
        if (bouncyMaterial != null && ballCollider != null)
        {
            ballCollider.material = bouncyMaterial; // Applique le matériau rebondissant
        }
    }

    void ThrowBall()
    {
        if (heldBall != null)
        {
            heldBall.transform.SetParent(null);
            Rigidbody rb = heldBall.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.position = handPosition.position; // Assure que la balle part bien de la main
            rb.velocity = Vector3.zero; // Réinitialise toute vitesse précédente
            rb.angularVelocity = Vector3.zero; // Empêche la balle de rouler avant le lancer
            rb.AddForce(playerCamera.transform.forward * throwForce, ForceMode.Impulse);
            heldBall = null;
            Invoke("SpawnBall", 2f); // Respawn une balle après 2 secondes
        }
    }
}
