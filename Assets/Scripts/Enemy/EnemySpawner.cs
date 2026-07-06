using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnEntry
{
    public GameObject enemyPrefab;
    public int unlockLevel = 1;
    public int initialCount = 1;
    public int maxAliveCount = 10;
    public float spawnInterval = 5f;
    public int spawnCountPerInterval = 1;

    [HideInInspector] public bool isUnlocked;
    [HideInInspector] public float spawnTimer;
    [HideInInspector] public List<GameObject> spawnedEnemies = new();
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float spawnMargin = 1.5f;
    [SerializeField] private SpawnEntry[] spawnEntries;

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    public void SetLevel(int level)
    {
        foreach (SpawnEntry entry in spawnEntries)
        {
            if (entry.isUnlocked) continue;
            if (level < entry.unlockLevel) continue;

            entry.isUnlocked = true;
            entry.spawnTimer = entry.spawnInterval;

            for (int i = 0; i < entry.initialCount; i++)
                SpawnOne(entry);
        }
    }

    private void SpawnOne(SpawnEntry entry)
    {
        if (entry.enemyPrefab == null) return;
        if (targetCamera == null) return;

        Vector2 spawnPosition = GetSpawnPositionOutsideCamera();

        GameObject enemy = Instantiate(
            entry.enemyPrefab,
            spawnPosition,
            Quaternion.identity
        );

        entry.spawnedEnemies.Add(enemy);
    }

    private Vector2 GetSpawnPositionOutsideCamera()
    {
        float halfHeight = targetCamera.orthographicSize;
        float halfWidth = halfHeight * targetCamera.aspect;

        Vector2 center = targetCamera.transform.position;

        int side = Random.Range(0, 4);

        switch (side)
        {
            case 0: // Left
                return new Vector2(
                    center.x - halfWidth - spawnMargin,
                    Random.Range(center.y - halfHeight, center.y + halfHeight)
                );

            case 1: // Right
                return new Vector2(
                    center.x + halfWidth + spawnMargin,
                    Random.Range(center.y - halfHeight, center.y + halfHeight)
                );

            case 2: // Top
                return new Vector2(
                    Random.Range(center.x - halfWidth, center.x + halfWidth),
                    center.y + halfHeight + spawnMargin
                );

            default: // Bottom
                return new Vector2(
                    Random.Range(center.x - halfWidth, center.x + halfWidth),
                    center.y - halfHeight - spawnMargin
                );
        }
    }

    void Update()
    {
        foreach (SpawnEntry entry in spawnEntries)
        {
            if (!entry.isUnlocked) continue;

            RemoveDestroyedEnemies(entry);

            if (entry.spawnedEnemies.Count >= entry.maxAliveCount)
                continue;

            entry.spawnTimer -= Time.deltaTime;
            if (entry.spawnTimer > 0f) continue;

            int spawnableCount = entry.maxAliveCount - entry.spawnedEnemies.Count;
            int spawnCount = Mathf.Min(entry.spawnCountPerInterval, spawnableCount);

            for (int i = 0; i < spawnCount; i++)
                SpawnOne(entry);

            entry.spawnTimer = entry.spawnInterval;
        }
    }

    private void RemoveDestroyedEnemies(SpawnEntry entry)
    {
        entry.spawnedEnemies.RemoveAll(enemy => enemy == null);
    }
}
