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
    
    [FormerlySerializedAs("hasParried")] [HideInInspector] public bool playerHasParried;
    [FormerlySerializedAs("_Timer")] [HideInInspector] public float parryTimer;

    private void Start()
    {
        _playerScript = GetComponentInParent<PlayerScript>();
        _col = GetComponent<Collider>();
        parryForce = _playerScript.parryForce;
    }

    private void FixedUpdate()
    {
        if (playerHasParried)
        {
            // Debug.Log("Aled");
            parryTimer += Time.deltaTime;
            if (canParry && parryTimer <= _playerScript.parryWindow)
            {
                Parry();    
            }
        }
        else
        {
            parryTimer = 0;
        }
        
        
        if (parryTimer >= _playerScript.parryCooldown)
        {
            playerHasParried = false;
            canParry = false;
            parryTimer = 0;
            // Debug.Log("Parry cooldown over");
        }
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
            parryTimer = 0;
        }
    }

    public void Parry()
    {
        if (canParry && _ballToParry)
        {
            // Debug.Log("Aled");
            _currentBallSpeed = _ballToParry.GetComponent<Rigidbody>().linearVelocity.magnitude;
            Rigidbody ballRigidbody = _ballToParry.GetComponent<Rigidbody>();
            if (ballRigidbody != null)
            {
                ballRigidbody.linearVelocity = Vector3.zero;
                // Calculate the vector between the player and the ball.
                Vector3 direction = _ballToParry.transform.position - transform.position;
                
                ballRigidbody.AddForce(direction * (parryForce * _currentBallSpeed), ForceMode.Impulse);
                _ballSM.ChangeState(_ballSM.GetComponent<TargetingState>());
                parryTimer = 0;
                canParry = false;
                playerHasParried = false;
            }
            
        }
    }

    // private void OnDrawGizmos()
    // {
    //     if (_col != null)
    //     {
    //         Gizmos.color = new Color(1, 0, 1); 
    //         if (_col is BoxCollider)
    //         {
    //             BoxCollider box = (BoxCollider)_col;
    //             Gizmos.DrawWireCube(box.bounds.center, box.bounds.size);
    //         }
    //         else if (_col is SphereCollider)
    //         {
    //             SphereCollider sphere = (SphereCollider)_col;
    //             Gizmos.DrawWireSphere(sphere.bounds.center, sphere.bounds.size.x / 2);
    //         }
    //         // Add more collider types if needed
    //     }
    // }
}