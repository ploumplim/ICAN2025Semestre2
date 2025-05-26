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
    public void PlayClassicBounce()
    {
        // Crée l'instance FMOD de l'événement 2D
        FMOD.Studio.EventInstance bounceInstance = FMODUnity.RuntimeManager.CreateInstance(FMODEvents.instance.ClassicBounce_FX);

        // Définit la valeur du paramètre "BallSpeed"
        bounceInstance.setParameterByName("BallSpeed", speedPercent);

        // Joue le son
        bounceInstance.start();

        // Libère l'instance pour que FMOD puisse le nettoyer après lecture
        bounceInstance.release();
    }

   




}
