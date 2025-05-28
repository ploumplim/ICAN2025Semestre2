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
    public GameObject neutralBallObject;
    public GameObject neutralBallOutlineObject;
    // public GameObject lethalBall;
    
    [Header("Ball Impact settings")]
    public ParticleSystem impactParticle;
    public float particleSizeModifier = 0.1f;
    [FormerlySerializedAs("particleImpactZoneSize")] public float particleImpactZoneSizeModifier = 0.1f;
    
    [Header("Ball catch settings")]
    public ParticleSystem catchParticle;
    
    [Header("Perfect hit settings")]
    public ParticleSystem perfectHitParticle;
    public float perfectHitThreshold = 0.95f;
    public ParticleSystem hitChargeParticle;
    
    [Header("Lethal ball settings")]
    public ParticleSystem lethalBallParticle;
    
    [Header("SpeedFeedback settings")]
    public ParticleSystem speedFeedbackParticle;

    public float speedFeedBackMultiplier = 0.1f;
    public float speedFeedbackMaximumSpeed = 100f;
    public float speedFeedbackMinimumSpeed = 20f;

    [Header("Stretch and squash settings")]
    public float stretchModifier = 0.5f;
    
    // ---------------PRIVATE---------------
    private BallSM ballSM;
    private Camera _mainCamera;
    private TrailRenderer _trailRenderer;
    private Material _neutralBallMaterial;
    private Material _neutralBallOutlineMaterial;

    public void OnEnable()
    {
        ballSM = GetComponent<BallSM>();
        _mainCamera = Camera.main;
        _trailRenderer = trailVisuals.GetComponent<TrailRenderer>();
        _neutralBallMaterial = neutralBallObject.GetComponentInChildren<MeshRenderer>().material;
        _neutralBallOutlineMaterial = neutralBallOutlineObject.GetComponent<MeshRenderer>().material;
        
        // Set all faces to inactive at the start.
        neutralFace.gameObject.SetActive(false);
        lethalFace.gameObject.SetActive(false);
        hitFace.gameObject.SetActive(false);
        
    }
    
    
    private void FixedUpdate()
    {
        TrailEmitter();
        BallColorAndLight();
        // UpdateFace();
        // UpdateBallSpeedFeedbacks();
        UpdateForward();
        StretchAndSquash();
    }

    private void UpdateForward()
    {
        // Make the ball always align with its linear velocity vector 3 direction.
        Vector3 ballVelocity = ballSM.GetComponent<Rigidbody>().linearVelocity;
        if (ballVelocity != Vector3.zero)
        {
            // Set the rotation of the ball to look in the direction of its velocity.
            Quaternion targetRotation = Quaternion.LookRotation(ballVelocity);
            ballSM.transform.rotation = targetRotation;
        }
    }

    public void UpdateFlyingColor(Color ownerColor)
    {
        // Update the color of the ball when it's flying based on the owner player's color.
        flyingBallColor = ownerColor;
        startFlyingTrailColor = ownerColor;
        endFlyingTrailColor = new Color(ownerColor.r, ownerColor.g, ownerColor.b, 0f);
    }

    private void StretchAndSquash()
    {
        // Apply on the ball's neutral sphere a stretch and squash effect based on the ball's current speed. Do this by 
        // changing the local Z scale of the ball's neutral sphere.
        
        float speed = ballSM.GetComponent<Rigidbody>().linearVelocity.magnitude;
        float currentBallSize = neutralBall.GetComponent<Transform>().localScale.x;
        // Calculate the new Z scale based on the speed and ball size.
        float newZScale = Mathf.Clamp( speed * currentBallSize * stretchModifier, 1f, 1.5f);
        // Set the new local scale of the ball's neutral sphere.
        Vector3 newScale = new Vector3(currentBallSize, currentBallSize, newZScale);
        neutralBall.transform.localScale = newScale;
    }

    private void UpdateBallSpeedFeedbacks()
    {
        // Multiply the current velocity multiplier from the speedFeedback Particle by the modifier and the current speed of the ball.
        float ballSpeed = ballSM.GetComponent<Rigidbody>().linearVelocity.magnitude;
        float speedFeedback = Mathf.Clamp( ballSpeed * speedFeedBackMultiplier, speedFeedbackMinimumSpeed, speedFeedbackMaximumSpeed);
        
        var velocityOverLifetime = speedFeedbackParticle.velocityOverLifetime;
        ParticleSystem.MinMaxCurve speedModifier = velocityOverLifetime.speedModifier;
        speedModifier.constant = speedFeedback;
        
        // Set the orientation of the particle system equal to the vector velocity of the ball.
        Vector3 ballVelocity = ballSM.GetComponent<Rigidbody>().linearVelocity;
        if (ballVelocity != Vector3.zero)
        {
            speedFeedbackParticle.transform.rotation = Quaternion.LookRotation(ballVelocity);
        }
    }
    
    
    private void TrailEmitter()
    {
        // Enable or disable the trail based on the ball's state. Enabled when it's midair, disabled otherwise.
        _trailRenderer.emitting = ballSM.currentState.GetType() == typeof(FlyingState) || ballSM.currentState.GetType() == typeof(LethalBallState);
        
        // Get the current speed magnitude from the ball
        float speed = ballSM.GetComponent<Rigidbody>().linearVelocity.magnitude;
        

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
    
    private void BallColorAndLight()
    {
        // Change the color of the ball based on the ball's state. Red when it's midair, green otherwise.

            switch (ballSM.currentState)
            {
                case CaughtState:
                    _neutralBallOutlineMaterial.color = caughtBallColor;
                    _neutralBallMaterial.SetColor("_EmissionColor", caughtBallColor);
                    ballLight.color = caughtBallColor;
                    break;
                case HitState:
                    _neutralBallOutlineMaterial.color = HitBallColor;
                    _neutralBallMaterial.SetColor("_EmissionColor", HitBallColor);
                    ballLight.color = HitBallColor;
                    break;
                case FlyingState:
                    _neutralBallMaterial.color = flyingBallColor;
                    _neutralBallOutlineMaterial.color = Color.black;
                    _neutralBallMaterial.SetColor("_EmissionColor", flyingBallColor);
                    ballLight.color = flyingBallColor;
                    break;
                case DroppedState:
                    _neutralBallOutlineMaterial.color = groundedBallColor;
                    _neutralBallMaterial.SetColor("_EmissionColor", groundedBallColor);
                    ballLight.color = groundedBallColor;
                    break;
            }
            
    }

    public void OnBallCaught()
    {
        
        // Set the catchParticle's shape radius to be the same as the ball's current scale.
        float ballSize = ballSM.GetComponent<Transform>().localScale.x;
        ParticleSystem.ShapeModule shapeModule = catchParticle.shape;
        shapeModule.radius = ballSize;
        
        // play the catch particle system.
        catchParticle.Play();
        
    }

    public void StopCatchAnimation()
    {
        // stop the catch particle system.
        catchParticle.Stop();
    }

    public void OnHitStateStart()
    {
        // play the hit charge particle system.
        hitChargeParticle.Play();
        
    }

    public void OnHitStateEnd()
    {
        // stop the hit charge particle system.
        hitChargeParticle.Stop();
    }

    public void OnPerfectHit()
    {
        // recover ball size
        float ballSize = ballSM.GetComponent<Transform>().localScale.x;
        // Set the size of the perfect hit particle system based on the ball's size.
        ParticleSystem.MainModule mainModule = perfectHitParticle.main;
        
        // From the charge state, recover the ball owning player's charge value.
        PlayerScript ballOwnerPlayerScript = ballSM.ballOwnerPlayer.GetComponent<PlayerScript>();
        
        
        // Recover the particle's current size.
        float currentSize = mainModule.startSizeMultiplier;
        
        mainModule.startSize = ballSize * currentSize;
        // From the HitState, get the direction of the hit.
        Vector3 hitDirection = ballSM.currentState.GetComponent<HitState>().hitDirection;
        
        // Set the rotation of the perfect hit particle system to the hit direction.
        if (hitDirection != Vector3.zero)
        {
            perfectHitParticle.transform.rotation = Quaternion.LookRotation(hitDirection);
        }
        
        
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
