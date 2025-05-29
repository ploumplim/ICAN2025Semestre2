using UnityEngine;
using FMODUnity;

public class MenuMusicController : MonoBehaviour
{
    public StudioEventEmitter menuEmitter;

    public void StopMenuMusic()
    {
        if (menuEmitter != null)
        {
            menuEmitter.Stop(); // Ou Stop(STOP_MODE.ALLOWFADEOUT) pour un fondu
        }
    }
}
