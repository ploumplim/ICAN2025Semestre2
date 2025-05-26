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
    [FormerlySerializedAs("playerMeshMaterial")] [FormerlySerializedAs("_playerMeshMaterial")] public Material playerCapMaterial;
    private Color _originalPlayerMeshColor;
    
    //-------------PUBLIC VARIABLES-------------
    
    [Tooltip("Player's mesh.")]
    public GameObject playerMesh;

    public GameObject perso;
    
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
    public GameObject sprintText;
    
    public ParticleSystem grabParticle;
    private ParticleSystem.ShapeModule _grabParticleShape;
    private float _currentEmissionRate;
    
    
    void Start()
    {
        // Recover the PlayerScript from the player.
        playerScript = GetComponent<PlayerScript>();
        // Recover the player's mesh material and color.
        _parryDiameter = playerScript.hitDetectionRadius * 2f - parryParticle.main.startSizeMultiplier / 2f;
        _grabParticleShape = grabParticle.shape;


        if (perso)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = perso.GetComponentInChildren<SkinnedMeshRenderer>();
            Material[] materials = skinnedMeshRenderer.materials;
            
            playerCapMaterial = materials[1];
        }
    }

    // Update is called once per frame
    void Update()
    {
        PlayerStateText();
        PlayerSprintText();
        
        switch (playerScript.currentState) 
        { 
            case NeutralState:
                ResetGrabParticle();
                playerCapMaterial.color = _originalPlayerMeshColor;
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
                playerCapMaterial.color = Color.black; 
                if (!deadParticle.isPlaying)
                {deadParticle.Play();}
                OnSprintEnd();
                break;
            
            case KnockbackState:
                playerCapMaterial.color = knockbackColor;
                OnSprintEnd();
                break;
            
            case SprintState:
                OnSprintStart();
                break;
            
            default:
                break;
        }
        switch (playerScript.hitType)
        {
            case PlayerScript.HitType.ForwardHit:
                // rangeSphereObject.SetActive(true);
                break;
            case PlayerScript.HitType.ReflectiveHit:
                rangeSphereObject.SetActive(false);
                break;

        }

        
        dashTrail.startColor = playerCapMaterial.color;
        dashTrail.endColor = playerCapMaterial.color;
        

        var parryParticleShape = parryParticle.shape;
        parryParticleShape.radius = _parryDiameter / 2f;
        
        GrabChargeValue(playerScript.grabCurrentCharge);

    }

    public void PlayerSprintText()
    {
        // Using the currentSprintBoost of the sprint state, change the text of the SprintText.
        SprintState sprintState = GetComponent<SprintState>();
        float currentSprintBoost = sprintState.currentSprintBoost;

        if (sprintState != null)
        {
            // Change the text to show the current sprint boost rounded to 2 decimal points.
            sprintText.GetComponent<TextMeshPro>().text = (Mathf.Round(currentSprintBoost * 100f) / 100f).ToString(CultureInfo.CurrentCulture);
        }
        else
        {
            sprintText.GetComponent<TextMeshPro>().text = (Mathf.Round(currentSprintBoost * 100f) / 100f).ToString(CultureInfo.CurrentCulture);
        }
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
        
        // //Recover the current emission rate
        // _currentEmissionRate = grabParticle.emission.rateOverTime.constant;
        //
        // // Adjust the emission rate based on the current charge
        // float chargePercentage = playerScript.grabCurrentCharge / playerScript.grabTotalCharge;
        //
        // // Set the emission rate to be proportional to the charge percentage
        // float newEmissionRate = _currentEmissionRate * chargePercentage;
        // var emission = grabParticle.emission;
        // emission.rateOverTime = newEmissionRate;
        
        
        // Get the current angle of the grabbing state.
        float currentAngle = grabbingState.currentAngle;

        _grabParticleShape.arc = currentAngle;
        
        _grabParticleShape.rotation = new Vector3(-90, 90 + (180 - currentAngle * 0.5f), 0);
    }
    
    private void ResetGrabParticle()
    {
        
        // Reset the emission rate to the original value.
        // var emission = grabParticle.emission;
        // emission.rateOverTime = _currentEmissionRate;
        _grabParticleShape.arc = playerScript.maxGrabAngle;
        _grabParticleShape.rotation = new Vector3(-90, playerScript.maxGrabAngle, 0);
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
        if (playerCapMaterial)
        {
            // Debug.Log("Changing player color to " + color);
            playerCapMaterial.color = color;
            _originalPlayerMeshColor = color;
        }
        else
        {
            playerCapMaterial = playerMesh.GetComponentInChildren<MeshRenderer>().material;
            playerCapMaterial.color = color;
            _originalPlayerMeshColor = color;
            // Debug.Log("Changing player color to " + color);
        }
        
        
    }
}
