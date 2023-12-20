using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject powerupIndicator;
    public GameObject missle;
    public float speed = 5.0f;

    private GameObject focalPoint;
    private Rigidbody playerRb;
    private bool isPowerup;
    private float forceFieldStrength = 15.0f;
    private float groundPoundStrength = 15.0f;
    private float groundPoundSpeed = 20.0f;
    private float groundPoundRadius = 10.0f;
    private string hasPowerup = null;
    private string[] powerupTags = {"ForceField", "RocketBlast", "GroundPound"};
    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
    }

    // Update is called once per frame
    void Update()
    {
        // Move player vertically
        float forwardInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * speed * forwardInput);
        powerupIndicator.transform.position = transform.position + new Vector3(0, -0.5f, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        isPowerup = powerupTags.Contains(other.gameObject.tag);
        Debug.Log("isPowerup: " + isPowerup);
        if (isPowerup && hasPowerup == null) {
            hasPowerup = other.gameObject.tag;
            Destroy(other.gameObject);
            StartCoroutine(PowerupCountdownRoutine());
            powerupIndicator.gameObject.SetActive(true);
            switch (hasPowerup)
            {
                case "RocketBlast":
                    StartCoroutine(RocketBlastRoutine());
                    break;
                case "GroundPound":
                    StartCoroutine(GroundPoundRoutine());
                    break;
            }
        }
    }

    IEnumerator PowerupCountdownRoutine()
    {
        // Deactivate powerup after some time
        yield return new WaitForSeconds(7);
        hasPowerup = null;
        powerupIndicator.gameObject.SetActive(false);
    }

    IEnumerator RocketBlastRoutine() {
        while (hasPowerup != null)
        {
            // Set a postion in front of the player for the rockets to spawn
            Vector3 spawnPosition = transform.position + playerRb.velocity.normalized * 2;
            Enemy[] enemies = FindObjectsOfType<Enemy>();
            for (int i = 0; i < enemies.Length; i++) {
                // Spawn a rocket
                GameObject newMissle = Instantiate(missle, spawnPosition, transform.rotation);
                // Assign it to an Enemy
                newMissle.GetComponent<MissleController>().enemy = enemies[i];
            }
            yield return new WaitForSeconds(1.5f);
        }
    }

    IEnumerator GroundPoundRoutine() {        
        while (hasPowerup != null)
        {
            if (transform.position.y > 0)
            {
                Enemy[] enemies = FindObjectsOfType<Enemy>();
                // Turn off the collisions with enemies
                for (int i = 0; i < enemies.Length; i++) {
                    Physics.IgnoreCollision(enemies[i].gameObject.GetComponent<Collider>(), GetComponent<Collider>(), true);
                }
                // Raise player up and wait a bit
                playerRb.velocity += Vector3.up * groundPoundSpeed;
                yield return new WaitForSeconds(0.5f);
                // Throw player down and wait a bit
                playerRb.velocity += Vector3.down * groundPoundSpeed * 2;
                yield return new WaitForSeconds(0.4f);
                PushEnemiesNearby();
                // Turn on the collisions with enemies back
                for (int i = 0; i < enemies.Length; i++) {
                    if (enemies[i])
                    {
                        Physics.IgnoreCollision(enemies[i].gameObject.GetComponent<Collider>(), GetComponent<Collider>(), false);
                    }
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    void PushEnemiesNearby() {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        for (int i = 0; i < enemies.Length; i++) {
            Vector3 enemyPosition = enemies[i].transform.position;
            // Calculate the distance from an Enemy to the player
            float distance = Vector3.Distance(transform.position, enemyPosition);
            // If an Enemy is in distance of the powerup
            if (distance < groundPoundRadius)
            {
                Rigidbody enemyRb = enemies[i].gameObject.GetComponent<Rigidbody>();
                // Calculate a ratio to make the pound less effective on the distant enemies
                float ratio = 1 - distance / groundPoundRadius;
                // Apply the ration to get the final throwback strength
                float finalStrength = groundPoundStrength * ratio;
                Vector3 awayFromPlayer = enemyPosition - transform.position;
                // Throw an Enemy away
                enemyRb.AddForce(awayFromPlayer * finalStrength, ForceMode.Impulse);
            }
        }
    }

    private void OnCollisionEnter(Collision other) {
        // If collistion happens with an Enemy
        if (other.gameObject.CompareTag("Enemy")) {
            Debug.Log("Player collided with " + other.gameObject.name + " with powerup set to " + hasPowerup);

            // And player has a Force Field
            if (hasPowerup == "ForceField")
            {
                Rigidbody enemyRb = other.gameObject.GetComponent<Rigidbody>();
                Vector3 awayFromPlayer = other.gameObject.transform.position - transform.position;

                // Throw an Enemy away
                enemyRb.AddForce(awayFromPlayer * forceFieldStrength, ForceMode.Impulse);
            }
        }
    }
}
