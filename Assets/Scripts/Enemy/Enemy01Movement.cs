using UnityEngine;

public class Enemy01Movement : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float screenPadding = 0.5f;
    [SerializeField] private float enterSpeed = 2f;

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
    private bool isEntering = true;

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
        if (targetCamera == null)
            targetCamera = Camera.main;

        targetPosition = ClampToCameraBounds(rb.position);
        isMoving = true;

        if (Enemy01Manager.Instance != null && Enemy01Manager.Instance.IsPanic)
            StartPanic();
    }

    private Vector2 ClampToCameraBounds(Vector2 position)
    {
        Rect bounds = GetCameraBounds();

        float clampedX = Mathf.Clamp(position.x, bounds.xMin, bounds.xMax);
        float clampedY = Mathf.Clamp(position.y, bounds.yMin, bounds.yMax);

        return new Vector2(clampedX, clampedY);
    }

    private bool IsInsideCameraBounds(Vector2 position)
    {
        Rect bounds = GetCameraBounds();
        return bounds.Contains(position);
    }

    private Rect GetCameraBounds()
    {
        float halfHeight = targetCamera.orthographicSize;
        float halfWidth = halfHeight * targetCamera.aspect;

        Vector2 center = targetCamera.transform.position;

        float minX = center.x - halfWidth + screenPadding;
        float maxX = center.x + halfWidth - screenPadding;
        float minY = center.y - halfHeight + screenPadding;
        float maxY = center.y + halfHeight - screenPadding;

        return Rect.MinMaxRect(minX, minY, maxX, maxY);
    }

    private void FixedUpdate()
    {
        if (enemy != null && enemy.IsKnockbacking) return;

        if (isEntering)
            EnterScreenMove();
        else if (isPanic)
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

        Vector2 nextPosition = rb.position + panicDirection * panicSpeed * Time.fixedDeltaTime;

        if (!IsInsideCameraBounds(nextPosition))
        {
            nextPosition = ClampToCameraBounds(nextPosition);
            SetupPanicMoveDirection();
        }

        rb.MovePosition(nextPosition);
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
        Vector2 rawTargetPosition = rb.position + randomDirection * calmMoveDistance;

        targetPosition = ClampToCameraBounds(rawTargetPosition);
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

    private void EnterScreenMove()
    {
        Vector2 currentPosition = rb.position;

        Vector2 moveDirection = targetPosition - currentPosition;
        UpdateFlip(moveDirection);

        Vector2 nextPosition = Vector2.MoveTowards(
            currentPosition,
            targetPosition,
            enterSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(nextPosition);
        isMoving = true;

        if (Vector2.Distance(nextPosition, targetPosition) <= 0.05f)
        {
            isEntering = false;
            isMoving = false;
            waitTimer = Random.Range(0, calmWaitMax);
        }
    }
}
