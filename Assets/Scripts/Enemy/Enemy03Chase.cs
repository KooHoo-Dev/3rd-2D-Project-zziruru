using UnityEngine;

public class Enemy03Chase : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform visualRoot;

    private Rigidbody2D rb;
    private Enemy enemy;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy = GetComponent<Enemy>();
    }

    private void Start()
    {
        if (player == null)
        {
            Player foundPlayer = FindFirstObjectByType<Player>();
            if (foundPlayer != null)
                player = foundPlayer.transform;
        }
    }

    private void FixedUpdate()
    {
        if (player == null) return;
        if (enemy != null && enemy.IsKnockbacking) return;

        Vector2 direction = ((Vector2)player.position - rb.position).normalized;

        rb.MovePosition(
            rb.position + direction * moveSpeed * Time.fixedDeltaTime
        );

        UpdateVisual(direction);
    }

    private void UpdateVisual(Vector2 direction)
    {
        // if (visualRoot != null && Mathf.Abs(direction.x) > 0.01f)
        // {
        //     Vector3 scale = visualRoot.localScale;
        //     scale.x = Mathf.Abs(scale.x) * (direction.x < 0f ? -1f : 1f);
        //     visualRoot.localScale = scale;
        // }

        if (visualRoot != null && Mathf.Abs(direction.x) > 0.01f)
        {
            if (direction.x < 0f)
                visualRoot.localRotation = Quaternion.Euler(0f, 180f, 0f);
            else
                visualRoot.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }


        if (animator != null)
        {
            animator.SetBool("1_Move", direction.sqrMagnitude > 0.01f);
            animator.SetFloat("MoveSpeed", moveSpeed);
        }
    }
}
