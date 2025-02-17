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
    
    //-------------PUBLIC VARIABLES-------------
    
    [Tooltip("Game Object that holds the charge visuals.")]
    public GameObject chargeVisuals;
    [Tooltip("charge visual Offset X")]
    public float chargeVisualOffsetX;
    [Tooltip("charge visual Offset Y")]
    public float chargeVisualOffsetY;
    
    void Start()
    {
        // Recover the PlayerScript from the player.
        playerScript = GetComponent<PlayerScript>();
        // Recover the Image from the charge visuals.
        chargeSprite = chargeVisuals.GetComponentInChildren<Image>();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        chargePorcentage = playerScript.chargeValueIncrementor;
        
        // Convert the player's hand position to screen space
        Vector3 screenPosition = playerScript.playerCamera.WorldToScreenPoint(playerScript.playerHand.transform.position);

        // Apply the offset
        screenPosition.x += chargeVisualOffsetX;
        screenPosition.y += chargeVisualOffsetY;

        // Update the position of the charge visuals in the canvas
        chargeVisuals.transform.position = screenPosition;

        
        // Update the Image fill amount with the charge porcentage.
        chargeSprite.fillAmount = chargePorcentage;
        
    }
}
