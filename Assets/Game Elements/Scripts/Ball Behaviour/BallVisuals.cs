using System;
using Unity.VisualScripting;
using UnityEngine;

public class BallVisuals : MonoBehaviour
{
    // ~~~~~~~~~~~~~~~~VARIABLES~~~~~~~~~~~~~~~~
    // ---------------PUBLIC---------------
    [Tooltip("Game Object containing the UI image used to position the ball behind walls.")]
    public GameObject ballMarker;
    
    // ---------------PRIVATE---------------
    private BallSM ballSM;
    private Camera _mainCamera;

    public void Start()
    {
        ballSM = GetComponent<BallSM>();
        _mainCamera = Camera.main;
    }

    private void FixedUpdate()
    {
        BallMarker();
    }

    private void BallMarker()
    {
        
        // Convert the ball's position to screen space
        Vector3 screenPosition = _mainCamera.WorldToScreenPoint(transform.position);

        // Check if the ball is behind a wall
        bool isBehindWall = Physics.Linecast(_mainCamera.transform.position, transform.position, out RaycastHit hit);
        
        // Update the position of the ball marker in the canvas
        ballMarker.transform.position = screenPosition;
        ballMarker.SetActive(isBehindWall);
    }
}
