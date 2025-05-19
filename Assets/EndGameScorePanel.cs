using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EndGameScorePanel : MonoBehaviour
{
    public GameObject EndGameScorePanelGO;
    public GameObject PlayerScoreGO;
    public GameObject EndGamePlayerSpotPoint;
    public GameObject playerSpotGO;
    
    public int playerCount;
    public int winnerTimerSpot;
    public GameObject WinnerScoreGO;
    public GameObject WinnerCenterPoint;
    public PlayerScript playerTopScore;
    public float SizingUpSpotValue;
    public List<GameObject> playerSpotList = new List<GameObject>();
    public List<GameObject> playerScoreListGO = new List<GameObject>();
    public List<(GameObject player, int score)> playerScores = new List<(GameObject player, int score)>();
    
    //Moving Background
    public float rotatingBGSpeed;
    public GameObject BackGround;

    public void StartEndGamePanel()
    {
        playerCount = GameManager.Instance.PlayerScriptList.Count;
        ShowEndGameScorePanel();
        SetVisiblePlayerScores();
        UpdateScoreList(GameManager.Instance.PlayerScriptList);
        UpdateTopPlayer();
        MoveWinnerToCenter();
        ScaleUpWinnerSpot();
        StartCoroutine(StartWinnerTimer());
    }

    public void Update()
    {
        if (EndGameScorePanelGO != null && EndGameScorePanelGO.activeSelf)
        {
            BackGround.transform.Rotate(Vector3.forward, rotatingBGSpeed * Time.deltaTime);
        }
    }

    private void ShowEndGameScorePanel()
    {
        for (int i = 0; i < playerCount; i++)
        {
            var playerSpot = Instantiate(playerSpotGO, EndGamePlayerSpotPoint.transform.position, EndGamePlayerSpotPoint.transform.rotation);
            playerSpotList.Add(playerSpot);
            playerSpot.transform.SetParent(EndGamePlayerSpotPoint.transform);
        }
        EndGameScorePanelGO.SetActive(true);
        
        //TODO Rotate en continu EndGameScorePanel
    }

    private void SetVisiblePlayerScores()
    {
        for (int i = 0; i < playerCount; i++)
        {
            var playerScoreGO = Instantiate(PlayerScoreGO, playerSpotList[i].transform);
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
            WinnerScoreGO = IsValidIndex(topScorerIndex, playerScoreListGO) ? playerScoreListGO[topScorerIndex] : null;
        }
    }

    private void MoveWinnerToCenter()
    {
        if (TryGetWinnerIndex(out int winnerIndex))
        {
            WinnerScoreGO.transform.SetParent(WinnerCenterPoint.transform);
            WinnerScoreGO.transform.localPosition = Vector3.zero;
            WinnerScoreGO.transform.localRotation = Quaternion.identity;
        }
    }

    private void ScaleUpWinnerSpot()
    {
        if (TryGetWinnerIndex(out int winnerIndex))
        {
            var target = playerScoreListGO[winnerIndex];
            StartCoroutine(LerpScale(target, Vector3.one * SizingUpSpotValue, 1.0f));
        }
    }

    private IEnumerator LerpScale(GameObject target, Vector3 targetScale, float duration)
    {
        Vector3 startScale = target.transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            target.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        target.transform.localScale = targetScale;
        Debug.LogWarning("DebugTest");
    }

    private IEnumerator StartWinnerTimer()
    {
        yield return new WaitForSeconds(winnerTimerSpot);
        LerpWinnerToOriginalPosition();
    }

    private void LerpWinnerToOriginalPosition()
    {
        if (TryGetWinnerIndex(out int winnerIndex))
        {
            Vector3 targetPosition = playerSpotList[winnerIndex].transform.position;
            StartCoroutine(LerpPosition(WinnerScoreGO, targetPosition, 1.0f));
        }
    }

    private IEnumerator LerpPosition(GameObject target, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = target.transform.position;
        Vector3 startScale = target.transform.localScale;
        Vector3 targetScale = Vector3.one;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            target.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            target.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        target.transform.position = targetPosition;
        target.transform.localScale = targetScale;
        Debug.LogWarning("DebugTest2");
        
    }

    private int GetPlayerIndex(GameObject player)
    {
        return GameManager.Instance.PlayerScriptList.FindIndex(p => p.gameObject == player);
    }

    private bool IsValidIndex(int index, List<GameObject> list)
    {
        return index >= 0 && index < list.Count;
    }

    private bool TryGetWinnerIndex(out int winnerIndex)
    {
        winnerIndex = playerTopScore != null ? GetPlayerIndex(playerTopScore.gameObject) : -1;
        return IsValidIndex(winnerIndex, playerSpotList);
    }
}