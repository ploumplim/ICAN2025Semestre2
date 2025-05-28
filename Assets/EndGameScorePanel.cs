using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class EndGameScorePanel : MonoBehaviour
{
    public GameObject EndGameScorePanelGO;

    public Dictionary<PlayerScript, (int, GameObject)> playerData;

    // List related to player spot positions
    public List<GameObject> playerSpotPositions;
    public GameObject playerSpotPositionPrefab;
    public GameObject playerSpotPositionParent;
    
    //GO related to the winning player spot
    public GameObject winningPlayerSpot;
    
    //Prefab relate for the players
    public GameObject playerIconPrefab;
    
    //WinningPlayerIcon
    private GameObject winningPlayerIcon;
    
    
    public void StartEndGamePanel()
    {
        playerData = GameManager.Instance.PlayerScriptList
            .ToDictionary(player => player, player => (player.playerPoint, (GameObject)null));

        ShowEndGameScorePanel();
        setPlayerSpotPositions();
        SpawnWinningPlayerIcon();
        
    }

    private void ShowEndGameScorePanel()
    {
        EndGameScorePanelGO.SetActive(true);
    }

    public void setPlayerSpotPositions()
    {
        foreach (PlayerScript playerScript in GameManager.Instance.PlayerScriptList)
        {
            GameObject playerSpotPosition = Instantiate(playerSpotPositionPrefab, playerSpotPositionParent.transform);
            playerSpotPositions.Add(playerSpotPosition);
        }
        
    }
    public void SpawnWinningPlayerIcon()
    {
        winningPlayerIcon = Instantiate(playerIconPrefab, winningPlayerSpot.transform);
        
        
        
        foreach (TextMeshProUGUI textPlayericon in winningPlayerIcon.transform.GetComponentsInChildren<TextMeshProUGUI>())
        {
            if (textPlayericon.gameObject.name == "PlayerName")
            {
                textPlayericon.text = GameManager.Instance.levelManager.winningPlayer.name;
            }

            if (textPlayericon.gameObject.name == "Score")
            {
                textPlayericon.text = GameManager.Instance.levelManager.winningPlayer.playerGlobalPoint.ToString();
            }
                    
                    
        }
        
        
        // var textComponent = winningPlayerIcon.GetComponentInChildren<TextMeshProUGUI>();
        // if (textComponent != null)
        // {
        //     textComponent.text = GameManager.Instance.levelManager.winningPlayer.name;
        // }
        
        
        RectTransform rectTransform = winningPlayerIcon.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(40f, 40f); // Par exemple, largeur 40 et hauteur 40
        winningPlayerIconLerping();
        
    }

    public void winningPlayerIconLerping()
    {
        // Lerp vers la grande taille au centre du spot gagnant
        TimerWaitForSeconds(2f, () => StartCoroutine(ScalingLerp(40f, 300f, 4f, winningPlayerSpot.GetComponent<RectTransform>().anchoredPosition)));

        var winningPlayer = GameManager.Instance.levelManager.winningPlayer;
        if (playerData.ContainsKey(winningPlayer))
        {
            var currentData = playerData[winningPlayer];
            playerData[winningPlayer] = (currentData.Item1, winningPlayerIcon);
        }
        int index = playerData.Keys.ToList().FindIndex(p => p == winningPlayer);
        
        //TODO : Lerp la position du winningPlayerIcon vers le spot du joueur gagnant
        
        // //Conversion de la position monde du spot cible en coordonnées locales du canvas
        // Vector2 localPoint;
        //
        //  RectTransform parentRect = winningPlayerIcon.transform.parent.GetComponent<RectTransform>();
        //  Vector3 worldPos = playerSpotPositions[index].transform.position;
        //  RectTransformUtility.ScreenPointToLocalPointInRectangle(
        //      parentRect,
        //      Camera.main.WorldToScreenPoint(worldPos),
        //      null,
        //      out localPoint
        //  );
        //
        //  // Lerp vers la position du spot du joueur gagnant
        //  TimerWaitForSeconds(2f, () => StartCoroutine(ScalingLerp(300f, 100f, 4f, localPoint)));
        
        
    }
    
    public void SpawnPlayerIcons()
    {
        foreach (var kvp in playerData)
        {
            if (kvp.Value.Item2 == null)
            {
                GameObject playerIcon = Instantiate(playerIconPrefab, playerSpotPositions[playerData.Keys.ToList().IndexOf(kvp.Key)].transform);

                foreach (TextMeshProUGUI textPlayericon in playerIcon.transform.GetComponentsInChildren<TextMeshProUGUI>())
                {
                    if (textPlayericon.gameObject.name == "PlayerName")
                    {
                        textPlayericon.text = kvp.Key.name;
                    }

                    if (textPlayericon.gameObject.name == "Score")
                    {
                        textPlayericon.text = kvp.Value.Item1.ToString();
                    }
                    
                    
                }
            }
        }
                
                // foreach (GameObject playerIconChild in playerIcon.transform)
                // {
                //     if (playerIconChild.name == "Score")
                //     {
                //         var textComponent = playerIcon.GetComponentInChildren<TextMeshProUGUI>();
                //         if (textComponent != null)
                //         {
                //             textComponent.text = kvp.Key.name;
                //         }
                //     }
                // }
            
        
    }
    
    public void TimerWaitForSeconds(float seconds, Action callback)
    {
        StartCoroutine(TimerCoroutine(seconds, callback));
    }

    private IEnumerator TimerCoroutine(float seconds, Action callback)
    {
        yield return new WaitForSeconds(seconds);
        callback?.Invoke();
    }
    
    private IEnumerator ScalingLerp(float startSize, float endSize, float duration, Vector3 targetPosition)
    {
        yield return LerpWinningPlayerIconSizeAndPosition(startSize, endSize, duration, targetPosition);
        yield return new WaitForSeconds(2f);
        
        yield return LerpWinningPlayerIconSizeAndPosition(endSize, 100, duration, targetPosition);
        yield return new WaitForSeconds(2f);
        SpawnPlayerIcons();
        // Debug.Log("Début de la prochaine étape après le lerp et 2 secondes d'attente.");
    }

    private IEnumerator LerpWinningPlayerIconSizeAndPosition(float startSize, float endSize, float duration, Vector3 targetPosition)
    {
        RectTransform rectTransform = winningPlayerIcon.GetComponent<RectTransform>();
        float elapsed = 0f;
        Vector3 startPosition = rectTransform.anchoredPosition;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float size = Mathf.Lerp(startSize, endSize, t);
            rectTransform.sizeDelta = new Vector2(size, size);
            rectTransform.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rectTransform.sizeDelta = new Vector2(endSize, endSize);
        rectTransform.anchoredPosition = targetPosition;
        
    }
   
}