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

    [Header("Ball Faces settings")]
    public GameObject facesGameObject;
    public SpriteRenderer neutralFace;
    public SpriteRenderer lethalFace;
    public SpriteRenderer hitFace;
    
    [Header("Ball shape settings")]
    public GameObject neutralBall;
    public GameObject lethalBall;
    
    // ---------------PRIVATE---------------
    private BallSM ballSM;
    private Camera _mainCamera;
    private TrailRenderer _trailRenderer;
    private Material _neutralBallMaterial;
    private Material _lethalBallMaterial;

    public void OnEnable()
    {
        ballSM = GetComponent<BallSM>();
        _mainCamera = Camera.main;
        _trailRenderer = trailVisuals.GetComponent<TrailRenderer>();
        _neutralBallMaterial = neutralBall.GetComponent<MeshRenderer>().material;
        _lethalBallMaterial = lethalBall.GetComponent<MeshRenderer>().material;
        
        // Set all faces to inactive at the start.
        neutralFace.gameObject.SetActive(false);
        lethalFace.gameObject.SetActive(false);
        hitFace.gameObject.SetActive(false);
        
    }
    
    
    private void FixedUpdate()
    {
        TrailEmitter();
        BallColorAndLight();
        UpdateFace();
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

    private void UpdateFace()
    {
        switch (ballSM.currentState)
        {
            case FlyingState:
                // Set the ball shape to neutral.
                neutralBall.SetActive(true);
                lethalBall.SetActive(false);
                
                // Set the neutral face to active and the others to inactive.
                neutralFace.gameObject.SetActive(true);
                lethalFace.gameObject.SetActive(false);
                hitFace.gameObject.SetActive(false);
                break;
            case DroppedState:
                neutralBall.SetActive(true);
                lethalBall.SetActive(false);
                
                neutralFace.gameObject.SetActive(true);
                lethalFace.gameObject.SetActive(false);
                hitFace.gameObject.SetActive(false);
                break;
            case CaughtState:
                neutralBall.SetActive(true);
                lethalBall.SetActive(false);
                
                neutralFace.gameObject.SetActive(false);
                lethalFace.gameObject.SetActive(false);
                hitFace.gameObject.SetActive(true);
                break;    
            case HitState:
                neutralBall.SetActive(true);
                lethalBall.SetActive(false);
                
                neutralFace.gameObject.SetActive(false);
                lethalFace.gameObject.SetActive(false);
                hitFace.gameObject.SetActive(true);
                break;
            case LethalBallState:
                neutralBall.SetActive(false);
                lethalBall.SetActive(true);
                
                neutralFace.gameObject.SetActive(false);
                lethalFace.gameObject.SetActive(true);
                hitFace.gameObject.SetActive(false);
                break;
        }
        
        //Make the face always look towards the main camera using the facesGameObject on the X axis.
        Vector3 lookDirection = _mainCamera.transform.position - facesGameObject.transform.position;
        lookDirection.x = 0;
        lookDirection.Normalize();
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        facesGameObject.transform.rotation = Quaternion.Slerp(facesGameObject.transform.rotation, lookRotation, Time.deltaTime * 5f);
        
        
    }
    
    private void BallColorAndLight()
    {
        // Change the color of the ball based on the ball's state. Red when it's midair, green otherwise.

            switch (ballSM.currentState)
            {
                case CaughtState:
                    _neutralBallMaterial.color = caughtBallColor;
                    _neutralBallMaterial.SetColor("_EmissionColor", caughtBallColor);
                    ballLight.color = caughtBallColor;
                    break;
                case HitState:
                    _neutralBallMaterial.color = HitBallColor;
                    _neutralBallMaterial.SetColor("_EmissionColor", HitBallColor);
                    ballLight.color = HitBallColor;
                    break;
                case FlyingState:
                    _neutralBallMaterial.color = flyingBallColor;
                    _neutralBallMaterial.SetColor("_EmissionColor", flyingBallColor);
                    ballLight.color = flyingBallColor;
                    break;
                case DroppedState:
                    _neutralBallMaterial.color = groundedBallColor;
                    _neutralBallMaterial.SetColor("_EmissionColor", groundedBallColor);
                    ballLight.color = groundedBallColor;
                    break;
                // case BuntedBallState:
                //     _ballMaterial.color = buntedBallColor;
                //     _ballMaterial.SetColor("_EmissionColor", buntedBallColor);
                //     ballLight.color = buntedBallColor;
                //     break;
                case LethalBallState:
                    _lethalBallMaterial.color = lethalBallColor;
                    _lethalBallMaterial.SetColor("_EmissionColor", lethalBallColor);
                    _neutralBallMaterial.color = lethalBallColor;
                    _neutralBallMaterial.SetColor("_EmissionColor", lethalBallColor);
                    ballLight.color = lethalBallColor;
                    break;
            }
            
        
    }
}
