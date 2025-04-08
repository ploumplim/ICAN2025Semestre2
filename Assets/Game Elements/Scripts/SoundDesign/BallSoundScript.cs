using UnityEngine;

public class BallSoundScript : MonoBehaviour
{
   
    
    
    public void PlaySound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.ClassicBounce_FX, this.transform.position);
    }
}
