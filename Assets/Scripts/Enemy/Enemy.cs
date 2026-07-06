using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHp = 3;
    private int currentHp;
    private Rigidbody2D rb;
    private HitFeedback hitFeedback;

    [SerializeField] private float knockbackSpeed = 8f;
    [SerializeField] private float knockbackDuration = 0.08f;

    private Vector2 knockbackDirection;
    private float knockbackTimer;

    public bool IsKnockbacking => knockbackTimer > 0f;
    [SerializeField] private int expReward = 20;
    private bool isDead;
    [SerializeField] private GameObject healItemPrefab;
    [SerializeField, Range(0f, 1f)] private float healItemDropChance = 0.1f;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hitFeedback = GetComponent<HitFeedback>();
    }

    private void Start()
    {
        currentHp = maxHp;
    }

    // public void TakeDamage(int damage)
    // {
    //     TakeDamage(damage, Vector2.zero);
    // }

    public void TakeDamage(int damage, Vector2 hitDirection)
    {
        if (isDead) return;

        currentHp -= damage;

        if (hitFeedback != null)
            hitFeedback.PlayHitFlash();

        if (hitDirection.sqrMagnitude > 0.001f)
        {
            knockbackDirection = hitDirection.normalized;
            knockbackTimer = knockbackDuration;
        }

        if (currentHp <= 0)
        {
            isDead = true;
            StartCoroutine(DieRoutine());
        }
    }

    private void FixedUpdate()
    {
        if (knockbackTimer <= 0f) return;

        knockbackTimer -= Time.fixedDeltaTime;

        rb.MovePosition(
            rb.position + knockbackDirection * knockbackSpeed * Time.fixedDeltaTime
        );
    }

    private IEnumerator DieRoutine()
    {
        Enemy01Movement enemy01 = GetComponent<Enemy01Movement>();

        if (enemy01 != null && Enemy01Manager.Instance != null)
            Enemy01Manager.Instance.NotifyEnemyKilled();

        if (hitFeedback != null)
            yield return hitFeedback.PlayDeathFade();

        TryDropHealItem(transform.position);

        PlayerStats playerStats = FindFirstObjectByType<PlayerStats>();
        if (playerStats != null)
            playerStats.AddExp(expReward);

        Destroy(gameObject);
    }

    private void TryDropHealItem(Vector3 dropPosition)
    {
        if (healItemPrefab == null) return;
        if (Random.value > healItemDropChance) return;

        Instantiate(healItemPrefab, dropPosition, Quaternion.identity);
    }
}
