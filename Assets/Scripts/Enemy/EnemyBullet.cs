using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float lifeTime = 4f;
    [SerializeField] private int damage = 10;

    private Vector2 moveDirection;
    
    public void Initialize(Vector2 direction)
    {
        moveDirection = direction.normalized;
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += (Vector3)(moveDirection * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
         PlayerStats playerStats = other.GetComponent<PlayerStats>();
        if (playerStats == null) return;

        playerStats.TakeDamage(damage, moveDirection);
        Destroy(gameObject);
    }
}
