using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipInteractionZone : MonoBehaviour
{
    [SerializeField]
    private ShipRigidBodyScript spaceship;

    private ZeroGMovement player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = other.gameObject.GetComponentInParent<ZeroGMovement> ();
            if(player != null)
            {
                player.ShipToEnter = spaceship;
            }

            print("Player is in the interaction zone");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (player != null)
            {
                player.ShipToEnter = null;
            }
            print("Player left interaction zone");
        }
    }
}
