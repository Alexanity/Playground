using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudio : MonoBehaviour
{
    private AudioSource audioSource;
    private bool hasPlayed = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Stop();
    }

    private void OnParticleCollision(GameObject other)
    {
        if (!hasPlayed)
        {
            audioSource.Play();
            hasPlayed = true;
        }
        else
        {
            hasPlayed = false;
        }
    }
}
