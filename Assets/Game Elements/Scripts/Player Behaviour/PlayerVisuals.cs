using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerVisuals : MonoBehaviour
{
    //-------------PRIVATE VARIABLES-------------
    // Player script.
    private PlayerScript playerScript;
    // Charge porcentage from 0 to 1.
   [SerializeField]private float chargePorcentage;

    // Image component of the charging visuals.
    private Image chargeSprite;
    
    // Image component of the parry timer visuals.
    private Image _parryTimerSprite;
    private bool _canParry;
    private float _parryRadius;
    
    // Player's normal mesh material and color.
    private Material _playerMeshMaterial;
    private Color _originalPlayerMeshColor;
    
    //-------------PUBLIC VARIABLES-------------
    
    [Tooltip("Player's mesh.")]
    public GameObject playerMesh;
    [Tooltip("Color when knocked back.")]
    public Color knockbackColor;
    [Tooltip("Color when parry is available.")]
    public Color canParryColor;
    [Tooltip("Game Object that holds the charge visuals.")]
    public GameObject chargeVisuals;
    [Tooltip("charge visual Offset X")]
    public float chargeVisualOffsetX;
    [Tooltip("charge visual Offset Y")]
    public float chargeVisualOffsetY;
    [FormerlySerializedAs("parryTimerVisuals")] [Tooltip("Game Object that holds the parry timer visuals.")]
    public GameObject hitTimerVisuals;
    [Tooltip("Parry timer visual Offset X")]
    public float parryTimerVisualOffsetX;
    [Tooltip("Parry timer visual Offset Y")]
    public float parryTimerVisualOffsetY;
    [Tooltip("This particle is played when the player parries.")]
    public ParticleSystem parryParticle;
    [Tooltip("Trail that is left behind when player dashes")]
    public TrailRenderer dashTrail;
    
    void Start()
    {
        // Recover the PlayerScript from the player.
        playerScript = GetComponent<PlayerScript>();
        // Recover the Image from the charge visuals.
        chargeSprite = chargeVisuals.GetComponentInChildren<Image>();
        // Recover the player's mesh material and color.
        _playerMeshMaterial = playerMesh.GetComponent<MeshRenderer>().material;
        _originalPlayerMeshColor = _playerMeshMaterial.color;
        _parryTimerSprite = hitTimerVisuals.GetComponentInChildren<Image>();
        

        _parryRadius = playerScript.hitDetectionRadius;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {        
        Vector3 handScreenPosition = playerScript.playerCamera.WorldToScreenPoint(playerScript.playerHand.transform.position);
        Vector3 playerScreenPosition = playerScript.playerCamera.WorldToScreenPoint(playerScript.transform.position);
        ChargeBar(handScreenPosition);
        ParryBar(playerScreenPosition);
        
        if (!_canParry)
        {
            switch (playerScript.currentState)
            {
                case KnockbackState:
                    _playerMeshMaterial.color = knockbackColor;
                    break;
                default:
                    _playerMeshMaterial.color = _originalPlayerMeshColor;
                    break;
            }
        }

        _parryTimerSprite.fillAmount = playerScript.hitTimer / playerScript.releaseDuration;
        RecoverAfterDash();
        // Dash trail width is equal to the player's rollDetectionRadius.
        dashTrail.widthMultiplier = playerScript.rollDetectionRadius;
        
        // Update the parry radius collider.
        var parryParticleShape = parryParticle.shape;
        parryParticleShape.radius = _parryRadius;

    }

    private void ChargeBar(Vector3 chargeVisualScreenPosition)
    {
        chargePorcentage = playerScript.chargeValueIncrementor;
        
        // Convert the player's hand position to screen space

        // Apply the offset
        chargeVisualScreenPosition.x += chargeVisualOffsetX;
        chargeVisualScreenPosition.y += chargeVisualOffsetY;

        // Update the position of the charge visuals in the canvas
        chargeVisuals.transform.position = chargeVisualScreenPosition;
        
        // Update the Image fill amount with the charge percentage.
        chargeSprite.fillAmount = chargePorcentage;
        
        // Change the rotation of the player mesh to emulate them standing up.
    }
    public void RecoverAfterDash()
    {
        if (playerMesh.transform.rotation.x != 0)
        {
            // Rotate the player mesh on the X axis to emulate them standing up over time.
            
            playerMesh.transform.rotation = Quaternion.Euler
            (Mathf.Lerp(playerMesh.transform.rotation.x, 0, Time.deltaTime * playerScript.rollDuration),
                playerMesh.transform.rotation.y, playerMesh.transform.rotation.z);
        }
    }    
    private void ParryBar(Vector3 parryTimerVisualScreenPosition)
    {
        // Convert the player's hand position to screen space
        // Vector3 parryTimerVisualScreenPosition = playerScript.playerCamera.WorldToScreenPoint(playerScript.playerHand.transform.position);
        
        // Apply the offset
        parryTimerVisualScreenPosition.x += parryTimerVisualOffsetX;
        parryTimerVisualScreenPosition.y += parryTimerVisualOffsetY;

        // Update the position of the parry timer visuals in the canvas
        hitTimerVisuals.transform.position = parryTimerVisualScreenPosition;
    }
    
    public void OnParryAvailable()
    {
        if (playerScript.currentState != playerScript.GetComponent<KnockbackState>())
        {
            _playerMeshMaterial.color = canParryColor;
            _canParry = true;
        }
    }
    public void OnParryUnavailable()
    {
        _playerMeshMaterial.color = _originalPlayerMeshColor;
        _canParry = false;
    }
    
    public void OnParry()
    {
        // Play the parry particle.
        parryParticle.Play();
        // Change the player's color to the original color.
        _playerMeshMaterial.color = _originalPlayerMeshColor;
        _canParry = false;
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
        }
        else
        {
            _playerMeshMaterial = playerMesh.GetComponentInChildren<MeshRenderer>().material;
            _playerMeshMaterial.color = color;
            // Debug.Log("Changing player color to " + color);
        }
        
        
    }
}
