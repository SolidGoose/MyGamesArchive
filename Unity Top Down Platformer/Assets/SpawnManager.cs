using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public int waveNumber = 1;
    public float minionSpawnInterval = 5.0f;
    public GameObject[] enemyPrefabs;
    public GameObject[] megaBossPrefabs;
    public GameObject[] powerupPrefabs;
    private int enemyCount;
    private float spawnRange = 9;
    private GameObject randomEnemy;
    private GameObject randomPowerup;
    // Start is called before the first frame update
    void Start()
    {
        SpawnEnemyWave(waveNumber, false);
        SpawnPowerup();
    }

    // Update is called once per frame
    void Update()
    {
        enemyCount = FindObjectsOfType<Enemy>().Length;
        if (enemyCount == 0) {
            var spawnMegaBoss = ++waveNumber % 3 == 0;
            SpawnEnemyWave(waveNumber, spawnMegaBoss);
            SpawnPowerup();
        }
    }

    void SpawnRandomEnemy()
    {
        randomEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Instantiate(randomEnemy, GenerateSpawnPosition(), randomEnemy.transform.rotation);
    }

    void SpawnEnemyWave(int enemiesToSpawn, bool spawnMegaBoss)
    {
        Debug.Log("Wave N" + enemiesToSpawn);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            if (spawnMegaBoss & i + 1 == enemiesToSpawn)
            {
                Instantiate(megaBossPrefabs[0], GenerateSpawnPosition(), randomEnemy.transform.rotation);
                StartCoroutine(SpawnMegaBossMinionsRoutine());
            }
            else
            {
                SpawnRandomEnemy();
            }
        }
    }

    IEnumerator SpawnMegaBossMinionsRoutine()
    {
        // Spawn a minion each 5 seconds until the MegaBoss is destroyed
        GameObject megaBoss = GameObject.Find("Enemy(Clone)");
        while (megaBoss)
        {
            SpawnRandomEnemy();
            yield return new WaitForSeconds(minionSpawnInterval);
        }
    }

    void SpawnPowerup()
    {
        randomPowerup = powerupPrefabs[Random.Range(0, powerupPrefabs.Length)];
        Instantiate(randomPowerup, GenerateSpawnPosition(), randomPowerup.transform.rotation);
    }

    public Vector3 GenerateSpawnPosition() {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = Random.Range(-spawnRange, spawnRange);
        Vector3 randomPos = new Vector3(spawnPosX, 0, spawnPosZ);
        return randomPos;
    }
}
