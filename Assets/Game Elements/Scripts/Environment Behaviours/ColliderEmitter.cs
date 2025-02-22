using System;
using Unity.VisualScripting;
using UnityEngine;

public class ColliderEmitter : MonoBehaviour
{

    // This script invokes a Unity Event when a collider is hit.
    // We can control the tag that the collider looks for with a variable.
    
    public string tagToLookFor;
    public UnityEngine.Events.UnityEvent onColliderHit;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(tagToLookFor))
        {
            onColliderHit?.Invoke();
        }
    }
}
