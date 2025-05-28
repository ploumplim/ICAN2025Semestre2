using UnityEngine;

public class PlayerSoundScript : MonoBehaviour
{
    private FMOD.Studio.EventInstance grabInstance;
    private bool isGrabbing = false;
    private float grabStartTime;

    private FMOD.Studio.EventInstance dashInstance;
    private bool isDashing = false;
    private float dashStartTime;

    void Start()
    {
        grabInstance = FMODUnity.RuntimeManager.CreateInstance(FMODEvents.instance.GrabPress_FX);
        dashInstance = FMODUnity.RuntimeManager.CreateInstance(FMODEvents.instance.Dash_FX);
    }

    public void PlayHitSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.Hit_Sound, this.transform.position);
    }

    public void BallHitByPlayerSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.BallTouched_FX, this.transform.position);
    }

    // --------- DASH SOUND ----------
    public void StartDashSound()
    {
        if (!isDashing)
        {
            isDashing = true;
            dashStartTime = Time.time;
            dashInstance.start();
        }
    }

    public void StopDashSound()
    {
        if (isDashing)
        {
            isDashing = false;
            dashInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
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
            grabStartTime = Time.time;
            grabInstance.start();
        }
    }

    public void StopGrabSound()
    {
        if (isGrabbing)
        {
            isGrabbing = false;
            grabInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            if (Time.time - grabStartTime > 0.1f)
            {
                PlayGrabOut();
            }
        }
    }

    void OnDestroy()
    {
        grabInstance.release();
        dashInstance.release();
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
