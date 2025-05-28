using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class LevelSoundScript : MonoBehaviour
{
    private float currentPointPercent;
    private EventInstance evolutiveMusicInstance;

    void Start()
    {
        evolutiveMusicInstance = RuntimeManager.CreateInstance(FMODEvents.instance.EvolutivMusic_MSC);
        evolutiveMusicInstance.set3DAttributes(RuntimeUtils.To3DAttributes(this.transform));
        evolutiveMusicInstance.start();
    }

    public void PlayerSpawn()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.PlayerSpawn_FX, this.transform.position);
    }

    public void RecoverCurrentRound(int points)
    {
        float pointNeeded = GetComponent<LevelManager>().pointNeededToWin;
        currentPointPercent = (float)points / pointNeeded;

        evolutiveMusicInstance.setParameterByName("ScorePitch", currentPointPercent);
    }

    public void OnLevelChanged(int levelIndex)
    {
        // Réinitialise le pitch
        evolutiveMusicInstance.setParameterByName("ScorePitch", -2.50f);

        // Modifie immédiatement la couche musicale en fonction du niveau
        evolutiveMusicInstance.setParameterByName("MusicLayer", levelIndex);
    }

    private void OnDestroy()
    {
        evolutiveMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        evolutiveMusicInstance.release();
    }
}
