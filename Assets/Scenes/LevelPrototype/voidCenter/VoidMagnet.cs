using System.Collections.Generic;
using UnityEngine;

public class VoidMagnet : MonoBehaviour
{
    [SerializeField] private float attractionForce = 10f; // Force d'attraction

    void FixedUpdate()
    {
        // Vérifiez si la liste des joueurs est disponible
        List<PlayerScript> playerList = GameManager.Instance.PlayerScriptList;
        if (playerList != null)
        {
            foreach (PlayerScript player in playerList)
            {
                // Vérifiez si le joueur a un GameObject et un Rigidbody
                GameObject playerObject = player.gameObject;
                if (playerObject != null)
                {
                    Rigidbody playerRigidbody = playerObject.GetComponent<Rigidbody>();
                    if (playerRigidbody != null)
                    {
                        // Calcul de la direction vers l'objet
                        Vector3 direction = (transform.position - playerObject.transform.position).normalized;

                        // Application de la force d'attraction
                        playerRigidbody.AddForce(direction * attractionForce);
                    }
                }
            }
        }
    }
}