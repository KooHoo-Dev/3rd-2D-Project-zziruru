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
    private enum SpawnSide
    {
        Left,
        Right,
        Top,
        Bottom
    }

    [SerializeField] private Camera targetCamera;
    [SerializeField] private float spawnMargin = 1.5f;
    [SerializeField] private SpawnEntry[] spawnEntries;

    [SerializeField] private LayerMask collisionLayer;
    [SerializeField] private BoxCollider2D mapBounds;
    [SerializeField] private float spawnCheckRadius = 0.35f;
    [SerializeField] private int maxSpawnAttempts = 100;

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    public void SetMapBounds(BoxCollider2D newMapBounds)
    {
        mapBounds = newMapBounds;
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

        if (!TryGetSpawnPosition(out Vector2 spawnPosition)) return;

        GameObject enemy = Instantiate(
            entry.enemyPrefab,
            spawnPosition,
            Quaternion.identity
        );

        entry.spawnedEnemies.Add(enemy);
    }

    private bool TryGetSpawnPosition(out Vector2 spawnPosition)
    {
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            if (!TryGetSpawnPositionOutsideCamera(out Vector2 candidate))
                break;

            bool isBlocked = Physics2D.OverlapCircle(
                candidate,
                spawnCheckRadius,
                collisionLayer
            ) != null;

            if (!isBlocked)
            {
                spawnPosition = candidate;
                return true;
            }
        }

        spawnPosition = Vector2.zero;
        return false;
    }

    private bool TryGetSpawnPositionOutsideCamera(out Vector2 spawnPosition)
    {
        float halfHeight = targetCamera.orthographicSize;
        float halfWidth = halfHeight * targetCamera.aspect;

        Vector2 center = targetCamera.transform.position;
        Bounds bounds = mapBounds.bounds;

        float minX = bounds.min.x + spawnCheckRadius;
        float maxX = bounds.max.x - spawnCheckRadius;
        float minY = bounds.min.y + spawnCheckRadius;
        float maxY = bounds.max.y - spawnCheckRadius;

        float cameraLeft = center.x - halfWidth;
        float cameraRight = center.x + halfWidth;
        float cameraBottom = center.y - halfHeight;
        float cameraTop = center.y + halfHeight;

        List<SpawnSide> validSides = new();

        if (cameraLeft - spawnMargin >= minX)
            validSides.Add(SpawnSide.Left);

        if (cameraRight + spawnMargin <= maxX)
            validSides.Add(SpawnSide.Right);

        if (cameraTop + spawnMargin <= maxY)
            validSides.Add(SpawnSide.Top);

        if (cameraBottom - spawnMargin >= minY)
            validSides.Add(SpawnSide.Bottom);

        if (validSides.Count == 0)
        {
            spawnPosition = Vector2.zero;
            return false;
        }

        SpawnSide side = validSides[Random.Range(0, validSides.Count)];

        switch (side)
        {
            case SpawnSide.Left:
                spawnPosition = new Vector2(
                    cameraLeft - spawnMargin,
                    Random.Range(
                        Mathf.Max(cameraBottom, minY),
                        Mathf.Min(cameraTop, maxY)
                    )
                );
                return true;

            case SpawnSide.Right:
                spawnPosition = new Vector2(
                    cameraRight + spawnMargin,
                    Random.Range(
                        Mathf.Max(cameraBottom, minY),
                        Mathf.Min(cameraTop, maxY)
                    )
                );
                return true;

            case SpawnSide.Top:
                spawnPosition = new Vector2(
                    Random.Range(
                        Mathf.Max(cameraLeft, minX),
                        Mathf.Min(cameraRight, maxX)
                    ),
                    cameraTop + spawnMargin
                );
                return true;

            default:
                spawnPosition = new Vector2(
                    Random.Range(
                        Mathf.Max(cameraLeft, minX),
                        Mathf.Min(cameraRight, maxX)
                    ),
                    cameraBottom - spawnMargin
                );
                return true;
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
