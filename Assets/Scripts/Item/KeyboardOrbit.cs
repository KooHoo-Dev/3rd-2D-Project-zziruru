using UnityEngine;

public class KeyboardOrbit : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private Vector2 orbitRadius = new Vector2(1.2f, 0.55f);
    [SerializeField] private Vector2 orbitCenterOffset = new Vector2(0f, 0.35f);

    [SerializeField] private float orbitSpeed = 180f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float hitCooldown = 0.3f;

    [SerializeField] private SpriteRenderer keyboardRenderer;
    [SerializeField] private SpriteRenderer targetRenderer;

    private float angle;
    private float lastHitTime;

    private void Awake()
    {
        if (keyboardRenderer == null)
            keyboardRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (target == null) return;

        angle += orbitSpeed * Time.deltaTime;

        float radian = angle * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(
            Mathf.Cos(radian) * orbitRadius.x,
            Mathf.Sin(radian) * orbitRadius.y,
            0f
        );

        Vector3 center = target.position + (Vector3)orbitCenterOffset;
        transform.position = center + offset;

        UpdateSorting(offset);
    }

    private void UpdateSorting(Vector3 offset)
    {
        if (keyboardRenderer == null || targetRenderer == null) return;

        if (offset.y > 0f)
            keyboardRenderer.sortingOrder = targetRenderer.sortingOrder - 1;
        else
            keyboardRenderer.sortingOrder = targetRenderer.sortingOrder + 1;
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

        if (target != null)
        targetRenderer = target.GetComponent<SpriteRenderer>();
    }
}
