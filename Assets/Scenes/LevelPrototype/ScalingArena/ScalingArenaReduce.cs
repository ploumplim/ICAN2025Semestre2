using System.Collections;
using UnityEngine;

public class ScalingArenaReduce : MonoBehaviour
{
    [SerializeField] private float reductionSpeed = 0.1f; // Vitesse de réduction
    [SerializeField] private float duration = 5f; // Durée de réduction
    [SerializeField] private float waitTime = 5f; // Temps d'attente avant de relancer

    private void Start()
    {
        StartCoroutine(ScaleObject());
    }

    private IEnumerator ScaleObject()
    {
        while (true)
        {
            // Réduction de la taille pendant `duration` secondes
            float timer = 0f;
            while (timer < duration)
            {
                transform.localScale -= Vector3.one * (reductionSpeed * Time.deltaTime);
                timer += Time.deltaTime;
                yield return null;
            }

            // Attente de `waitTime` secondes
            yield return new WaitForSeconds(waitTime);
        }
    }
}