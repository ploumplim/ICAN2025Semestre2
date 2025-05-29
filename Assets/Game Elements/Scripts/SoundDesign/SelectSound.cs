using UnityEngine;
using FMODUnity;

public class SelectSound : MonoBehaviour
{
    
    public string fmodEvent;

    public void PlaySound()
    {
        //AudioManager.instance.PlayOneShot(FMODEvents.instance.Select_UI, this.transform.position);
        RuntimeManager.PlayOneShot(fmodEvent);

    }
}
