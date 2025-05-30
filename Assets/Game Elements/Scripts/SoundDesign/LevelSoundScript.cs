using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class LevelSoundScript : MonoBehaviour
{
    private float currentPointPercent;
    private EventInstance evolutiveMusicInstance;

    void Start()
    {
        StartEvolutiveMusic();
    }

    private void StartEvolutiveMusic()
    {
        evolutiveMusicInstance = RuntimeManager.CreateInstance(FMODEvents.instance.EvolutivMusic_MSC);
        evolutiveMusicInstance.set3DAttributes(RuntimeUtils.To3DAttributes(this.transform));
        evolutiveMusicInstance.start();
    }

   

    public void PlayScoringBounce()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.ScoringBounce_FX, this.transform.position);

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
