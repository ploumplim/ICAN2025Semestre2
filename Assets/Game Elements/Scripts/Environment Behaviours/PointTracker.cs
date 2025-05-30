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
    public ParticleSystem goalParticles;
    public ParticleSystem otherGoalParticles;

    public void Start()
    {
        
        var instanceLevelManager = GameManager.Instance.levelManager;
        instanceLevelManager.PointTrackers.Add(this);
        
    }

    public void AddPoints()
    {
        ballSM = GameManager.Instance.levelManager.gameBall.GetComponent<BallSM>();
        // Verify if the ball's owner player is the one attacking this goal
        if (ballSM.ballOwnerPlayer != null && 
            ballSM.ballOwnerPlayer.GetComponent<PlayerScript>().playerGoalToAttack == linkedGoal)
        {
            goalParticles.Play();
            otherGoalParticles.Play();
            ballSM.ballOwnerPlayer.GetComponent<PlayerScript>().playerPoint++;
            
            GameManager.Instance.levelManager.gameCameraScript.screenShakeGO.GetComponent<ScreenShake>().StartGoalScreenShake(ballSM.rb.linearVelocity.magnitude);
            Debug.Log("Player " + ballSM.ballOwnerPlayer.name + " scored" +ballSM.ballOwnerPlayer.GetComponent<PlayerScript>().playerPoint );
            _points++;
            GameManager.Instance.levelManager.OnGoalScored.Invoke(_points);
            
            pointsText.text = _points.ToString();

            MoveBallSpawnPositionToLoosingPlayer();

            BallResetPositionAfterGoal();
            if (ballSM.ballOwnerPlayer.GetComponent<PlayerScript>().playerPoint >= GameManager.Instance.levelManager.pointNeededToWin)
            {
                Debug.Log("Player " + ballSM.ballOwnerPlayer.name + " won the set");
                ballSM.ballOwnerPlayer.GetComponent<PlayerScript>().playerGlobalPoint++;
            }
            
            //TODO Faire un slowDown de timeScale 0.5 pendant 1 seconde pour le goal
            // Slowdown du temps pendant 1 seconde
            StartCoroutine(SlowDownTimeOnGoal());

            
        }

        // if (ballSM.ballOwnerPlayer.GetComponent<PlayerScript>().playerGoalToAttack!=linkedGoal)
        // {
        //     Debug.Log("Player " + ballSM.ballOwnerPlayer.name + " is not attacking this goal: " + linkedGoal.name + "he need to attack: " + ballSM.ballOwnerPlayer.GetComponent<PlayerScript>().playerGoalToAttack.name);
        // }
        
    }
    
    private System.Collections.IEnumerator SlowDownTimeOnGoal()
    {
        // Time.timeScale = GameManager.Instance.levelManager.SlowDownOnGoalTimer;
        // yield return new WaitForSecondsRealtime(1f);
        // Time.timeScale = 1f;
        yield return null;
    }

    private void MoveBallSpawnPositionToLoosingPlayer()
    {

        foreach (var player in GameManager.Instance.PlayerScriptList)
        {
             if (player.playerGoalToAttack == gameObject)
             {
                 defendingPlayer = player;
                 
                 Transform transformPosition = GameManager.Instance.levelManager.ballSpawnPosition.transform;
                 
                 transformPosition.position = player.playerSpawnPoint.transform.position;
                 
             }
             else
             {
                
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
