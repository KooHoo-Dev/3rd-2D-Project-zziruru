using UnityEngine;

public class KeyboardOrbit : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float orbitRadius = 1.2f;
    [SerializeField] private float orbitSpeed = 180f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float hitCooldown = 0.3f;

    private float angle;
    private float lastHitTime;

    private void Update()
    {
        if (target == null) return;

        angle += orbitSpeed * Time.deltaTime;

        float radian = angle * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(
            Mathf.Cos(radian),
            Mathf.Sin(radian),
            0f
        ) * orbitRadius;

        transform.position = target.position + offset;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryHit(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryHit(other);
    }

    private void TryHit(Collider2D other)
    {
        if (target == null) return;
        if (Time.time < lastHitTime + hitCooldown) return;

        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null) return;

        Vector2 hitDirection = enemy.transform.position - target.position;

        enemy.TakeDamage(damage, hitDirection);

        lastHitTime = Time.time;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
