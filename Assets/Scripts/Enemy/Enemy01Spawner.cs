using UnityEngine;

public class Enemy01Spawner : MonoBehaviour
{
    [SerializeField] private int initialSpawnCount = 6;
    [SerializeField] private int maxAliveCount = 100;

    [SerializeField] private GameObject enemy01Prefab;
    [SerializeField] private Transform[] spawnPoints;

    [SerializeField] private float spawnInterval = 3f;

    private float spawnTimer;

    private void Start()
    {
        for (int i = 0; i < initialSpawnCount; i++)
            SpawnOne();

        spawnTimer = spawnInterval;
    }

    private void Update()
    {
        if (Enemy01Manager.Instance == null) return;
        if (!Enemy01Manager.Instance.IsPanic) return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer > 0f) return;
        spawnTimer = spawnInterval;

        if (CountAliveEnemy() >= maxAliveCount) return;
        SpawnOne();
    }

    private void SpawnOne()
    {
        if (enemy01Prefab == null) return;
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        Instantiate(enemy01Prefab, spawnPoint.position, Quaternion.identity);
    }

    private int CountAliveEnemy()
    {
        Enemy01Movement[] enemies =
            FindObjectsByType<Enemy01Movement>(FindObjectsSortMode.None);

        return enemies.Length;
    }
}
