using System;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // ~~~~~~~~~~~~~~~~VARIABLES~~~~~~~~~~~~~~~~
    // ---------------PRIVATE---------------
    private float _trueCameraSize; // The actual size of the camera, calculated from the size modifier.
    private float _sizeModifier; // The size modifier of the camera, based on the distance between the player and the ball.
    private Vector3[] _lockPoints; // The array of points that the camera can lock itself to.
    private GameObject cameraObject; // The camera component of the camera object.
    private Vector3 _targetPoint; // The point that the camera should lock itself to.

    // ---------------PUBLIC---------------
    [Tooltip("The gameobject holding the camera. The code will move THIS object, while the camera is fixed to it as " +
             "a child of it.")]
    public GameObject cameraHolderObject;

    [Tooltip("The camera's follow speed.")]
    public float followSpeed = 5f;

    [Tooltip("The camera's speed when moving away from the scene.")]
    public float zoomSpeed = 5f;

    [Tooltip("This list contains all objects that the camera should use as points to lock itself.")]
    public GameObject[] lockPoints;

    public void Start()
    {
        // Get the camera component of the camera object
        cameraObject = cameraHolderObject.GetComponentInChildren<Camera>().gameObject;
        
    }

    public void FixedUpdate()
    {
        UpdateCameraPosition();
        // UpdateCameraDistance();
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
        else
        {
            // If the lockPoints array is empty, return
            return;
        }

        // Calculate the average point between the lock points
        _targetPoint = CalculateAveragePoint(_lockPoints);
        
        

        // Move the camera holder object to the middle point
        Vector3 newPosition = Vector3.Lerp(cameraHolderObject.transform.position, _targetPoint, followSpeed * Time.deltaTime);
        
        // Set the camera holder object to the new position
        cameraHolderObject.transform.position = newPosition;


        // var newPositionXvalue = cameraHolderObject.transform.position;
        // newPositionXvalue.x = newPosition.x;
        // var cameraHolderObjectVec3 = cameraHolderObject.transform.position;
        // cameraHolderObjectVec3.x = Mathf.Lerp(cameraHolderObject.transform.position.x, newPositionXvalue.x, followSpeed * Time.deltaTime);
        // cameraHolderObject.transform.position = cameraHolderObjectVec3;
        
        
        
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

    public void AddObjectToArray(GameObject @object)
    {
        Array.Resize(ref lockPoints, lockPoints.Length + 1);
        lockPoints[^1] = @object;
    }
    
    public void RemoveObjectFromArray(GameObject @object)
    {
        for (int i = 0; i < lockPoints.Length; i++)
        {
            if (lockPoints[i] == @object)
            {
                lockPoints[i] = null;
                // Remove the null element from the array
                lockPoints = Array.FindAll(lockPoints, x => x != null);
            }
        }
    }

    private void UpdateCameraDistance()
    {
        // Recover the vector distance between the camera holder object and the camera object
        Vector3 tPointAndCamVec3 = _targetPoint - cameraHolderObject.transform.position;
        
        // Clamp the distance between the min and max distance
        float distance = tPointAndCamVec3.magnitude;
        
        // Check if all objects within the lockPoints array are visible
        if (!AreAllLockPointsVisible())
        {
            // If they are, move the camera holder object away from the camera object
            cameraHolderObject.transform.position = Vector3.Lerp(cameraHolderObject.transform.position,
                cameraHolderObject.transform.position + tPointAndCamVec3, zoomSpeed * Time.deltaTime);
        }
    }

    private bool AreAllLockPointsVisible()
    {
        Camera gameCamera = cameraObject.GetComponent<Camera>();
        
        if (_lockPoints == null || _lockPoints.Length == 0)
        {
            return false;
        }
        else
        {
            foreach (Vector3 point in _lockPoints)
            {
                Vector3 viewportPoint = gameCamera.WorldToViewportPoint(point);
                if (viewportPoint.x < 0 || viewportPoint.x > 1 || viewportPoint.y < 0 || viewportPoint.y > 1)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_targetPoint, 0.5f);
    }
}