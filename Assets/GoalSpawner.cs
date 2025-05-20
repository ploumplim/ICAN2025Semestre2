using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GoalSpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public List<GameObject> GoalSpawnPoints;
    public List<GameObject> GoalList;
    public GameObject GoalPrefab;
    void Start()
    {
        GameManager.Instance.levelManager.goalSpawner = this;
        foreach (Transform child in transform)
        {
            GoalSpawnPoints.Add(child.gameObject);
        }
        
        foreach (var goalSpawnPoint in GoalSpawnPoints)
        {
            GameObject GoalSpawn = Instantiate(GoalPrefab, goalSpawnPoint.transform.position, goalSpawnPoint.transform.rotation);
            GoalList.Add(GoalSpawn);
        }

        var instancePlayerScriptList = GameManager.Instance.PlayerScriptList;
        for (int i = 0; i < instancePlayerScriptList.Count; i++)
        {
            instancePlayerScriptList[i].playerGoalToDefend = GoalList[i].gameObject;
        }
    }

    public void LinkGoalToPlayer(int playerId)
    {
        int nextGoalIndex = (playerId + 1) % GoalList.Count;
        GameManager.Instance.PlayerScriptList[playerId].playerGoalToAttack = GoalList[nextGoalIndex].gameObject;
        //Debug.LogWarning(GameManager.Instance.PlayerScriptList[playerId].playerGoalToDefend.name);
        
        GameManager.Instance.PlayerScriptList[playerId].playerGoalToDefend = GoalList[playerId].gameObject;
    }

    
}
