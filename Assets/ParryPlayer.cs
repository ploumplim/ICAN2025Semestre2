using UnityEngine;

public class ParryPlayer : MonoBehaviour
{
    private Collider _col;
    public bool canParry;
    private GameObject _ballToParry;
    public float parryForce = 10f; // Adjust the force as needed

    private void Start()
    {
        _col = GetComponent<Collider>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other)
        {
            BallSM ballSM = other.GetComponent<BallSM>();
            if (ballSM != null && ballSM.currentState == ballSM.GetComponent<MidAirState>())
            {
                canParry = true;
                _ballToParry = other.gameObject;
            }
        }
    }

    public void Parry()
    {
        if (canParry && _ballToParry != null)
        {
            Debug.Log("Aled");
            Rigidbody ballRigidbody = _ballToParry.GetComponent<Rigidbody>();
            if (ballRigidbody != null)
            {
                ballRigidbody.AddForce(Vector3.up * parryForce, ForceMode.Impulse);
            }
            canParry = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (_col != null)
        {
            Gizmos.color = Color.green;
            if (_col is BoxCollider)
            {
                BoxCollider box = (BoxCollider)_col;
                Gizmos.DrawWireCube(box.bounds.center, box.bounds.size);
            }
            else if (_col is SphereCollider)
            {
                SphereCollider sphere = (SphereCollider)_col;
                Gizmos.DrawWireSphere(sphere.bounds.center, sphere.bounds.size.x / 2);
            }
            // Add more collider types if needed
        }
    }
}