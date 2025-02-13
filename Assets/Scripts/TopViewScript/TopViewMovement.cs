using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopViewMovement : MonoBehaviour
{
    public float speed = 5f;
    public float bulletFireSpeed = 20f;
    public GameObject bulletPrefab; // Prefab of the bullet
    public Transform bulletSpawnPoint; // Spawn point of the bullet
    public GameObject target; // Target GameObject

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        MoveCharacter();

        if (Input.GetButtonDown("Fire1")) // Assuming left mouse button or a specific key is used to shoot
        {
            Shoot();
        }
    }

    void MoveCharacter()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveX, 0, moveY) * (speed * Time.deltaTime);
        rb.MovePosition(transform.position + movement);

        if (movement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, speed * 100 * Time.deltaTime);
        }
    }

    void Shoot()
    {
        if (target == null) return;

        Vector3 direction = (target.transform.position - bulletSpawnPoint.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.LookRotation(direction));
        
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            bulletRb.velocity = direction * bulletFireSpeed;
        }
    }
}
