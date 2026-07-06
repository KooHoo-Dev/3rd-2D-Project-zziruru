using UnityEngine;

public class MonitorProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private float spinSpeed = 720f;

    private Vector2 targetPosition;
    private int damage;
    private float explosionRadius;

    public void Initialize(Vector2 targetPosition, int damage, float explosionRadius)
    {
        this.targetPosition = targetPosition;
        this.damage = damage;
        this.explosionRadius = explosionRadius;

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPosition) <= 0.05f)
            Explode();
    }

    private void Explode()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        foreach (Enemy enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance > explosionRadius) continue;

            Vector2 hitDirection = enemy.transform.position - transform.position;
            enemy.TakeDamage(damage, hitDirection);
        }

        Destroy(gameObject);
    }
}
