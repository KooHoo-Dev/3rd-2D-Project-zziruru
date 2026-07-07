using UnityEngine;

public class MapTransitionTrigger : MonoBehaviour
{
    [SerializeField] private GameObject currentMap;
    [SerializeField] private GameObject nextMap;
    [SerializeField] private Transform nextSpawnPoint;
    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] private BoxCollider2D nextMapBounds;
    [SerializeField] private EnemySpawner enemySpawner;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player == null) return;

        currentMap.SetActive(false);
        nextMap.SetActive(true);

        Debug.Log($"SpawnPoint position: {nextSpawnPoint.position}");

        player.TeleportTo(nextSpawnPoint.position);

        Debug.Log($"Player after teleport: {player.transform.position}");

        if (cameraFollow != null)
        {
            cameraFollow.SetMapBounds(nextMapBounds);
            cameraFollow.SnapToTarget();
        }

        if (enemySpawner != null)
            enemySpawner.SetMapBounds(nextMapBounds);
    }
}
