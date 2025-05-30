using UnityEngine;

public class MultiplayerSound : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void PlayerSpawn()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.PlayerSpawn_FX, this.transform.position);
    }
}
