using UnityEngine;
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
    [Tooltip("Game Object that holds the parry timer visuals.")]
    public GameObject parryTimerVisuals;
    [Tooltip("Parry timer visual Offset X")]
    public float parryTimerVisualOffsetX;
    [Tooltip("Parry timer visual Offset Y")]
    public float parryTimerVisualOffsetY;
    
    void Start()
    {
        // Recover the PlayerScript from the player.
        playerScript = GetComponent<PlayerScript>();
        // Recover the Image from the charge visuals.
        chargeSprite = chargeVisuals.GetComponentInChildren<Image>();
        // Recover the player's mesh material and color.
        _playerMeshMaterial = playerMesh.GetComponent<MeshRenderer>().material;
        _originalPlayerMeshColor = _playerMeshMaterial.color;
        _parryTimerSprite = parryTimerVisuals.GetComponentInChildren<Image>();
        
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
                case MomentumState:
                    _playerMeshMaterial.color = knockbackColor;
                    break;
                default:
                    _playerMeshMaterial.color = _originalPlayerMeshColor;
                    break;
            }
        }

        _parryTimerSprite.fillAmount = playerScript.parryTimer / playerScript.parryCooldown;
        
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
    }
    
    private void ParryBar(Vector3 parryTimerVisualScreenPosition)
    {
        // Convert the player's hand position to screen space
        // Vector3 parryTimerVisualScreenPosition = playerScript.playerCamera.WorldToScreenPoint(playerScript.playerHand.transform.position);
        
        // Apply the offset
        parryTimerVisualScreenPosition.x += parryTimerVisualOffsetX;
        parryTimerVisualScreenPosition.y += parryTimerVisualOffsetY;

        // Update the position of the parry timer visuals in the canvas
        parryTimerVisuals.transform.position = parryTimerVisualScreenPosition;
    }
    
    public void OnParryAvailable()
    {
        if (playerScript.currentState != playerScript.GetComponent<MomentumState>())
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
}
