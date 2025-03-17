using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelSelector : MonoBehaviour
{
    public float moveSpeed = 5f;
    
    public Gamepad assignedGamepad;

    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject PlayerBorderGO;
    public HandleGamePads handleGamePads;

    public void Start()
    {
        handleGamePads = FindFirstObjectByType<HandleGamePads>();
        DetectionNewPlayer(assignedGamepad);
    }

    private void DetectionNewPlayer(Gamepad gamepad)
    {
        if (handleGamePads)
        {
            //subscribe to the OnSouthButtonPressed event
            handleGamePads.OnSouthButtonPressed += InstantiatePlayerBorder;
        }
    }

    private void InstantiatePlayerBorder(Gamepad gamepad)
    {
        GameObject playerBorder = Instantiate(PlayerBorderGO, transform.position, Quaternion.identity, panel.transform);
        
    }
}