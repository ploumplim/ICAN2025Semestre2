using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GoalSpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public List<GameObject> GoalSpawnPoints;
   
    public GameObject GoalPrefab;
    
    public LevelManager levelManager;
    void Start()
    {
        levelManager = GameManager.Instance.levelManager;
        GameManager.Instance.levelManager.goalSpawner = this;
        foreach (Transform child in transform)
        {
            GoalSpawnPoints.Add(child.gameObject);
        }
        
        foreach (var goalSpawnPoint in GoalSpawnPoints)
        {
            // Instantiate the object and set it as a child of the GameObject that has this script
            GameObject GoalSpawn = Instantiate(GoalPrefab, goalSpawnPoint.transform.position, goalSpawnPoint.transform.rotation, this.transform);
    
            levelManager.GoalList.Add(GoalSpawn);
        }

        var instancePlayerScriptList = GameManager.Instance.PlayerScriptList;
        for (int i = 0; i < instancePlayerScriptList.Count; i++)
        {
            instancePlayerScriptList[i].playerGoalToDefend = levelManager.GoalList[i].gameObject;
        }
    }
    

    public void LinkGoalToPlayer(int playerId)
    {
        int nextGoalIndex = (playerId + 1) % levelManager.GoalList.Count;
        GameManager.Instance.PlayerScriptList[playerId].playerGoalToAttack = levelManager.GoalList[nextGoalIndex].gameObject;
        //Debug.LogWarning(GameManager.Instance.PlayerScriptList[playerId].playerGoalToDefend.name);
        
        GameManager.Instance.PlayerScriptList[playerId].playerGoalToDefend = levelManager.GoalList[playerId].gameObject;
    }

    
}
