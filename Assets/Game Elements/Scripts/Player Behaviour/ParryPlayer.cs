using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;


public class ParryPlayer : MonoBehaviour
{
    private PlayerScript _playerScript;
    private BallSM _ballSM;
    
    private Collider _col;
    private bool canParry;
    private GameObject _ballToParry;
    private float parryForce;

    private float _currentBallSpeed;

    private void Start()
    {
        _playerScript = GetComponentInParent<PlayerScript>();
        _col = GetComponent<Collider>();
        parryForce = _playerScript.parryForce;
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            _ballSM = other.GetComponent<BallSM>();
            if (_ballSM != null && _ballSM.currentState == _ballSM.GetComponent<MidAirState>())
            {
                canParry = true;
                _ballToParry = other.gameObject;
                _ballSM.canBeParriedEvent?.Invoke();
                _playerScript.CanParryTheBallEvent?.Invoke();
                _currentBallSpeed = _ballToParry.GetComponent<Rigidbody>().linearVelocity.magnitude;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            canParry = false;
            _ballSM.cannotBeParriedEvent?.Invoke();
            _playerScript.CannotParryTheBallEvent?.Invoke();
            _ballToParry = null;
        }
    }

    public void Parry()
    {
        if (canParry && _ballToParry)
        {
            // Debug.Log("Aled");
            Rigidbody ballRigidbody = _ballToParry.GetComponent<Rigidbody>();
            if (ballRigidbody != null)
            {
                ballRigidbody.linearVelocity = Vector3.zero;
                ballRigidbody.AddForce(_playerScript.gameObject.transform.forward * parryForce * _currentBallSpeed, ForceMode.Impulse);
                _ballSM.ChangeState(_ballSM.GetComponent<TargetingState>());
            }
            canParry = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (_col != null)
        {
            Gizmos.color = new Color(1, 0, 1); 
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