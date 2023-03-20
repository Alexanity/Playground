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
            print("Player is in the interaction zone");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            print("Player left interaction zone");
        }
    }
}
