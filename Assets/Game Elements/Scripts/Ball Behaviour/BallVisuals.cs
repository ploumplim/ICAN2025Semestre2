using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

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
    
    [FormerlySerializedAs("flyingTrailColor")]
    [Header("Trail settings")]
    [Tooltip("Trail color of the ball when flying.")]
    public Color startFlyingTrailColor = new (1,0,0,0.5f);
    [FormerlySerializedAs("lethalTrailColor")] [Tooltip("Trail color of the ball when lethal.")]
    public Color startLethalTrailColor = new (0,0,0,0.5f);
    [Tooltip("End color of the trail when flying.")]
    public Color endFlyingTrailColor = new (1,0,0,0f);
    [Tooltip("End color of the trail when lethal.")]
    public Color endLethalTrailColor = new (0,0,0,0f);
    
    public float trailTimeModifier = 0.1f;
    
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
    
    [Header("Ball Impact settings")]
    public ParticleSystem impactParticle;
    public float particleSizeModifier = 0.1f;
    [FormerlySerializedAs("particleImpactZoneSize")] public float particleImpactZoneSizeModifier = 0.1f;
    
    [Header("Ball catch settings")]
    public ParticleSystem catchParticle;
    
    [Header("Perfect hit settings")]
    public ParticleSystem perfectHitParticle;
    public float perfectHitThreshold = 0.95f;
    
    [Header("Lethal ball settings")]
    public ParticleSystem lethalBallParticle;
    
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
        
        // float ballSize = ballSM.GetComponent<Transform>().localScale.x;
        //Recover the current width of the trail
        // float currentWidth = _trailRenderer.startWidth;
        
        //Change the width of the trail based on the ball's speed. The faster the ball, the wider the trail.
        // _trailRenderer.startWidth = Mathf.Lerp(currentWidth, currentWidth * ballSize, Time.deltaTime);

        _trailRenderer.time = speed * trailTimeModifier; 
        
        
        switch (ballSM.currentState)
        {
            case FlyingState:
                _trailRenderer.startColor = startFlyingTrailColor;
                _trailRenderer.endColor = endFlyingTrailColor;
                break;
            case LethalBallState:
                _trailRenderer.startColor = startLethalTrailColor;
                _trailRenderer.endColor = endLethalTrailColor;
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

    public void OnBallCaught()
    {
        // play the catch particle system.
        catchParticle.Play();
        
    }

    public void StopCatchAnimation()
    {
        // stop the catch particle system.
        catchParticle.Stop();
    }

    public void OnPerfectHit()
    {
        // recover ball size
        float ballSize = ballSM.GetComponent<Transform>().localScale.x;
        // Set the size of the perfect hit particle system based on the ball's size.
        ParticleSystem.MainModule mainModule = perfectHitParticle.main;
        
        // From the charge state, recover the ball owning player's charge value.
        PlayerScript ballOwnerPlayerScript = ballSM.ballOwnerPlayer.GetComponent<PlayerScript>();
        float chargeValue = Mathf.Clamp(ballOwnerPlayerScript.chargeValueIncrementor, ballOwnerPlayerScript.chargeClamp, 1f);
        
        
        // Recover the particle's current size.
        float currentSize = mainModule.startSizeMultiplier;
        
        mainModule.startSize = ballSize * currentSize * chargeValue;
        // From the HitState, get the direction of the hit.
        Vector3 hitDirection = ballSM.currentState.GetComponent<HitState>().hitDirection;
        
        // Set the rotation of the perfect hit particle system to the hit direction.
        perfectHitParticle.transform.rotation = Quaternion.LookRotation(hitDirection);
        
        
        perfectHitParticle.Play();
        
        mainModule.startSizeMultiplier = currentSize;
    }
    
    public void OnLethalBall()
    {
        // play the lethal ball particle system.
        lethalBallParticle.Play();
    }
    
    private void OnCollisionEnter(Collision other)
    {
        foreach (ContactPoint contact in other.contacts)
        {
            Vector3 collisionPoint = contact.point;
            
            // Instantiate the impact particle system at the collision point.
            ParticleSystem impactParticleInstance = Instantiate(impactParticle, collisionPoint, Quaternion.identity);
            
            // Set the position of the impact particle system to the collision point.
            impactParticleInstance.transform.position = collisionPoint;
            
            // Set the rotation of the impact particle system to the collision normal.
            impactParticleInstance.transform.rotation = Quaternion.LookRotation(contact.normal);
            
            // Set the size of the impact particle system based on the ball's size and speed.
            float ballSize = ballSM.GetComponent<Transform>().localScale.x;
            float speed = ballSM.GetComponent<Rigidbody>().linearVelocity.magnitude;
            float impactZoneSize = speed * particleImpactZoneSizeModifier;
            
            ParticleSystem.MainModule mainModule = impactParticleInstance.main;
            
            mainModule.startSize = ballSize * particleSizeModifier;
            
            // Set the scale of the particle game object depending on the size and speed of the ball.
            impactParticleInstance.transform.localScale = new Vector3(impactZoneSize, impactZoneSize, impactZoneSize);
            
            // Play the impact particle system.
            impactParticleInstance.Play();
            
            // Destroy the impact particle system after 1 second.
            Destroy(impactParticleInstance.gameObject, 1f);
        }
    } 
}
