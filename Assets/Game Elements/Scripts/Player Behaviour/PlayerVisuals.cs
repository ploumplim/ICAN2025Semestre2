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
    private Material _playerMeshMaterial;
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
    [Tooltip("The pointer showing the direction the player is aiming towards.")]
    public GameObject aimPointer;
    [Tooltip("The game object holding the bar that is shown when a hit is being charged.")]
    public GameObject chargeBar;
    private SpriteRenderer _chargeBarSprite;
    
    
    
    void Start()
    {
        // Recover the PlayerScript from the player.
        playerScript = GetComponent<PlayerScript>();
        // Recover the player's mesh material and color.
        _playerMeshMaterial = playerMesh.GetComponent<MeshRenderer>().material;
        

        _parryRadius = playerScript.hitDetectionRadius;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {        
        
        switch (playerScript.currentState) 
        { 
            case NeutralState:
                _playerMeshMaterial.color = _originalPlayerMeshColor;
                if (deadParticle.isPlaying)
                {deadParticle.Stop();}
                break;
            case DeadState:
                _playerMeshMaterial.color = Color.black; 
                if (!deadParticle.isPlaying)
                {deadParticle.Play();}
                break;
            case KnockbackState:
                _playerMeshMaterial.color = knockbackColor;
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
            case PlayerScript.HitType.EightDirHit:
                aimPointer.SetActive(true);
                if (playerScript.moveInputVector2 != Vector2.zero)
                {
                    SetEightDirectionArrow();
                }
                break;
        }
        





        RecoverAfterDash();
        // Dash trail width is equal to the player's rollDetectionRadius.
        dashTrail.widthMultiplier = playerScript.rollDetectionRadius * 2f;
        
        // Dash color is equal to the player's color.
        dashTrail.startColor = _playerMeshMaterial.color;
        dashTrail.endColor = _playerMeshMaterial.color;
        
        // Update the parry radius collider.
        var parryParticleShape = parryParticle.shape;
        parryParticleShape.radius = _parryRadius;

    }
    
    
    public void RecoverAfterDash()
    {
        if (playerMesh.transform.rotation.x != 0)
        {
            // Rotate the player mesh on the X axis to emulate them standing up over time.
            
            playerMesh.transform.rotation = Quaternion.Euler
            (Mathf.Lerp(playerMesh.transform.rotation.x, 0, Time.deltaTime * playerScript.dashDuration),
                playerMesh.transform.rotation.y, playerMesh.transform.rotation.z);
        }
    }

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
    
    public void OnDashEnter()
    {
        dashTrail.emitting = true;
        // Rotate the player mesh to be completely horizontal
        playerMesh.transform.rotation = Quaternion.Euler(90, playerMesh.transform.rotation.y, playerMesh.transform.rotation.z);
    }
    
    public void OnDashExit()
    {
        dashTrail.emitting = false;
    }

    public void ChangePlayerColor(Color color)
    {
        if (_playerMeshMaterial)
        {
            // Debug.Log("Changing player color to " + color);
            _playerMeshMaterial.color = color;
            _originalPlayerMeshColor = color;
        }
        else
        {
            _playerMeshMaterial = playerMesh.GetComponentInChildren<MeshRenderer>().material;
            _playerMeshMaterial.color = color;
            _originalPlayerMeshColor = color;
            // Debug.Log("Changing player color to " + color);
        }
        
        
    }
}
