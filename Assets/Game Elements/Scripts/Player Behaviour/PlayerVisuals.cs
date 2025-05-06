using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerVisuals : MonoBehaviour
{
    //-------------PRIVATE VARIABLES-------------
    // Player script.
    private PlayerScript playerScript;
    
    // Image component of the parry timer visuals.
    private float _parryRadius;
    
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
    [Tooltip("The pointer showing the direction the player is aiming towards.")]
    public GameObject aimPointer;

    private float _aimPointerScale = 2.5f;
    
    
    
    void Start()
    {
        // Recover the PlayerScript from the player.
        playerScript = GetComponent<PlayerScript>();
        // Recover the player's mesh material and color.
        playerMeshMaterial = playerMesh.GetComponent<MeshRenderer>().material;
        _aimPointerScale = aimPointer.transform.localScale.x;

        _parryRadius = playerScript.hitDetectionRadius;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    { 
        switch (playerScript.currentState) 
        { 
            case NeutralState:
                // Change the player's color back to the original color.
                playerMeshMaterial.color = _originalPlayerMeshColor;
                
                // Stop the dead particle if it is playing.
                if (deadParticle.isPlaying)
                {deadParticle.Stop();}
                
                // Set the aimPointer's scale.
                aimPointer.transform.localScale = new Vector3(_aimPointerScale, _aimPointerScale, _aimPointerScale);
                
                OnSprintEnd();
                
                break;
            
            case ChargingState:
                // Function to signal the charging state of the player.
                // ChargeFeedback();
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
                aimPointer.SetActive(true);
                break;
            case PlayerScript.HitType.ReflectiveHit:
                aimPointer.SetActive(false);
                break;

        }

        // WarnChargeAlmostOver();

        // RecoverAfterDash();
        // // Dash trail width is equal to the player's rollDetectionRadius.
        // dashTrail.widthMultiplier = playerScript.rollDetectionRadius * 2f;
        
        // Dash color is equal to the player's color.
        dashTrail.startColor = playerMeshMaterial.color;
        dashTrail.endColor = playerMeshMaterial.color;
        
        // Update the parry radius collider.
        var parryParticleShape = parryParticle.shape;
        parryParticleShape.radius = _parryRadius;

    }

    // public void ChargeFeedback()
    // {
    //     // Get the charging state of the player.
    //     ChargingState chargingState = GetComponent<ChargingState>();
    //     
    //     // Calculate the percentage of time past using the chargeLimitTimer and the chargeTimeLimit.
    //     float chargePercentage = chargingState.chargeLimitTimer / playerScript.chargeTimeLimit;
    //     
    //     // The bigger chargePercentage is, the smaller the AimPointer will be.
    //     aimPointer.transform.localScale = new Vector3(_aimPointerScale * (1 - chargePercentage), _aimPointerScale * (1 - chargePercentage), _aimPointerScale * (1 - chargePercentage));
    // }
    //
    // public void WarnChargeAlmostOver()
    // {
    //     // Get the charging state of the player.
    //     ChargingState chargingState = GetComponent<ChargingState>();
    //     
    //     // Calculate the percentage of time past using the chargeLimitTimer and the chargeTimeLimit.
    //     float chargePercentage = chargingState.chargeLimitTimer / playerScript.chargeTimeLimit;
    //     
    //     if (chargePercentage >= chargeEndingParticleTime)
    //     {
    //         if (!chargeEndingParticle.isPlaying)
    //         {
    //             chargeEndingParticle.Play();
    //         }
    //     }
    //     else
    //     {
    //         if (chargeEndingParticle.isPlaying)
    //         {
    //             chargeEndingParticle.Stop();
    //         }
    //     }
    //     
    //     
    // }
    
    // public void RecoverAfterDash()
    // {
    //     if (playerMesh.transform.rotation.x != 0)
    //     {
    //         // Rotate the player mesh on the X axis to emulate them standing up over time.
    //         
    //         playerMesh.transform.rotation = Quaternion.Euler
    //         (Mathf.Lerp(playerMesh.transform.rotation.x, 0, Time.deltaTime * playerScript.dashDuration),
    //             playerMesh.transform.rotation.y, playerMesh.transform.rotation.z);
    //     }
    // }

    public void SetEightDirectionArrow()
    {
        // Using the player's eightDirection Vector3, recover a float that represents the rotation value of that direction
        // in relation to the player's forward direction.
        //float angle = x;
        // Debug.Log(angle);
        // Rotate the aimPointer to the angle.
        //aimPointer.transform.rotation = Quaternion.Euler(90, angle, 0);
        
    }
    public void OnParry(float chargeValue)
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
