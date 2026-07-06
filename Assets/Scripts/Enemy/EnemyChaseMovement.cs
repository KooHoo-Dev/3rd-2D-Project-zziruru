using UnityEngine;

public class EnemyChaseMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform visualRoot;

    private Rigidbody2D rb;
    private Enemy enemy;

    [SerializeField] private float confusedMoveSpeed = 1.5f;

    private bool isConfused;
    private float confusedTimer;
    private Vector2 confusedDirection;

    [SerializeField] private float fleeMoveSpeed = 3.5f;

    private bool isFleeing;
    private float fleeTimer;

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

        Vector2 direction;

        if (isFleeing)
        {
            fleeTimer -= Time.fixedDeltaTime;

            direction = (rb.position - (Vector2)player.position).normalized;

            if (direction.sqrMagnitude <= 0.001f)
                direction = Random.insideUnitCircle.normalized;

            rb.MovePosition(
                rb.position + direction * fleeMoveSpeed * Time.fixedDeltaTime
            );

            if (fleeTimer <= 0f)
                isFleeing = false;
        }
        else if (isConfused)
        {
            confusedTimer -= Time.fixedDeltaTime;
            direction = confusedDirection;

            rb.MovePosition(
                rb.position + direction * confusedMoveSpeed * Time.fixedDeltaTime
            );

            if (confusedTimer <= 0f)
                isConfused = false;
        }
        else
        {
            direction = ((Vector2)player.position - rb.position).normalized;

            rb.MovePosition(
                rb.position + direction * moveSpeed * Time.fixedDeltaTime
            );
        }

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

    public void LosePlayer(float duration)
    {
        isConfused = true;
        confusedTimer = duration;
        confusedDirection = Random.insideUnitCircle.normalized;
    }

    public void FleeFromPlayer(float duration)
    {
        isFleeing = true;
        fleeTimer = duration;
    }
}
