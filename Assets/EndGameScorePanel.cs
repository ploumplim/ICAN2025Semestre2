using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EndGameScorePanel : MonoBehaviour
{
    public GameObject EndGameScorePanelGO;
    public GameObject PlayerScoreGO;
    public int playerCount;
    public int winnerTimerSpot;
    public GameObject WinnerScoreGO;
    public GameObject WinnerCenterPoint;
    public PlayerScript playerTopScore;
    public float SizingUpSpotValue;
    public List<GameObject> playerSpotList;
    public List<GameObject> playerScoreListGO;
    public List<(GameObject player, int score)> playerScores = new List<(GameObject player, int score)>();

    public void StartEndGamePanel()
    {
        ShowEndGameScorePanel();
        playerCount = GameManager.Instance.PlayerScriptList.Count;
        SetVisiblePlayerScores();
        UpdateScoreList(GameManager.Instance.PlayerScriptList);
        UpdateTopPlayer();
        MoveWinnerToCenter();
        ScaleUpWinnerSpot();
        StartCoroutine(StartWinnerTimer());
    }

    private void ShowEndGameScorePanel()
    {
        EndGameScorePanelGO.SetActive(true);
    }

    private void SetVisiblePlayerScores()
    {
        for (int i = 0; i < playerCount; i++)
        {
            GameObject playerScoreGO = Instantiate(PlayerScoreGO, playerSpotList[i].transform);
            playerScoreListGO.Add(playerScoreGO);
        }
    }

    private void UpdateScoreList(List<PlayerScript> players)
    {
        playerScores = players
            .Select(player => (player.gameObject, player.GetComponent<PlayerPointTracker>().points))
            .ToList();
    }

    private void UpdateTopPlayer()
    {
        var topScorer = playerScores.OrderByDescending(ps => ps.score).FirstOrDefault();
        playerTopScore = topScorer.player?.GetComponent<PlayerScript>();

        if (playerTopScore != null)
        {
            int topScorerIndex = GetPlayerIndex(topScorer.player);
            if (IsValidIndex(topScorerIndex, playerScoreListGO))
            {
                WinnerScoreGO = playerScoreListGO[topScorerIndex];
            }
        }
    }

    private void MoveWinnerToCenter()
    {
        if (playerTopScore != null)
        {
            int winnerIndex = GetPlayerIndex(playerTopScore.gameObject);
            if (IsValidIndex(winnerIndex, playerSpotList))
            {
                Debug.Log(WinnerScoreGO.name);
                //WinnerScoreGO = playerSpotList[winnerIndex];
                WinnerScoreGO.transform.SetParent(WinnerCenterPoint.transform);
                WinnerScoreGO.transform.localPosition = Vector3.zero;
                WinnerScoreGO.transform.localRotation = Quaternion.identity;
            }
        }
    }

    private void ScaleUpWinnerSpot()
    {
        if (playerTopScore != null)
        {
            int winnerIndex = GetPlayerIndex(playerTopScore.gameObject);
            if (IsValidIndex(winnerIndex, playerSpotList))
            {
                playerSpotList[winnerIndex].transform.localScale = Vector3.one * SizingUpSpotValue;
            }
        }
    }

    private IEnumerator StartWinnerTimer()
    {
        yield return new WaitForSeconds(winnerTimerSpot);
        LerpWinnerToOriginalPosition();
    }

    private void LerpWinnerToOriginalPosition()
    {
        if (playerTopScore != null)
        {
            int winnerIndex = GetPlayerIndex(playerTopScore.gameObject);
            if (IsValidIndex(winnerIndex, playerSpotList))
            {
                Debug.Log($"Winner's PlayerSpotScore: {playerSpotList[winnerIndex].name}");
                Vector3 targetPosition = playerSpotList[winnerIndex].transform.position;

                // Start a coroutine to smoothly move the WinnerScoreGO
                StartCoroutine(LerpPosition(WinnerScoreGO, targetPosition, 1.0f)); // 1.0f is the duration
            }
        }
    }

    private IEnumerator LerpPosition(GameObject target, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = target.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            target.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        target.transform.position = targetPosition; // Ensure the final position is set
    }

    private int GetPlayerIndex(GameObject player)
    {
        return GameManager.Instance.PlayerScriptList
            .FindIndex(p => p.gameObject == player);
    }

    private bool IsValidIndex(int index, List<GameObject> list)
    {
        return index >= 0 && index < list.Count;
    }
}