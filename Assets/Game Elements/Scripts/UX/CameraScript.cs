using System;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private Vector3[] _lockPoints; 
    private GameObject _cameraObject;
    private Vector3 _targetPoint;

    public GameObject cameraHolderObject;
    public float followSpeed = 5f;
    public float zoomSpeed = 5f;
    public GameObject[] lockPoints;
    public GameObject Floor;
    public Vector3 centerFloor;
    public float distanceTargetCenter;

    [Tooltip("Padding for camera zoom.")]
    public float padding = 2f;

    [Tooltip("Min and Max Camera Distances.")]
    public float minCameraDistance = 10f;
    public float maxCameraDistance = 50f;

    [Tooltip("Min and Max distance between players before adjusting zoom.")]
    public float minTargetDistance = 5f;   // Nouvelle valeur pour éviter que la caméra zoome trop près
    public float maxTargetDistance = 30f;  // Nouvelle valeur pour limiter l’éloignement de la caméra

    private void Start()
    {
        _cameraObject = cameraHolderObject.GetComponentInChildren<Camera>().gameObject;
        centerFloor = Floor.transform.position;
    }

    private void FixedUpdate()
    {
        UpdateCameraPosition();
        UpdateCameraDistance();
    }

    private void UpdateCameraPosition()
    {
        if (lockPoints.Length > 0)
        {
            _lockPoints = new Vector3[lockPoints.Length];

            for (int i = 0; i < lockPoints.Length; i++)
                _lockPoints[i] = lockPoints[i].transform.position;
        }

        _targetPoint = CalculateAveragePoint(_lockPoints);
        cameraHolderObject.transform.position = Vector3.Lerp(
            cameraHolderObject.transform.position, _targetPoint, followSpeed * Time.deltaTime);

        distanceTargetCenter = Vector3.Distance(centerFloor, _targetPoint);
    }

    private void UpdateCameraDistance()
    {
        
        //TODO MODIFIER LE CODE POUR AMELIORER LE ZOOM ET DEZOOM DE LA CAM
        
        // On s'assure que la distance est comprise entre les limites définies
        float clampedDistance = Mathf.Clamp(distanceTargetCenter, maxTargetDistance, minTargetDistance);

        // Calcul de la nouvelle distance avec une interpolation linéaire
        float targetDistance = Mathf.Lerp(minCameraDistance, maxCameraDistance, 
                                          (clampedDistance - minTargetDistance) / (maxTargetDistance - minTargetDistance));
        
        // Récupération de la direction de la caméra
        Vector3 cameraDirection = (_cameraObject.transform.position - cameraHolderObject.transform.position).normalized;

        // Appliquer la nouvelle distance avec un lissage
        _cameraObject.transform.position = Vector3.Lerp(
            _cameraObject.transform.position,
            cameraHolderObject.transform.position + cameraDirection * targetDistance,
            Time.deltaTime * zoomSpeed
        );
    }

    public static Vector3 CalculateAveragePoint(Vector3[] points)
    {
        if (points == null || points.Length == 0)
            return Vector3.zero;

        Vector3 sum = Vector3.zero;
        foreach (Vector3 point in points)
            sum += point;

        return sum / points.Length;
    }

    public void AddPlayerToArray(GameObject player)
    {
        Array.Resize(ref lockPoints, lockPoints.Length + 1);
        lockPoints[^1] = player;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_targetPoint, 0.5f);
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(centerFloor, 0.5f);
    }
}
