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
        // Cr�e l'instance FMOD de l'�v�nement 2D
        FMOD.Studio.EventInstance bounceInstance = FMODUnity.RuntimeManager.CreateInstance(FMODEvents.instance.ClassicBounce_FX);

        // D�finit la valeur du param�tre "BallSpeed"
        bounceInstance.setParameterByName("BallSpeed", speedPercent);

        // Joue le son
        bounceInstance.start();

        // Lib�re l'instance pour que FMOD puisse le nettoyer apr�s lecture
        bounceInstance.release();
    }

   




}
