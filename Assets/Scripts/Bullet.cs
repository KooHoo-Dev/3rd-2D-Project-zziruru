using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifeTime = 2f;
    [SerializeField] private int damage = 1;
    
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
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null) return;
        enemy.TakeDamage(damage);
        Destroy(gameObject);
    }
}
