using UnityEngine;

public class EnemyBodyDamage : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float damageInterval = 1f;
    
    private float lastDamageTime;

    // collider 끼리 부닥쳤을때
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (Time.time < lastDamageTime + damageInterval) return;

        PlayerStats playerStats = collision.collider.GetComponentInParent<PlayerStats>();
        if (playerStats == null) return;

        Vector2 hitDirection = playerStats.transform.position - transform.position;

        playerStats.TakeDamage(damage, hitDirection);
        lastDamageTime = Time.time;
    }
}
