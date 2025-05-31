using TMPro;
using UnityEngine;

public class PointFeedback : MonoBehaviour
{
    private PointTracker pointTracker;

    [SerializeField] private TextMeshPro pointText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pointTracker = GetComponent<PointTracker>();
    }

    // Update is called once per frame
    void Update()
    {
        pointText.text = pointTracker._points.ToString();
    }
}
