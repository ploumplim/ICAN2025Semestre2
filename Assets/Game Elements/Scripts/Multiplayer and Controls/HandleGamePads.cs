using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandleGamePads : MonoBehaviour
{
    public event Action<Gamepad> OnSouthButtonPressed;
    
    public readonly HashSet<Gamepad> PendingGamepads = new HashSet<Gamepad>();
    public readonly HashSet<Gamepad> AssignedGamepads = new HashSet<Gamepad>();
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        // Check for new gamepads
        foreach (var gamepad in Gamepad.all)
        {
            // create a loop that checks if that gamepad is already in the list.
            if (!AssignedGamepads.Contains(gamepad) && !PendingGamepads.Contains(gamepad))
            {
                Debug.Log("Detected Gamepad : " + gamepad.displayName);
                PendingGamepads.Add(gamepad);
            }
        }
        
    }

    public void CheckGamepadAssignments()
    {
        foreach (Gamepad gamepad in PendingGamepads.ToList())
        {
            if (gamepad.buttonSouth.wasReleasedThisFrame)
            {
                OnSouthButtonPressed?.Invoke(gamepad);
                AssignedGamepads.Add(gamepad); // Ajoute la manette à la liste des manettes assignées.
                PendingGamepads.Remove(gamepad); // Remove the gamepad from the list of pending gamepads.
                return; // Évite de traiter plusieurs manettes en une frame.
            }
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
        
        // Assign the gamepad to the player's input manager
        PlayerInput playerInput = player.GetComponent<PlayerInput>();
        
        if (playerInput)
        {
            playerInput.SwitchCurrentControlScheme(gamepad);
        }
        else
        {
            Debug.LogError("PlayerInput component not found on the player.");
        }
        
        Debug.Log($"gamepad '{gamepad.displayName}' assigned to player  '{player.name}'");
    }
    
}
