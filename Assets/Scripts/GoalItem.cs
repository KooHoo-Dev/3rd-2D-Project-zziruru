using UnityEngine;

public class GoalItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager != null) gameManager.CollectGoalItem();

        Destroy(gameObject);
    }
}
