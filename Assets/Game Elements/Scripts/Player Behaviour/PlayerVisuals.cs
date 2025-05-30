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
    
    [Tooltip("This particle is played when the player parries.")]
    public ParticleSystem parryParticle;
    [Tooltip("Trail that is left behind when player dashes")]
    public TrailRenderer dashTrail;
    [Tooltip("Particle that is played when the player dies.")]
    public ParticleSystem deadParticle;
    [FormerlySerializedAs("aimPointer")] [Tooltip("The sphere that determines the character's range.")]
    public GameObject rangeSphereObject;

    public GameObject stateText;
    public GameObject chargeText;
    public GameObject sprintText;
    
    public ParticleSystem grabParticle;
    private ParticleSystem.ShapeModule _grabParticleShape;
    private float _grabParticleSize;

    public ParticleSystem runningParticles;
    public float baseRunningParticleRefreshRate = 1f; // How often the running particles play, divided by current speed.
    public float runningParticleRefreshMultiplier = 10f; // How much the running particle refresh rate is multiplied by the player's speed.
    private float _runningParticleTimer;
    
    public ParticleSystem sprintBoostParticle;
    [FormerlySerializedAs("sprintBoostColor")] public Color GoodSprintBoostColor = Color.yellow;
    public Color badSprintBoostCoolor = Color.white;
    public AnimationCurve sprintBoostCurve = AnimationCurve.Linear(0, 0, 1, 1);
    private float _currentSprintBoostParticleSize;

    public ParticleSystem sprintSweatParticle;
    [Range(0, 100)] public int sweatPercent = 50; // Percentage of sprint boost at which the sweat particle will be activated.
    
    public ParticleSystem knockbackParticle;

    public ParticleSystem fullChargeSprintBoostParticle;
    
    public ParticleSystem playerReadyParticle;
    void Start()
    {
        // Recover the PlayerScript from the player.
        playerScript = GetComponent<PlayerScript>();
        // Recover the player's mesh material and color.
        _parryDiameter = playerScript.hitDetectionRadius * 2f - parryParticle.main.startSizeMultiplier / 2f;
        _grabParticleShape = grabParticle.shape;
        _grabParticleSize = grabParticle.main.startSize.constant;
        _currentSprintBoostParticleSize = sprintBoostParticle.main.startSize.constant;

        if (perso)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = perso.GetComponentInChildren<SkinnedMeshRenderer>();
            Material[] materials = skinnedMeshRenderer.materials;
            
            playerCapMaterial = materials[1];
        }
    }

    void Update()
    {
        PlayerStateText();
        PlayerSprintText();
        RunningParticleUpdater();
        SweatParticleUpdater();
        // SprintBoostUpdater();

        if (playerScript.isReady && GameManager.Instance.levelManager.currentState == GameManager.Instance.levelManager.GetComponent<OutOfLevelState>())
        {
            // If the player is ready, play the ready particle.
            if (!playerReadyParticle.isPlaying)
            {
                playerReadyParticle.Play();
            }
        }
        else
        {
            // If the player is not ready, stop the ready particle.
            if (playerReadyParticle.isPlaying)
            {
                playerReadyParticle.Stop();
            }
        }
        
        {
            
        }
        
        
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
                // playerCapMaterial.color = knockbackColor;
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
        
        // Set the alpha value to 0.5f for the trail color
        Color endColor = dashTrail.endColor;
        endColor.a = 0f;
        dashTrail.endColor = endColor;
        
        Color startColor = dashTrail.startColor;
        startColor.a = 0.5f;
        dashTrail.startColor = startColor;
        
        
        
        var parryParticleShape = parryParticle.shape;
        parryParticleShape.radius = _parryDiameter / 2f;
        
        GrabChargeValue(playerScript.grabCurrentCharge);

    }
    
    private void SweatParticleUpdater()
    {
        // Activate the sweat particle if the player's boost is less than half, deactivate when its more.
        SprintState sprintState = GetComponent<SprintState>();

        if (!sprintState)
        {
            return;
        }
        
        if ((sprintState.currentSprintBoost >= playerScript.sprintMaxInitialBoost * (sweatPercent / 100f) ||
             playerScript.currentState is SprintState))
        {
            if (sprintSweatParticle.isPlaying)
            {
                sprintSweatParticle.Stop();
            }
            return;
        }

        if (sprintState.currentSprintBoost < playerScript.sprintMaxInitialBoost * (sweatPercent / 100f))
        {
            if (!sprintSweatParticle.isPlaying)
            {
                sprintSweatParticle.Play();
            }
        }
        
        
    }

    private void SprintBoostUpdater()
    {
        // If the player is sprinting, play the sprint boost particles.
        SprintState sprintState = GetComponent<SprintState>();
        if (sprintState != null)
        {
            float currentSprintBoost = Mathf.Clamp(sprintState.currentSprintBoost, 1, sprintState.currentSprintBoost);
            float r = currentSprintBoost / playerScript.sprintMaxInitialBoost;
            // Using an animation curve, create a color that goes from the original color to the sprint boost color depending on the current sprint boost.
            var main = sprintBoostParticle.main;
            main.startColor = Color.Lerp(badSprintBoostCoolor, GoodSprintBoostColor, sprintBoostCurve.Evaluate(r));
        }
    }

    public void StartKnockbackParticle()
    {
        // Start the knockback particle.
        if (!knockbackParticle.isPlaying)
        {
            knockbackParticle.Play();
        }
    }
    public void StopKnockbackParticle()
    {
        // Stop the knockback particle.
        if (knockbackParticle.isPlaying)
        {
            knockbackParticle.Stop();
        }
    }
    
    public void StartSprintBoostParticle()
    {
        // Start the sprint boost particle.
        if (!sprintBoostParticle.isPlaying)
        {
            // Set the particle size to multiply the current sprint boost by the original size.
            SprintBoostUpdater();
            var main = sprintBoostParticle.main;
            
            main.startSize = Mathf.Clamp(_currentSprintBoostParticleSize * GetComponent<SprintState>().currentSprintBoost, 1f, 5f);
            sprintBoostParticle.Play();
        }
    }
    
    public void StopSprintBoostParticle()
    {
        // Stop the sprint boost particle.
        if (sprintBoostParticle.isPlaying)
        {
            sprintBoostParticle.Stop();
        }
    }
    
    public void FullyChargedSprintBoostParticle()
    {
        // Start the full charge sprint boost particle.
        if (!fullChargeSprintBoostParticle.isPlaying)
        {
            fullChargeSprintBoostParticle.Play();
        }
    }
    
    private void RunningParticleUpdater()
    {
        float playerSpeed = playerScript.rb.linearVelocity.magnitude;
        float newRefreshRate = baseRunningParticleRefreshRate;
        
        _runningParticleTimer += Time.deltaTime;
        
        if (playerSpeed <= 0)
        {
            return;
        }
        
        if (playerSpeed > 0)
        {
            // Calculate the new refresh rate based on the player's speed.
            newRefreshRate = baseRunningParticleRefreshRate * runningParticleRefreshMultiplier / playerSpeed;
            newRefreshRate = Mathf.Max(newRefreshRate, 0.15f);
        }
        
        // Everytime the _runningParticleTimer reaches the runningParticleRefreshRate, play the running particles.
        if (_runningParticleTimer >= newRefreshRate)
        {
            // Reset the timer.
            _runningParticleTimer = 0f;
            // Play the running particles.
            runningParticles.Play();
            // Debug.Log("Playing running particles with refresh rate: " + newRefreshRate);
            
        }
        
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
        
        // Recover the particle size.
        var main = grabParticle.main;
        
        // modify the particle size based on the current grab charge.
        main.startSize = Mathf.Clamp(playerScript.grabCurrentCharge * _grabParticleSize, 0.1f, _grabParticleSize);
        
        // Debug.Log ("Grab charge: " + playerScript.grabCurrentCharge + " Particle size: " + main.startSize.constant);
        
        // Get the current angle of the grabbing state.
        float currentAngle = grabbingState.currentAngle;

        _grabParticleShape.arc = currentAngle;
        
        _grabParticleShape.rotation = new Vector3(-90, 90 + (180 - currentAngle * 0.5f), 0);
    }
    
    private void ResetGrabParticle()
    {
        // Reset the particle size to 3
        var main = grabParticle.main;
        main.startSize = _grabParticleSize;

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
