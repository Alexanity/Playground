using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public float amplitude = 0.5f;  // amount of movement in meters
    public float speed = 1f;  // speed of movement in meters per second
    public float rotationSpeed = 180f; // speed of rotation in degrees per second

    private Vector3 initialPosition;
   //public GameObject collectedParticlesPrefab;
    void Start()
    {
        initialPosition = transform.position;
    }
    void Update()
    {
        // Move the object up and down
        float yOffset = amplitude * Mathf.Sin(speed * Time.time);
        transform.position = initialPosition + new Vector3(0f, yOffset, 0f);

        // Rotate the object around the X-axis
        float xRotation = rotationSpeed * Time.deltaTime;
        transform.Rotate(xRotation, 0f, 0f);
    }
    void OnCollisonEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SpaceShip"))
        {
            //Instantiate(collectedParticlesPrefab, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
