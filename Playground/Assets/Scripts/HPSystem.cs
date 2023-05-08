using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HPSystem : MonoBehaviour
{
    [Header("=== Ship health ===")]
    public int maxHealth = 100;
    public int currentHealth;
    [Header("=== Particle ===")]
    [SerializeField]
    ParticleSystem collisionParticle = null;
    bool hasCollided = false;
    [Header("=== Respawn ===")]
    public Transform respawnPoint;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private int numRespawns = 0;
    [Header("=== Sound ===")]
    public AudioClip explosionSound;
    private AudioSource audioSource;

    private void Start()
    {
        currentHealth = maxHealth;
        collisionParticle.Pause();
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }
    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Respawn();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Asteroids"))
        {
            TakeDamage(20);
            if (collisionParticle != null)
            {
                ParticleSystem explosion = Instantiate(collisionParticle, transform.position, Quaternion.identity);
                collisionParticle.Play();
                FindObjectOfType<AudioManager>().Play("explosion");
            }
            
        }
    }
    void Respawn()
    {
        numRespawns++;
        if (numRespawns >= 3)
        {
            SceneManager.LoadScene("PlayerDied");
        }
        else
        {
            transform.position = originalPosition;
            transform.rotation = originalRotation;
            currentHealth = maxHealth;
        }
    }
}
