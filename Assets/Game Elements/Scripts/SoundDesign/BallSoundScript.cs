using UnityEngine;

public class BallSoundScript : MonoBehaviour
{
    private BallSM BallScript;
    private float speedPercent;

    private void OnEnable()
    {
        BallScript = GetComponent<BallSM>();

    }

    private void Update()
    {
        speedPercent = BallScript.rb.linearVelocity.magnitude / BallScript.maxSpeed;
    }
    public void PlaySound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.ClassicBounce_FX, this.transform.position);
    }
}
