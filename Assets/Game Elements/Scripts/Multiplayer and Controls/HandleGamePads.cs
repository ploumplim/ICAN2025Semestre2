using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandleGamePads : MonoBehaviour
{
    public event Action<Gamepad> OnSouthButtonPressed;
    public event Action<Gamepad> OnSelectButtonPressed; 

    public readonly HashSet<Gamepad> PendingGamepads = new HashSet<Gamepad>();
    public readonly HashSet<Gamepad> AssignedGamepads = new HashSet<Gamepad>();

    // Add this property to display PendingGamepads in the inspector
    public List<Gamepad> PendingGamepadsList = new List<Gamepad>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        GameManager.Instance.handleGamePads = this;
    }

    private void Update()
    {
        // Check for new gamepads
        foreach (var gamepad in Gamepad.all)
        {
            if (!AssignedGamepads.Contains(gamepad) && !PendingGamepads.Contains(gamepad))
            {
                Debug.Log("Detected Gamepad : " + gamepad.displayName);
                PendingGamepads.Add(gamepad);
            }
        }

        // Detect gamepad inputs
        //DetectGamepadInput();
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

            if (gamepad.selectButton.wasReleasedThisFrame)
            {
                OnSelectButtonPressed?.Invoke(gamepad);
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
//     public void DetectGamepadInput()
//     {
//         foreach (var gamepad in Gamepad.all)
//         {
//             if (gamepad.buttonSouth.wasPressedThisFrame)
//             {
//                 Debug.Log($"Gamepad {gamepad.displayName}: South button pressed");
//             }
//             if (gamepad.buttonNorth.wasPressedThisFrame)
//             {
//                 Debug.Log($"Gamepad {gamepad.displayName}: North button pressed");
//             }
//             if (gamepad.buttonEast.wasPressedThisFrame)
//             {
//                 Debug.Log($"Gamepad {gamepad.displayName}: East button pressed");
//             }
//             if (gamepad.buttonWest.wasPressedThisFrame)
//             {
//                 Debug.Log($"Gamepad {gamepad.displayName}: West button pressed");
//             }
//             if (gamepad.leftTrigger.wasPressedThisFrame)
//             {
//                 Debug.Log($"Gamepad {gamepad.displayName}: Left trigger pressed");
//             }
//             if (gamepad.rightTrigger.wasPressedThisFrame)
//             {
//                 Debug.Log($"Gamepad {gamepad.displayName}: Right trigger pressed");
//             }
//             if (gamepad.leftShoulder.wasPressedThisFrame)
//             {
//                 Debug.Log($"Gamepad {gamepad.displayName}: Left shoulder pressed");
//             }
//             if (gamepad.rightShoulder.wasPressedThisFrame)
//             {
//                 Debug.Log($"Gamepad {gamepad.displayName}: Right shoulder pressed");
//             }
//             if (gamepad.dpad.up.wasPressedThisFrame)
//             {
//                 Debug.Log($"Gamepad {gamepad.displayName}: D-pad up pressed");
//             }
//             if (gamepad.dpad.down.wasPressedThisFrame)
//             {
//                 Debug.Log($"Gamepad {gamepad.displayName}: D-pad down pressed");
//             }
//             if (gamepad.dpad.left.wasPressedThisFrame)
//             {
//                 Debug.Log($"Gamepad {gamepad.displayName}: D-pad left pressed");
//             }
//             if (gamepad.dpad.right.wasPressedThisFrame)
//             {
//                 Debug.Log($"Gamepad {gamepad.displayName}: D-pad right pressed");
//             }
//             if (gamepad.leftStickButton.wasPressedThisFrame)
//             {
//                 Debug.Log($"Gamepad {gamepad.displayName}: Left stick button pressed");
//             }
//             if (gamepad.rightStickButton.wasPressedThisFrame)
//             {
//                 Debug.Log($"Gamepad {gamepad.displayName}: Right stick button pressed");
//             }
//         }
//     }
 }