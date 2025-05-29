using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CameraScript : MonoBehaviour
{
    // ~~~~~~~~~~~~~~~~VARIABLES~~~~~~~~~~~~~~~~
    // ---------------PRIVATE---------------
    private float _trueCameraSize; // The actual size of the camera, calculated from the size modifier.
    private float _sizeModifier; // The size modifier of the camera, based on the distance between the player and the ball.
    private Vector3[] _lockPoints; // The array of points that the camera can lock itself to.
    private GameObject cameraObject; // The camera component of the camera object.
    private Vector3 _targetPoint; // The point that the camera should lock itself to.
    public ScreenShake screenShakeGO; // The screen shake component of the camera object.

    // ---------------PUBLIC---------------
    [Tooltip("The gameobject holding the camera. The code will move THIS object, while the camera is fixed to it as " +
             "a child of it.")]
    public GameObject cameraHolderObject;

    [FormerlySerializedAs("followSpeed")] [Tooltip("The camera's follow speed.")]
    public float maxFollowSpeed = 5f;
    public Vector3 maximumDistances = new Vector3(1,0,1); // The limits of the camera's movement.
    public GameObject[] lockPoints;

    public void Start()
    {
        // Get the camera component of the camera object
        cameraObject = cameraHolderObject.GetComponentInChildren<Camera>().gameObject;
        GameManager.Instance.multiplayerManager.camera = this;
        GameManager.Instance.levelManager.gameCameraScript = this;

        // Add the Vector3.zero to the _lockPoints array as the first element
    }

    public void FixedUpdate()
    {
        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        // Get the length of the lockPoints array
        int lockPointsLength = lockPoints.Length;

        // If the lockPoints array is not empty
        if (lockPointsLength > 0)
        {
            // Create a new Vector3 array with the same length as the lockPoints array
            _lockPoints = new Vector3[lockPointsLength + 1];

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
        Vector3 newPosition = Vector3.Lerp(cameraHolderObject.transform.position, _targetPoint, maxFollowSpeed);
        
        // Clamp the new position to the maximum distances, both in the positives and negatives
        newPosition.x = Mathf.Clamp(newPosition.x, -maximumDistances.x, maximumDistances.x);
        newPosition.y = Mathf.Clamp(newPosition.y, -maximumDistances.y, maximumDistances.y);
        newPosition.z = Mathf.Clamp(newPosition.z, -maximumDistances.z, maximumDistances.z);
        
        // Set the camera holder object to the new position
        cameraHolderObject.transform.position = newPosition;
        
        
        
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
    
    
    public void StartShake(float duration, float magnitude,float multiplier,float ballSpeed)
    {
        Debug.Log("StartShake");
        StartCoroutine(ShakeCamera(duration, magnitude, multiplier, ballSpeed));
    }

    public IEnumerator ShakeCamera(float duration, float magnitude, float multiplier, float ballSpeed)
    {
        Debug.Log("ShakeCamera");
        Vector3 originalPosition = cameraHolderObject.transform.localPosition;
        float elapsed = 0f;

        // Calculer le facteur de boost basé sur ballSpeed et multiplier
        float speedBoostFactor = ballSpeed * multiplier;

        while (elapsed < duration)
        {
            // Appliquer le facteur de boost à l'intensité du shake
            float offsetX = Random.Range(-1f, 1f) * magnitude * speedBoostFactor;
            float offsetY = Random.Range(-1f, 1f) * magnitude * speedBoostFactor;

            cameraHolderObject.transform.localPosition = new Vector3(
                originalPosition.x + offsetX,
                originalPosition.y + offsetY,
                originalPosition.z
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        cameraHolderObject.transform.localPosition = originalPosition;
    }
    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_targetPoint, 0.1f);
    }
}