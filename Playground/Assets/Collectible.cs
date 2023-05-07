using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public static event Action onCollected;
    public static int total;
    public ParticleSystem particleSystemPrefab;
    public AudioClip audioClip;
    private bool hasPlayed = false;

    void Awake() => total++;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SpaceShip"))
        {
            Debug.Log("Collided");
            onCollected?.Invoke();
            if (particleSystemPrefab != null)
            {
                ParticleSystem ps = Instantiate(particleSystemPrefab, transform.position, Quaternion.identity);
                Destroy(ps.gameObject, ps.main.duration);
            }
            if (audioClip != null && !hasPlayed)
            {
                AudioSource.PlayClipAtPoint(audioClip, transform.position);
                hasPlayed = true;
            }
            Destroy(gameObject);
        }
    }
}
