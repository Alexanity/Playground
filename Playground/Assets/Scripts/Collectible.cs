using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Collectible : MonoBehaviour
{
    public static event Action onCollected;
    public static int total;
    public ParticleSystem particleSystemPrefab;
    public AudioClip audioClip;
    private bool hasPlayed = false;
    private static int coinsCollected = 0;
    private int totalCoins;

    void Awake()
    {
        total++;
        totalCoins = GameObject.FindGameObjectsWithTag("Coin").Length;
        Debug.Log($"Number of coins:{totalCoins}");
        Debug.Log($"Coins collected at start: {coinsCollected}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SpaceShip"))
        {
            Debug.Log("Collided");
            onCollected?.Invoke();
            coinsCollected++;

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
            Debug.Log($"Coins collected: {coinsCollected}");
            if (coinsCollected == totalCoins)
            {
                SceneManager.LoadScene("EndGame");
            }
        }
    }
}
