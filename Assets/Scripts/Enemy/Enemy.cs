using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHp = 3;
    private int currentHp;

    private void Start()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;

        if (currentHp <= 0) Die();
    }

    private void Die()
    {
        Enemy01Movement enemy01 = GetComponent<Enemy01Movement>();

        if (enemy01 != null && Enemy01Manager.Instance != null)
            Enemy01Manager.Instance.NotifyEnemyKilled();

        Destroy(gameObject);
    }
}
