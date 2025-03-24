using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HandleGamePads : MonoBehaviour
{
    public event Action<Gamepad> OnSouthButtonPressed;
    
    [SerializeField] public HashSet<Gamepad> PendingGamepads = new HashSet<Gamepad>();
    [SerializeField] public HashSet<Gamepad> AssignedGamepads = new HashSet<Gamepad>();

    public List<PlayerScript> playerReady;
    public GameObject canvas;
    

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        // Check for new gamepads
        foreach (var gamepad in Gamepad.all)
        {
            if (!AssignedGamepads.Contains(gamepad) && !PendingGamepads.Contains(gamepad))
            {
                PendingGamepads.Add(gamepad);
            }
        }

        CheckGamepadAssignments();
    }

    public void CheckGamepadAssignments()
    {
        foreach (Gamepad gamepad in PendingGamepads.ToList())
        {
            if (gamepad.buttonSouth.wasReleasedThisFrame)
            {
                OnSouthButtonPressed?.Invoke(gamepad);
                AssignedGamepads.Add(gamepad);
                PendingGamepads.Remove(gamepad);
                return;
            }
            
        }
    }

    public void VerifyIfAllPlayerAreReady()
    {
        if (AssignedGamepads.Count == playerReady.Count)
        {
            Debug.Log("All players are ready");
            canvas.GetComponent<CanvasScript>().AllPlayerReady();
        }
        else
        {
            canvas.GetComponent<CanvasScript>().AllPlayerNotReady();
        }
    }

    public static void AssignControllerToPlayer(Gamepad gamepad, GameObject player)
    {
        if (!player)
        {
            Debug.LogWarning("No player to connect to the controller.");
            return;
        }

        player.SetActive(true);

        PlayerInput playerInput = player.GetComponent<PlayerInput>();
        
        if (playerInput)
        {
            playerInput.SwitchCurrentControlScheme(gamepad);
        }
        else
        {
            Debug.LogError("PlayerInput component not found on the player.");
        }

        Debug.Log($"Gamepad '{gamepad.displayName}' assigned to player '{player.name}'");
        
        
    }
}