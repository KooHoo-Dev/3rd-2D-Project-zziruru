using UnityEngine;

public class HealItem : MonoBehaviour
{
    [SerializeField] private int healAmount = 20;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerStats playerStats = other.GetComponentInParent<PlayerStats>();
        if (playerStats == null) return;

        playerStats.Heal(healAmount);
        Destroy(gameObject);
    }
}
