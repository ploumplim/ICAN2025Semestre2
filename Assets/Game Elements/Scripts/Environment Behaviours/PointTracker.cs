using System;
using TMPro;
using UnityEngine;

public class PointTracker : MonoBehaviour
{
    public TextMeshPro pointsText;
    
    public int _points;
    
    public PlayerScript defendingPlayer;

    public BallSM ballSM;

    public GameObject linkedGoal;

    public void Start()
    {
        ballSM = GameManager.Instance.levelManager.gameBall.GetComponent<BallSM>();
        var instanceLevelManager = GameManager.Instance.levelManager;
        instanceLevelManager.PointTrackers.Add(this);
        
    }

    public void AddPoints()
    {
        // Verify if the ball's owner player is the one attacking this goal
        if (ballSM.ballOwnerPlayer != null && 
            ballSM.ballOwnerPlayer.GetComponent<PlayerScript>().playerGoalToAttack == linkedGoal)
        {
            _points++;
            pointsText.text = _points.ToString();

            MoveBallSpawnPositionToLoosingPlayer();

            BallResetPositionAfterGoal();
        }
        else
        {
            Debug.LogWarning("The ball's owner player does not match the attacking goal.");
        }
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
            
                 // Set a new Vector3 based on the average distance
                 Vector3 direction = (player.playerSpawnPoint.transform.position - centerPoint).normalized;
                 Vector3 newBallSpawnPosition = centerPoint + direction * averageDistance;
            
                 // Move the ball spawn position to the calculated position
                 transformPosition.position = newBallSpawnPosition;
                 
             }
             
        }
        GameManager.Instance.levelManager.gameCameraScript.RemoveObjectFromArray(GameManager.Instance.levelManager.gameBall);
        Destroy(GameManager.Instance.levelManager.gameBall);
        GameManager.Instance.levelManager.SpawnBall();
    }

    private void BallResetPositionAfterGoal()
    {
        
        // Reset the ball
        var ball = GameManager.Instance.levelManager.gameBall.GetComponent<Rigidbody>(); // Assuming ball is a reference to the Rigidbody
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
