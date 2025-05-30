using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

public class LevelSoundScript : MonoBehaviour
{
    private float currentPointPercent;
    private EventInstance evolutiveMusicInstance;
    private float targetVolume = 1f; // Volume normal de la musique

    void Start()
    {
        StartEvolutiveMusic();
    }

    private void StartEvolutiveMusic()
    {
        evolutiveMusicInstance = RuntimeManager.CreateInstance(FMODEvents.instance.EvolutivMusic_MSC);
        evolutiveMusicInstance.set3DAttributes(RuntimeUtils.To3DAttributes(this.transform));
        evolutiveMusicInstance.start();
        evolutiveMusicInstance.setVolume(targetVolume); // Assure le volume de départ
    }

    public void PlayScoringBounce()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.ScoringBounce_FX, this.transform.position);
    }

    public void PlayPauseSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.PauseButton_UI, this.transform.position);
        StartCoroutine(FadeOutMusic(0.7f)); // Fade out en 1 secondes
    }

    public void PlayQuitPauseSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.QuitPauseButton_UI, this.transform.position);
        StartCoroutine(FadeInMusic(0.7f)); // Fade in en 1.5 secondes
    }

    private IEnumerator FadeOutMusic(float duration)
    {
        float currentVolume;
        evolutiveMusicInstance.getVolume(out currentVolume);

        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            float newVolume = Mathf.Lerp(currentVolume, 0f, timeElapsed / duration);
            evolutiveMusicInstance.setVolume(newVolume);
            timeElapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        evolutiveMusicInstance.setVolume(0f);
    }

    private IEnumerator FadeInMusic(float duration)
    {
        float currentVolume;
        evolutiveMusicInstance.getVolume(out currentVolume);

        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            float newVolume = Mathf.Lerp(currentVolume, targetVolume, timeElapsed / duration);
            evolutiveMusicInstance.setVolume(newVolume);
            timeElapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        evolutiveMusicInstance.setVolume(targetVolume);
    }

    public void RecoverCurrentRound(int points)
    {
        float pointNeeded = GetComponent<LevelManager>().pointNeededToWin;
        currentPointPercent = (float)points / pointNeeded;

        evolutiveMusicInstance.setParameterByName("ScorePitch", currentPointPercent);
    }

    public void OnLevelChanged()
    {
        // Réinitialise le pitch
        evolutiveMusicInstance.setParameterByName("ScorePitch", -2.5f);

        // Redémarre l'event musical pour que le Multi Instrument passe à la piste suivante (mode Sequential)
        evolutiveMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        evolutiveMusicInstance.release();

        StartEvolutiveMusic();
    }

    private void OnDestroy()
    {
        evolutiveMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        evolutiveMusicInstance.release();
    }
}
