using System;
using Unity.VisualScripting;
using UnityEngine;

public class BallVisuals : MonoBehaviour
{
    // ~~~~~~~~~~~~~~~~VARIABLES~~~~~~~~~~~~~~~~
    // ---------------PUBLIC---------------
    // [Tooltip("Game Object containing the UI image used to position the ball behind walls.")]
    // public GameObject ballMarker;
    [Header("game objects and component settings")]
    [Tooltip("Game Object that holds the trail visuals.")]
    public GameObject trailVisuals;
    [Tooltip("Game Object that holds the ball visuals.")]
    public GameObject ballVisuals;
    [Tooltip("This is the ball's light.")]
    public Light ballLight;
    
    [Header("Trail settings")]
    [Tooltip("Trail color of the ball when flying.")]
    public Color flyingTrailColor = new (1,0,0,0.5f);
    [Tooltip("Trail color of the ball when lethal.")]
    public Color lethalTrailColor = new (0,0,0,0.5f);
    
    [Header("Ball color settings")]
    public Color flyingBallColor = Color.blue;
    public Color lethalBallColor = Color.red;
    public Color caughtBallColor = Color.magenta;
    public Color HitBallColor = Color.yellow;
    public Color groundedBallColor = Color.green;
    
    // ---------------PRIVATE---------------
    private BallSM ballSM;
    private Camera _mainCamera;
    private TrailRenderer _trailRenderer;
    private Material _ballMaterial;

    public void OnEnable()
    {
        ballSM = GetComponent<BallSM>();
        _mainCamera = Camera.main;
        _trailRenderer = trailVisuals.GetComponent<TrailRenderer>();
        _ballMaterial = ballVisuals.GetComponent<MeshRenderer>().material;
        
    }
    
    
    private void FixedUpdate()
    {
        TrailEmitter();
        BallColorAndLight();
    }
    
    
    private void TrailEmitter()
    {
        // Enable or disable the trail based on the ball's state. Enabled when it's midair, disabled otherwise.
        _trailRenderer.emitting = ballSM.currentState.GetType() == typeof(FlyingState) || ballSM.currentState.GetType() == typeof(LethalBallState);
        
        // Get the current speed magnitude from the ball
        float speed = ballSM.GetComponent<Rigidbody>().linearVelocity.magnitude;
        
        // Recover the current width of the trail
        float currentWidth = _trailRenderer.startWidth;
        
        // Change the width of the trail based on the ball's speed. The faster the ball, the wider the trail.
        _trailRenderer.startWidth = Mathf.Lerp(currentWidth, speed / 10, Time.deltaTime);

        switch (ballSM.currentState)
        {
            case FlyingState:
                _trailRenderer.startColor = flyingTrailColor;
                _trailRenderer.endColor = flyingTrailColor;
                break;
            case LethalBallState:
                _trailRenderer.startColor = lethalTrailColor;
                _trailRenderer.endColor = lethalTrailColor;
                break;
        }
        
        
    }
    
    private void BallColorAndLight()
    {
        // Change the color of the ball based on the ball's state. Red when it's midair, green otherwise.

            switch (ballSM.currentState)
            {
                case CaughtState:
                    _ballMaterial.color = caughtBallColor;
                    _ballMaterial.SetColor("_EmissionColor", caughtBallColor);
                    ballLight.color = caughtBallColor;
                    break;
                case HitState:
                    _ballMaterial.color = HitBallColor;
                    _ballMaterial.SetColor("_EmissionColor", HitBallColor);
                    ballLight.color = HitBallColor;
                    break;
                case FlyingState:
                    _ballMaterial.color = flyingBallColor;
                    _ballMaterial.SetColor("_EmissionColor", flyingBallColor);
                    ballLight.color = flyingBallColor;
                    break;
                case DroppedState:
                    _ballMaterial.color = groundedBallColor;
                    _ballMaterial.SetColor("_EmissionColor", groundedBallColor);
                    ballLight.color = groundedBallColor;
                    break;
                // case BuntedBallState:
                //     _ballMaterial.color = buntedBallColor;
                //     _ballMaterial.SetColor("_EmissionColor", buntedBallColor);
                //     ballLight.color = buntedBallColor;
                //     break;
                case LethalBallState:
                    _ballMaterial.color = lethalBallColor;
                    _ballMaterial.SetColor("_EmissionColor", lethalBallColor);
                    ballLight.color = lethalBallColor;
                    break;
            }
            
        
    }
}
