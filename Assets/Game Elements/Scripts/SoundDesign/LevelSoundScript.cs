using UnityEngine;

public class LevelSoundScript : MonoBehaviour
{
    public void PlayerSpawn()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.PlayerSpawn_FX, this.transform.position);
    }
}
