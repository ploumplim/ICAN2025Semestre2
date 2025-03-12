using System;
using Unity.VisualScripting;
using UnityEngine;

public class BallVisuals : MonoBehaviour
{
    // ~~~~~~~~~~~~~~~~VARIABLES~~~~~~~~~~~~~~~~
    // ---------------PUBLIC---------------
    // [Tooltip("Game Object containing the UI image used to position the ball behind walls.")]
    // public GameObject ballMarker;
    [Tooltip("Game Object that holds the trail visuals.")]
    public GameObject trailVisuals;
    [Tooltip("Game Object that holds the ball visuals.")]
    public GameObject ballVisuals;
    [Tooltip("The ball turns into this color when it can be parried.")]
    public Color parryColor;
    [Tooltip("This is the ball's light.")]
    public Light ballLight;
    // ---------------PRIVATE---------------
    private BallSM ballSM;
    private Camera _mainCamera;
    private TrailRenderer _trailRenderer;
    private Material _ballMaterial;
    private Color _originalColor;

    public void OnEnable()
    {
        ballSM = GetComponent<BallSM>();
        _mainCamera = Camera.main;
        _trailRenderer = trailVisuals.GetComponent<TrailRenderer>();
        _ballMaterial = ballVisuals.GetComponent<MeshRenderer>().material;
        _originalColor = _ballMaterial.color;
        
    }
    
    
    private void FixedUpdate()
    {
        BallMarker();
        TrailEmitter();
        BallColorAndLight();
    }

    private void BallMarker()
    {
        //------------------------------------------------------------------------
        // Convert the ball's position to screen space
        // Vector3 screenPosition = _mainCamera.WorldToScreenPoint(transform.position);
        //
        // // Define a layer mask to exclude the canvas layer (assuming the canvas is on a layer named "UI")
        // int UILayerMask = ~LayerMask.GetMask("UI");
        //
        // // Define the layer mask for the wall (assuming the wall is on a layer named "Wall")
        // int wallLayerMask = LayerMask.GetMask("Wall");

        // Check if the ball is behind a wall, excluding the canvas layer
        // bool isBehindWall = false;
        // if (Physics.Linecast(_mainCamera.transform.position, transform.position, out RaycastHit hit, UILayerMask))
        // {
        //     if (((1 << hit.transform.gameObject.layer) & wallLayerMask) != 0)
        //     {
        //         isBehindWall = true;
        //     }
        // } 
        // Update the position of the ball marker in the canvas
        // ballMarker.transform.position = screenPosition;
        // ballMarker.SetActive(isBehindWall);
        
        
        //------------------------------------------------------------------------
        
        // // If the ball leaves the screen, make the ball marker stick to the edge of the screen, relative to the ball's position.
        // if (screenPosition.x < 0 || screenPosition.x > Screen.width || screenPosition.y < 0 ||
        //     screenPosition.y > Screen.height)
        // {
        //     // If the ball leaves the screen, make the ball marker stick to the edge of the screen, relative to the ball's position.
        //     screenPosition.x = Mathf.Clamp(screenPosition.x, 0, Screen.width);
        //     screenPosition.y = Mathf.Clamp(screenPosition.y, 0, Screen.height);
        // }
        
        
        // // Log detailed information about the hit
        // if (isBehindWall)
        // {
        //     Debug.Log($"Ball is behind wall. Hit object: {hit.transform.name}, Layer: {LayerMask.LayerToName(hit.transform.gameObject.layer)}");
        // }
        // else
        // {
        //     Debug.Log("Ball is not behind wall.");
        // }
    }
    
    private void TrailEmitter()
    {
        // Enable or disable the trail based on the ball's state. Enabled when it's midair, disabled otherwise.
        _trailRenderer.emitting = ballSM.currentState.GetType() == typeof(FlyingState);
        
        // Get the current speed magnitude from the ball
        float speed = ballSM.GetComponent<Rigidbody>().linearVelocity.magnitude;
        
        // Recover the current width of the trail
        float currentWidth = _trailRenderer.startWidth;
        
        // Change the width of the trail based on the ball's speed. The faster the ball, the wider the trail.
        _trailRenderer.startWidth = Mathf.Lerp(currentWidth, speed / 10, Time.deltaTime);
        
        
    }
    
    private void BallColorAndLight()
    {
        // Change the color of the ball based on the ball's state. Red when it's midair, green otherwise.

            switch (ballSM.currentState)
            {
                case FlyingState:
                    _ballMaterial.color = Color.red;
                    _ballMaterial.SetColor("_EmissionColor", Color.red);
                    ballLight.color = Color.red;
                    break;
                case DroppedState:
                    _ballMaterial.color = Color.green;
                    _ballMaterial.SetColor("_EmissionColor", Color.green);
                    ballLight.color = Color.green;
                    break;
                case BuntedBallState:
                    _ballMaterial.color = Color.magenta;
                    _ballMaterial.SetColor("_EmissionColor", Color.magenta);
                    ballLight.color = Color.magenta;
                    break;
            }
            
        
    }
}
