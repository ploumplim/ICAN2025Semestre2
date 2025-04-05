using UnityEngine;

public class PlayerSoundScript : MonoBehaviour
{
    private FMOD.Studio.EventInstance chargeInstance;
    private bool isCharging = false;

    void Start()
    {
        // Créer l'instance du son de charge
        chargeInstance = FMODUnity.RuntimeManager.CreateInstance(FMODEvents.instance.PressHit_FX);
    }

    public void PlayHitSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.Hit_Sound, this.transform.position);
    }

    public void StartChargeSound()
    {
        if (!isCharging)
        {
            isCharging = true;
            chargeInstance.start(); // Lancer le son de charge
        }
    }

    public void StopChargeSound()
    {
        if (isCharging)
        {
            isCharging = false;
            chargeInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); // Arrêter proprement
        }
    }

    void OnDestroy()
    {
        chargeInstance.release(); // Libérer l'instance
    }

    public void PlaySound2()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.BallBunt_Sound, this.transform.position);
    }

    public void PlaySound3()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.Bunt_FX, this.transform.position);
    }

    public void PlaySound5()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.BallTouched_FX, this.transform.position);
    }

    public void PlaySound6()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.Dash_FX, this.transform.position);
    }



}
