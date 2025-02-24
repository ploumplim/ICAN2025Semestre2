using System;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // ~~~~~~~~~~~~~~~~VARIABLES~~~~~~~~~~~~~~~~
    // ---------------PRIVATE---------------
    private float _trueCameraSize; // The actual size of the camera, calculated from the size modifier.
    private float _sizeModifier; // The size modifier of the camera, based on the distance between the player and the ball.
    private Vector3[] _lockPoints; // The array of points that the camera can lock itself to.
    private Camera _camera; // The camera component of the camera object.
    
    // ---------------PUBLIC---------------
    [Tooltip("The gameobject holding the camera. The code will move THIS object, while the camera is fixed to it as " +
             "a child of it.")]
    public GameObject cameraHolderObject;

    [Tooltip("The camera's follow speed.")]
    public float followSpeed = 5f;
    
    [Tooltip("This list contains all objects that the camera should use as points to lock itself.")]
    public GameObject[] lockPoints;
    
    [Tooltip("The padding each lock point will have from the edge of the screen.")]
    public float padding = 2f;
    
    public void Start()
    {
        // Get the camera component of the camera object
        _camera = GetComponent<Camera>();
    }
    public void FixedUpdate()
    {
        UpdateCameraPosition();
        AdjustFoV();
    }
    
    private void UpdateCameraPosition()
    {
        
        // Get the length of the lockPoints array
        int lockPointsLength = lockPoints.Length;
        
        // If the lockPoints array is not empty
        if (lockPointsLength > 0)
        {
            // Create a new Vector3 array with the same length as the lockPoints array
            _lockPoints = new Vector3[lockPointsLength];
            
            // Loop through the lockPoints array
            for (int i = 0; i < lockPointsLength; i++)
            {
                // Set the current index of the _lockPoints array to the position of the current index of the lockPoints array
                _lockPoints[i] = lockPoints[i].transform.position;
            }
        }
        
        // Calculate the average point between the lock points
        Vector3 averagePoint = CalculateAveragePoint(_lockPoints);
        
        
        // Move the camera holder object to the middle point
        cameraHolderObject.transform.position = Vector3.Lerp(cameraHolderObject.transform.position,
            averagePoint,followSpeed * Time.deltaTime);
    }
    
    
    public static Vector3 CalculateAveragePoint(Vector3[] points)
    {
        if (points == null || points.Length == 0)
        {
            return Vector3.zero;
        }

        Vector3 sum = Vector3.zero;
        foreach (Vector3 point in points)
        {
            sum += point;
        }

        return sum / points.Length;
    }
    
    public void AddPlayerToArray(GameObject player)
    {
        Array.Resize(ref lockPoints, lockPoints.Length + 1);
        lockPoints[^1] = player;
    }

    public void AdjustFoV()
    {
        if (_lockPoints == null || _lockPoints.Length == 0)
        {
            return;
        }

        // Calculate the bounding box that contains all the lock points
        Bounds bounds = new Bounds(_lockPoints[0], Vector3.zero);
        foreach (Vector3 point in _lockPoints)
        {
            bounds.Encapsulate(point);
        }

        // Calculate the required height to fit the bounding box
        float requiredHeight = bounds.extents.magnitude + padding;

        // Adjust the camera's Y position
        Vector3 newPosition = cameraHolderObject.transform.position;
        newPosition.y = requiredHeight;
        cameraHolderObject.transform.position = newPosition;
    }
    
}

// ---------------DEPRECATED---------------

    // private void UpdateCameraSize()
    // {
    //     // Calculate the distance between the player and the ball
    //     float distance = Vector3.Distance(_playerPosition, _ballPosition);
    //     
    //     // Calculate the size modifier based on the distance
    //     _sizeModifier = distance * expansionMultiplier;
    //     
    //     // Calculate the true camera size
    //     _trueCameraSize = baseSize + _sizeModifier;
    //     
    //     // Set the camera size
    //     GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize,
    //         _trueCameraSize, zoomSpeed * Time.deltaTime);
    // }