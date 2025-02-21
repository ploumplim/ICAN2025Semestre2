using System;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // ~~~~~~~~~~~~~~~~VARIABLES~~~~~~~~~~~~~~~~
    // ---------------PRIVATE---------------
    private float _trueCameraSize; // The actual size of the camera, calculated from the size modifier.
    private float _sizeModifier; // The size modifier of the camera, based on the distance between the player and the ball.
    private Vector3 _playerPosition; // The position of the player.
    private Vector3 _ballPosition; // The position of the ball.
    private Vector3 _middlePoint; // The middle point between the player and the ball.
    
    // ---------------PUBLIC---------------
    [Tooltip("The gameobject holding the camera. The code will move THIS object, while the camera is fixed to it as its" +
             "a child of it.")]
    public GameObject cameraHolderObject;
    [Tooltip("The player object that the camera will follow.")]
    public GameObject player;
    [Tooltip("The ball object that the camera has to keep in sight.")]
    public GameObject ball;
    [Tooltip("The game object at the center of the scene.")]
    public GameObject center;
    [Tooltip("The base size of the camera.")]
    public float baseSize = 5f;
    [Tooltip("The camera's expansion multipler : How much the camera will expand based on the distance between the player and the ball.")]
    public float expansionMultiplier = 0.5f;
    [Tooltip("The camera's follow speed.")]
    public float followSpeed = 5f;
    [Tooltip("The camera's zoom speed.")]
    public float zoomSpeed = 5f;
    
    
    
    public void FixedUpdate()
    {
        UpdateCameraPosition();
        // UpdateCameraSize();
    }
    
    private void UpdateCameraPosition()
    {
        // Get the player's position
        _playerPosition = player.transform.position;
        // Get the ball's position
        _ballPosition = ball.transform.position;
        // Get the center's position
        _middlePoint = center.transform.position;
        
        
        // Calculate the position between the player. the ball and the Vector3.zero of the scene.
        Vector3 middlePoint = (_playerPosition + _ballPosition + _middlePoint) / 3;
        
        
        // Move the camera holder object to the middle point
        cameraHolderObject.transform.position = Vector3.Lerp(cameraHolderObject.transform.position,
            middlePoint,followSpeed * Time.deltaTime);
        
        // Make the camera rise in the Y axis to keep the ball in sight
        cameraHolderObject.transform.position = new Vector3(cameraHolderObject.transform.position.x,
            Mathf.Lerp(cameraHolderObject.transform.position.y, _ballPosition.y, followSpeed * Time.deltaTime),
            cameraHolderObject.transform.position.z);
    }
    
    private void UpdateCameraSize()
    {
        // Calculate the distance between the player and the ball
        float distance = Vector3.Distance(_playerPosition, _ballPosition);
        
        // Calculate the size modifier based on the distance
        _sizeModifier = distance * expansionMultiplier;
        
        // Calculate the true camera size
        _trueCameraSize = baseSize + _sizeModifier;
        
        // Set the camera size
        GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize,
            _trueCameraSize, zoomSpeed * Time.deltaTime);
    }
    
    
}
