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
            GoalSpawn.GetComponent<PointTracker>().linkedGoal = GoalSpawn;
            levelManager.GoalList.Add(GoalSpawn);
        }

        var instancePlayerScriptList = GameManager.Instance.PlayerScriptList;
        for (int i = 0; i < instancePlayerScriptList.Count; i++)
        {
            instancePlayerScriptList[i].playerGoalToAttack = levelManager.GoalList[i].gameObject;
        }
    }
    

    public void LinkGoalToPlayer(int playerId)
    {
        
        int currenGoalscenePlaying = GameManager.Instance.currentSceneID * 2;
        if (playerId % 2 == 0)
        {
            GameManager.Instance.PlayerScriptList[playerId].playerGoalToDefend = levelManager.GoalList[currenGoalscenePlaying+1].gameObject;
            //TODO : Assign le mesh du goal to defend du player 2
        
            GameManager.Instance.PlayerScriptList[playerId].playerGoalToAttack = levelManager.GoalList[currenGoalscenePlaying].gameObject;
        }
        else
        {
            GameManager.Instance.PlayerScriptList[playerId].playerGoalToDefend = levelManager.GoalList[currenGoalscenePlaying].gameObject;
            //TODO : Assign le mesh du goal to defend du player 1
            
        
            GameManager.Instance.PlayerScriptList[playerId].playerGoalToAttack = levelManager.GoalList[currenGoalscenePlaying+1].gameObject;
        }
        
        
        
        foreach (Transform childTransform in GameManager.Instance.PlayerScriptList[playerId].playerGoalToDefend.transform)
        {
            if (childTransform.name == "Renderer")
            {
                childTransform.GetComponent<MeshRenderer>().material.color =
                    GameManager.Instance.PlayerScriptList[playerId].GetComponent<PlayerVisuals>().playerCapMaterial.color;
            }
        }

        
    }

    
}
