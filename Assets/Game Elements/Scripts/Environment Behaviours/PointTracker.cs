using System;
using TMPro;
using UnityEngine;

public class PointTracker : MonoBehaviour
{
    public TextMeshPro pointsText;
    
    public int _points;

    public void Start()
    {
        var instanceLevelManager = GameManager.Instance.levelManager;
        instanceLevelManager.PointTrackers.Add(this);
        // for (int i = 0; i < instanceLevelManager.PointTrackers.Count; i++)
        // {
        //     GameManager.Instance.PlayerScriptList[i].playerGoalToDefend = instanceLevelManager.PointTrackers[i];
        // }
    }

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
