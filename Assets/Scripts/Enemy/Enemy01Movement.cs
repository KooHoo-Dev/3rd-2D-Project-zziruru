using UnityEngine;

public class Enemy01Movement : MonoBehaviour
{
    [SerializeField] private float calmSpeed = 1.2f;
    [SerializeField] private float calmMoveDistance = 1.5f;
    [SerializeField] private float calmWaitMin = 3f;
    [SerializeField] private float calmWaitMax = 7f;
    [SerializeField] private float panicSpeed = 5f;
    [SerializeField] private float panicDirectionChangeInterval = 0.5f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Vector2 targetPosition;
    private Vector2 panicDirection;

    private float waitTimer;
    private float panicDirectionTimer;

    private bool isMoving;
    private bool isPanic;
    private Enemy enemy;
    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void UpdateFlip(Vector2 direction)
    {
        if (spriteRenderer == null) return;
        if (Mathf.Abs(direction.x) <= 0.01f) return;

        spriteRenderer.flipX = direction.x < 0f;
    }

    private void Start()
    {
        waitTimer = Random.Range(0f, calmWaitMax);
        if (Enemy01Manager.Instance != null && Enemy01Manager.Instance.IsPanic)
            StartPanic();
    }

    private void FixedUpdate()
    {
        if (enemy != null && enemy.IsKnockbacking) return;

        if (isPanic)
            PanicMove();
        else
            CalmMove();

        if (animator != null)
            animator.SetBool("IsMoving", isMoving);
    }

    private void PanicMove()
    {
        panicDirectionTimer -= Time.fixedDeltaTime;

        if (panicDirectionTimer <= 0f)
            SetupPanicMoveDirection();
            
        UpdateFlip(panicDirection);
        rb.MovePosition(rb.position + panicDirection * panicSpeed * Time.fixedDeltaTime);
        isMoving = true;
    }

    private void CalmMove()
    {
        if (isMoving)
        {
            Vector2 currentPosition = rb.position;

            Vector2 moveDirection = targetPosition - currentPosition;
            UpdateFlip(moveDirection);

            Vector2 nextPosition = Vector2.MoveTowards(
                currentPosition,
                targetPosition,
                calmSpeed * Time.fixedDeltaTime
            );

            rb.MovePosition(nextPosition);

            if (Vector2.Distance(nextPosition, targetPosition) <= 0.05f)
            {
                isMoving = false;
                waitTimer = Random.Range(calmWaitMin, calmWaitMax);
            }
        }
        else
        {
            waitTimer -= Time.fixedDeltaTime;
            if (waitTimer < 0f)
            {
                PickNewCalmTarget();
                isMoving = true;
            }
        }
    }

    private void PickNewCalmTarget()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        targetPosition = rb.position + randomDirection * calmMoveDistance;
    }

    public void StartPanic()
    {
        isPanic = true;
        SetupPanicMoveDirection();
    }

    private void SetupPanicMoveDirection()
    {
        panicDirection = Random.insideUnitCircle.normalized;
        panicDirectionTimer = panicDirectionChangeInterval;
    }
}
