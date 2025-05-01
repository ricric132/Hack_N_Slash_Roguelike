using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public GameObject enemyType1;        // First enemy type prefab
    public GameObject enemyType2;  
    public GameObject miniboss;       // Second enemy type prefab
    public int maxNumberOfSpawns = 6;   // Maximum number of spawns
    public float spawnRadius = 15f;      // Radius around the player for spawning
    public float minSpawnInterval = 1f;  // Minimum time interval between spawns
    public float maxSpawnInterval = 4f;  // Maximum time interval between spawns

    private Transform playerTransform;   // Reference to the player's transform
    private int spawnedCount = 0;
    public Transform BossBattleLocation;
    private bool minibossSpawned = false;
    public bool minibossDefeated = false;
    private bool SecondRoundBegin = false;
    private bool SecondRoundDefeated = false;
    public GameObject player;
    public MiniBossStatus miniBossStatus;
    public bool MinibossKilled = false;
    public string targetTag = "Enemy";

    public GameObject bird;
    public GameObject bossHP;

    public void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform; // Find player's transform
        StartCoroutine(SpawnObjectsWithRandomIntervals());
        player = GameObject.FindGameObjectWithTag("Player");
        bird.SetActive(false);
        bossHP.SetActive(false);
    }

    private IEnumerator SpawnObjectsWithRandomIntervals()
    {

        while (spawnedCount < maxNumberOfSpawns)
        {
            float randomInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(randomInterval);

            Vector3 randomOffset = Random.insideUnitSphere * spawnRadius; // Random offset within the spawn radius
            Vector3 spawnPosition = playerTransform.position + randomOffset; // Calculate spawn position

            // Ensure the Y position is at ground level (adjust this based on your terrain setup)
            spawnPosition.y = player.transform.position.y;

            // Spawn the object at the calculated position
            Instantiate(GetEnemyPrefab(), spawnPosition, Quaternion.identity);

            spawnedCount++;
        }
        if(SecondRoundBegin)
        {
            SecondRoundDefeated = true;
        }
    }
    public void Update()
    {
        GameObject[] entities = GameObject.FindGameObjectsWithTag(targetTag);
        int entityCount = entities.Length;

        if(spawnedCount >= maxNumberOfSpawns && !minibossSpawned && entityCount <= 0)
        {
            Vector3 randomOffset = Random.insideUnitSphere * spawnRadius; // Random offset within the spawn radius
            Vector3 spawnPosition = playerTransform.position + randomOffset; // Calculate spawn position

            // Ensure the Y position is at ground level (adjust this based on your terrain setup)
            spawnPosition.y = player.transform.position.y;
            Instantiate(miniboss, spawnPosition, Quaternion.identity);
            minibossSpawned = true;
        }

        if(MinibossKilled && !SecondRoundBegin)
        {
            spawnedCount = 0;
            SecondRoundBegin = true;
            StartCoroutine(SpawnObjectsWithRandomIntervals());
        }

        if(entityCount <= 0 && SecondRoundDefeated)
        {
            bird.SetActive(true);
            bossHP.SetActive(true);
        }
        
    }

    private GameObject GetEnemyPrefab()
    {
        // Randomly select between enemyType1 and enemyType2
        return Random.Range(0, 2) == 0 ? enemyType1 : enemyType2;
    }
}