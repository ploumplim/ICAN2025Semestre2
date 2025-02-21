using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem.Controls;

public class MultiplayerManager : MonoBehaviour
{
    private PlayerInputManager playerInputManager;
    private Dictionary<Gamepad, int> controllerIDs = new Dictionary<Gamepad, int>();
    private HashSet<Gamepad> pendingGamepads = new HashSet<Gamepad>(); // Manettes en attente de détection
    private int nextID = 1; // Commence à 1 pour éviter un ID nul

    void Start()
    {
        playerInputManager = GetComponent<PlayerInputManager>();

        // Ajoute toutes les manettes déjà branchées à la liste d'attente
        foreach (var gamepad in Gamepad.all)
        {
            pendingGamepads.Add(gamepad);
        }
    }

    void Update()
    {
        // Vérifie chaque manette en attente d'un premier input
        foreach (var gamepad in pendingGamepads.ToList()) 
        {
            if (gamepad.allControls.Any(control => control is ButtonControl button && button.wasPressedThisFrame))
            {
                RegisterController(gamepad);
                pendingGamepads.Remove(gamepad);
                return; // Évite plusieurs détections en même temps
            }
        }
    }

    private void RegisterController(Gamepad gamepad)
    {
        // Vérifie si la manette a déjà un ID (au cas où)
        if (!controllerIDs.ContainsKey(gamepad))
        {
            controllerIDs[gamepad] = nextID++;
        }

        // Ajoute le joueur
        playerInputManager.JoinPlayer();
        Debug.Log($"Joueur ajouté avec la manette : {gamepad.displayName} (ID: {controllerIDs[gamepad]})");
    }
}