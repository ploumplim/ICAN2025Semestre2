using System;
using UnityEngine;

namespace TopViewScript
{
    public class Bullet : MonoBehaviour
    {
        public float speed = 20f;
        public Rigidbody rb;

        private void Start()
        {
            rb.GetComponent<Rigidbody>();
            rb.velocity = transform.forward * speed;
        }

        private void OnCollisionEnter(Collision other)
        {
            Debug.Log("Hit: " + other.gameObject.name);
        }
    }
}