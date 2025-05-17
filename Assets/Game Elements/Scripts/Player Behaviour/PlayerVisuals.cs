    using System.Globalization;
    using TMPro;
    using UnityEditor;
    using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerVisuals : MonoBehaviour
{
    //-------------PRIVATE VARIABLES-------------
    // Player script.
    private PlayerScript playerScript;
    
    // Image component of the parry timer visuals.
    private float _parryDiameter;
    
    // Player's normal mesh material and color.
    [FormerlySerializedAs("_playerMeshMaterial")] public Material playerMeshMaterial;
    private Color _originalPlayerMeshColor;
    
    //-------------PUBLIC VARIABLES-------------
    
    [Tooltip("Player's mesh.")]
    public GameObject playerMesh;
    [Tooltip("Color when knocked back.")]
    public Color knockbackColor;
    [Tooltip("This particle is played when the player parries.")]
    public ParticleSystem parryParticle;
    [Tooltip("Trail that is left behind when player dashes")]
    public TrailRenderer dashTrail;
    [Tooltip("Particle that is played when the player dies.")]
    public ParticleSystem deadParticle;
    [Tooltip("Particle that plays when the player's charge is about to end")]
    public ParticleSystem chargeEndingParticle;
    [Tooltip("The porcentage of the charge time that the particle will start playing.")]
    public float chargeEndingParticleTime;
    [FormerlySerializedAs("aimPointer")] [Tooltip("The sphere that determines the character's range.")]
    public GameObject rangeSphereObject;

    public GameObject stateText;
    public GameObject chargeText;
    
    public ParticleSystem grabParticle;
    private ParticleSystem.ShapeModule _grabParticleShape;
    private float _currentEmissionRate;
    
    
    void Start()
    {
        // Recover the PlayerScript from the player.
        playerScript = GetComponent<PlayerScript>();
        // Recover the player's mesh material and color.
        playerMeshMaterial = playerMesh.GetComponent<MeshRenderer>().material;
        _parryDiameter = playerScript.hitDetectionRadius * 2f - parryParticle.main.startSizeMultiplier / 2f;
        _grabParticleShape = grabParticle.shape;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerStateText();
        
        switch (playerScript.currentState) 
        { 
            case NeutralState:
                ResetGrabParticle();
                playerMeshMaterial.color = _originalPlayerMeshColor;
                if (deadParticle.isPlaying)
                {deadParticle.Stop();}
                rangeSphereObject.transform.localScale = new Vector3(_parryDiameter, 1f, _parryDiameter);
                OnSprintEnd();
                
                break;
            
            case GrabbingState:
                UpdateGrabParticle();
                OnSprintEnd();
                break;
            
            case DeadState:
                playerMeshMaterial.color = Color.black; 
                if (!deadParticle.isPlaying)
                {deadParticle.Play();}
                OnSprintEnd();
                break;
            
            case KnockbackState:
                playerMeshMaterial.color = knockbackColor;
                OnSprintEnd();
                break;
            
            case DashingState:
                OnSprintStart();
                break;
            
            default:
                break;
        }
        switch (playerScript.hitType)
        {
            case PlayerScript.HitType.ForwardHit:
                rangeSphereObject.SetActive(true);
                break;
            case PlayerScript.HitType.ReflectiveHit:
                rangeSphereObject.SetActive(false);
                break;

        }

        
        dashTrail.startColor = playerMeshMaterial.color;
        dashTrail.endColor = playerMeshMaterial.color;
        

        var parryParticleShape = parryParticle.shape;
        parryParticleShape.radius = _parryDiameter / 2f;
        
        GrabChargeValue(playerScript.grabCurrentCharge);

    }

    public void OnGrabStateEntered()
    {
        // Play the grab particle.
        grabParticle.Play();
    }
    public void OnGrabStateExited()
    {
        // Stop the grab particle.
        grabParticle.Stop();
    }
    
    
    private void UpdateGrabParticle()
    {
        GrabbingState grabbingState = playerScript.currentState as GrabbingState;
        if (grabbingState == null) return;
        
        // Set the shape radius to be equal to the grab radius
        _grabParticleShape.radius = playerScript.grabDetectionRadius;
        
        
        
        // Adjust the emission rate based on the current charge
        // var emission = grabParticle.emission;
        // _currentEmissionRate = emission.rateOverTime.constant;
        // float maxEmissionRate = emission.rateOverTime.constant; // Maximum emission rate
        // emission.rateOverTime = maxEmissionRate * playerScript.grabCurrentCharge;
        
        
        
        // Get the current angle of the grabbing state.
        float currentAngle = grabbingState.currentAngle;

        _grabParticleShape.arc = currentAngle;
        
        _grabParticleShape.rotation = new Vector3(-90, 90 + (180-currentAngle*0.5f), 0);
    }
    
    private void ResetGrabParticle()
    {
        
        // var emission = grabParticle.emission;
        // emission.rateOverTime = _currentEmissionRate;
        _grabParticleShape.arc = 180;
        _grabParticleShape.rotation = new Vector3(-90, 180, 0);
    }

    private void PlayerStateText()
    {
        // Get the current state of the player.
        string currentState = playerScript.currentState.ToString();
        
        // Set the text of the stateText to the current state.
        stateText.GetComponent<TextMeshPro>().text = currentState;
    }

    private void GrabChargeValue(float charge)
    {
        //Round charge down to 2 decimal places.
        
        charge = Mathf.Round(charge * 100f) / 100f;
        
        
        chargeText.GetComponent<TextMeshPro>().text = charge.ToString(CultureInfo.CurrentCulture);
    }
    
    public void OnParry()
    {
        // Play the parry particle.
        parryParticle.Play();
    }
    
    public void OnSprintStart()
    {
        dashTrail.emitting = true;
    }
    
    public void OnSprintEnd()
    {
        dashTrail.emitting = false;
    }

    public void ChangePlayerColor(Color color)
    {
        if (playerMeshMaterial)
        {
            // Debug.Log("Changing player color to " + color);
            playerMeshMaterial.color = color;
            _originalPlayerMeshColor = color;
        }
        else
        {
            playerMeshMaterial = playerMesh.GetComponentInChildren<MeshRenderer>().material;
            playerMeshMaterial.color = color;
            _originalPlayerMeshColor = color;
            // Debug.Log("Changing player color to " + color);
        }
        
        
    }
}
