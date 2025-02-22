using System;
using TMPro;
using UnityEngine;

public class PointTracker : MonoBehaviour
{
    public GameObject posPointsObject;
    public GameObject negPointsObject;
    public TextMeshPro pointsText;
    
    private int _points;

    public void AddPoints()
    {
        _points++;
        pointsText.text = _points.ToString();
    }
    
    public void ResetPoints()
    {
        _points = 0;
        pointsText.text = _points.ToString();
    }
}
