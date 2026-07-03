using UnityEngine;

public class Enemy03Spawner : MonoBehaviour
{
    [SerializeField] private int initialSpawnCount = 3;
    [SerializeField] private int maxAliveCount = 20;
    [SerializeField] private GameObject enemy03Prefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnInterval = 5f;

    private float spawnTimer;

    private void Start()
    {
        for (int i = 0; i < initialSpawnCount; i++)
            SpawnOne();

        spawnTimer = spawnInterval;
    }

    private void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer > 0f) return;

        if (CountAliveEnemy() >= maxAliveCount) return;

        spawnTimer = spawnInterval;
        SpawnOne();
    }

    private void SpawnOne()
    {
        if (enemy03Prefab == null) return;
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        Instantiate(enemy03Prefab, spawnPoint.position, Quaternion.identity);
    }

    private int CountAliveEnemy()
    {
        Enemy03Chase[] enemies =
            FindObjectsByType<Enemy03Chase>(FindObjectsSortMode.None);

        return enemies.Length;
    }
}