using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GoalSpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public List<GameObject> GoalSpawnPoints;
   
    [FormerlySerializedAs("GoalPrefab")] public GameObject GoalPrefabCollider;

    [FormerlySerializedAs("goal")] public GameObject goalPrefab;

    public GameObject goalMesh;
    
    private LevelManager levelManager;
    void Start()
    {
        // Et dans GoalSpawner.cs
        levelManager = GameManager.Instance.levelManager;
        foreach (Transform child in transform)
        {
            GoalSpawnPoints.Add(child.gameObject);
        }
        goalMesh = Instantiate(goalPrefab, new Vector3(0,5,-10f), this.transform.rotation, this.transform);
        goalMesh.transform.localScale = new Vector3(1.25f, 1.25f, 1.7f);
        GameManager.Instance.levelManager.GoalSpawnerList.Add(goalMesh);
        
        foreach (var goalSpawnPoint in GoalSpawnPoints)
        {
            // Instantiate the object and set it as a child of the GameObject that has this script
            GameObject GoalSpawnCollider = Instantiate(GoalPrefabCollider, goalSpawnPoint.transform.position, goalSpawnPoint.transform.rotation, this.transform);
            
            
            GoalSpawnCollider.GetComponent<PointTracker>().linkedGoal = GoalSpawnCollider;
            levelManager.GoalList.Add(GoalSpawnCollider);
        }

        var instancePlayerScriptList = GameManager.Instance.PlayerScriptList;
        for (int i = 0; i < instancePlayerScriptList.Count; i++)
        {
            instancePlayerScriptList[i].playerGoalToAttack = levelManager.GoalList[i].gameObject;
        }
        
    }
    
    

    
}
