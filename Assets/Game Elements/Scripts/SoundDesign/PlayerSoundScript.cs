using UnityEngine;

public class PlayerSoundScript : MonoBehaviour
{
    private FMOD.Studio.EventInstance grabInstance;
    private bool isGrabbing = false;

    //private FMOD.Studio.EventInstance chargeInstance;
    //private bool isCharging = false;

    void Start()
    {
        // Création de l’instance Grab comme pour Charge
        grabInstance = FMODUnity.RuntimeManager.CreateInstance(FMODEvents.instance.GrabPress_FX);

        // chargeInstance = FMODUnity.RuntimeManager.CreateInstance(FMODEvents.instance.PressHit_FX);
    }

    public void PlayHitSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.Hit_Sound, this.transform.position);
    }

    
    public void BallHitByPlayerSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.BallTouched_FX, this.transform.position);
    }

    public void DashSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.Dash_FX, this.transform.position);
    }

    public void PlayPressHitFX()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.PressHit_FX, this.transform.position);
    }

    public void PlayKnockOutSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.KnockOut_FX, this.transform.position);
    }

    public void PlayKnockBack()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.KnockBack_FX, this.transform.position);
    }

    // --------- GRAB SOUND ----------
    public void StartGrabSound()
    {
        if (!isGrabbing)
        {
            isGrabbing = true;
            grabInstance.start();
        }
    }

    public void StopGrabSound()
    {
        if (isGrabbing)
        {
            isGrabbing = false;
            grabInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    void OnDestroy()
    {
        grabInstance.release();

        // chargeInstance.release();
    }

    public void PlayGrabBall()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.GrabBall_FX, this.transform.position);
    }

    public void PlayGrabOut()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.GrabOut_FX, this.transform.position);
    }
}
