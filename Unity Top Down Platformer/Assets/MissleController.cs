using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissleController : MonoBehaviour
{
    public Enemy enemy = null;
    private float speed = 15.0f;
    private float strength = 15.0f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (enemy != null)
        {
            float step = speed * Time.deltaTime;
            Vector3 enemyPosition = enemy.transform.position - transform.position;
            transform.position = Vector3.MoveTowards(transform.position, enemy.transform.position, step);
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, enemyPosition, Time.deltaTime * speed, 0.0f);

            transform.rotation = Quaternion.LookRotation(newDirection);
            if (transform.position.y < -10) {
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Enemy")) {

            Rigidbody enemyRb = other.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromMissle = (other.gameObject.transform.position - transform.position).normalized;

            enemyRb.AddForce(awayFromMissle * strength, ForceMode.Impulse);
            Destroy(gameObject);
        }
    }
}
