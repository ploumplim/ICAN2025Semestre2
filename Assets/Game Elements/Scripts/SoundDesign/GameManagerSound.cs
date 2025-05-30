using UnityEngine;

public class GameManagerSound : MonoBehaviour
{
    private bool hasPlayedCountdown = false;

    public void PlayeCountdown()
    {
        if (!hasPlayedCountdown)
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.Countdown_FX, this.transform.position);
            hasPlayedCountdown = true;
        }
    }
}
