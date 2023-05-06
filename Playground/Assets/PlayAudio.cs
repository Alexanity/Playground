using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudio : MonoBehaviour
{
    public float cooldownTime = 5f;
    private AudioSource audioSource;
    private bool hasPlayed = false;
    private float cooldownTimer;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (hasPlayed)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= cooldownTime)
            {
                hasPlayed = false;
                cooldownTimer = 0f;
            }
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (!hasPlayed)
        {
            audioSource.Play();
            hasPlayed = true;
        }
    }
}
