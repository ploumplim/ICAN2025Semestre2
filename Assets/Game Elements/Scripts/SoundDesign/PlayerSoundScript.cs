using UnityEngine;

public class PlayerSoundScript : MonoBehaviour
{
    //private FMOD.Studio.EventInstance chargeInstance;
    //private bool isCharging = false;

    void Start()
    {
        // Cr�er l'instance du son de charge
        //chargeInstance = FMODUnity.RuntimeManager.CreateInstance(FMODEvents.instance.PressHit_FX);
    }

    public void PlayHitSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.Hit_Sound, this.transform.position);
    }

    //public void StartChargeSound()
    //{
    //    if (!isCharging)
    //    {
    //        isCharging = true;
    //        chargeInstance.start(); // Lancer le son de charge
    //    }
    //}

    //public void StopChargeSound()
    //{
    //    if (isCharging)
    //    {
    //        isCharging = false;
    //        chargeInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); // Arr�ter proprement
    //    }
    //}

    //void OnDestroy()
    //{
    //    chargeInstance.release(); // Lib�rer l'instance
    //}

    public void BallBuntSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.BallBunt_Sound, this.transform.position);
    }

    public void PlayBuntSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.Bunt_FX, this.transform.position);
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

    public void PlayGrabSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.Grab_FX, this.transform.position);

    }
}
