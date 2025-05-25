using System;
using TMPro;
using UnityEngine;

public class PointTracker : MonoBehaviour
{
    public TextMeshPro pointsText;
    
    public int _points;
    
    public PlayerScript defendingPlayer;

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

        MoveBallSpawnPositionToLoosingPlayer();
        
        //TODO : Instantiate Ball function 
        
        
        
        BallResetPositionAfterGoal();
        
    }

    private void MoveBallSpawnPositionToLoosingPlayer()
    {
        LevelManager levelManager = GameManager.Instance.levelManager;

        foreach (var player in GameManager.Instance.PlayerScriptList)
        {
            if (player.playerGoalToAttack == gameObject)
            {
                defendingPlayer = player;

                // Calculate the average distance between levelManager.centerPoint and playerPosition
                Transform transformPosition = GameManager.Instance.levelManager.ballSpawnPosition.transform;
                Vector3 centerPoint = levelManager.centerPoint.transform.position;
                Vector3 playerPosition = defendingPlayer.transform.position;

                float averageDistance = Vector3.Distance(centerPoint, playerPosition);
                Debug.Log("Average Distance: " + averageDistance);

                // Set a new Vector3 based on the average distance
                Vector3 direction = (player.playerSpawnPoint.transform.position - centerPoint).normalized;
                Vector3 newBallSpawnPosition = centerPoint + direction * averageDistance;

                // Move the ball spawn position to the calculated position
                transformPosition.position = newBallSpawnPosition;
                Destroy(GameManager.Instance.levelManager.gameBall);
                GameManager.Instance.levelManager.SpawnBall();
                Debug.Log("New Ball Spawn Position: " + newBallSpawnPosition);
            }
        }
    }

    // private static void SpawnNewBall(Transform transformPosition)
    // {
    //     Debug.Log("Ball Destroyed at position: " + GameManager.Instance.levelManager.gameBall.transform.position);
    //     Destroy(GameManager.Instance.levelManager.gameBall);
    //             
    //     GameObject newball = Instantiate(GameManager.Instance.levelManager.ballPrefab, transformPosition.position, Quaternion.identity);
    //     GameManager.Instance.levelManager.gameBall = newball;
    //     
    //     
    //     GameManager.Instance.levelManager.gameCameraScript.AddObjectToArray(newball);
    // }

    private void BallResetPositionAfterGoal()
    {
        
        // Reset the ball
        var ball = GameManager.Instance.levelManager.gameBall.GetComponent<Rigidbody>(); // Assuming ball is a reference to the Rigidbody
        Debug.Log(ball.transform.position);
        ball.transform.position = GameManager.Instance.levelManager.ballSpawnPosition.position;
        ball.linearVelocity = Vector3.zero;
        ball.angularVelocity = Vector3.zero;
    }

    public void ResetPoints()
    {
        _points = 0;
        pointsText.text = _points.ToString();
    }
}
