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

    [SerializeField] private float collisionSkin = 0.03f;

    private ContactFilter2D movementFilter;
    private readonly RaycastHit2D[] movementHits = new RaycastHit2D[8];
    [SerializeField] private LayerMask collisionLayer;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        movementFilter = new ContactFilter2D();
        movementFilter.useTriggers = false;
        movementFilter.SetLayerMask(collisionLayer);
    }

    private bool TryMoveTo(Vector2 nextPosition)
    {
        Vector2 move = nextPosition - rb.position;
        float distance = move.magnitude;

        if (distance <= 0.001f)
            return true;

        int hitCount = rb.Cast(
            move.normalized,
            movementFilter,
            movementHits,
            distance + collisionSkin
        );

        if (HasBlockingHit(hitCount))
            return false;

        rb.MovePosition(nextPosition);
        return true;
    }

    private bool HasBlockingHit(int hitCount)
    {
        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = movementHits[i].collider;
            if (hit == null) continue;

            // 플레이어/다른 적은 "맵 벽"이 아니니까 막힌 지형으로 보지 않음
            if (hit.GetComponentInParent<Player>() != null) continue;
            if (hit.GetComponentInParent<Enemy>() != null) continue;

            return true;
        }

        return false;
    }

    private void UpdateFlip(Vector2 direction)
    {
        if (spriteRenderer == null) return;
        if (Mathf.Abs(direction.x) <= 0.1f) return;

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

        Vector2 currentPosition = rb.position;

        Vector2 nextPosition = rb.position + panicDirection * panicSpeed * Time.fixedDeltaTime;

        Vector2 beforePosition = rb.position;

        if (!TryMoveTo(nextPosition))
        {
            SetupPanicMoveDirection();
            return;
        }
        
        Vector2 actualMove = rb.position - beforePosition;  
        UpdateFlip(panicDirection);

        isMoving = true;
    }

    private void CalmMove()
    {
        if (isMoving)
        {
            Vector2 currentPosition = rb.position;

            Vector2 nextPosition = Vector2.MoveTowards(
                currentPosition,
                targetPosition,
                calmSpeed * Time.fixedDeltaTime
            );

            Vector2 beforePosition = rb.position;

            if (!TryMoveTo(nextPosition))
            {
                PickNewCalmTarget();
                isMoving = true;
                return;
            }

            Vector2 actualMove = rb.position - beforePosition;
            UpdateFlip(actualMove);

            // rb.MovePosition(nextPosition);

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

    private void EnterScreenMove()
    {
        Vector2 currentPosition = rb.position;

        Vector2 nextPosition = Vector2.MoveTowards(
            currentPosition,
            targetPosition,
            enterSpeed * Time.fixedDeltaTime
        );

        Vector2 beforePosition = rb.position;

        // rb.MovePosition(nextPositon);
        if (!TryMoveTo(nextPosition))
        {
            isEntering = false;
            PickNewCalmTarget();
            return;
        }

        Vector2 actualMove = rb.position - beforePosition;
        UpdateFlip(actualMove);
        isMoving = true;

        if (Vector2.Distance(nextPosition, targetPosition) <= 0.05f)
        {
            isEntering = false;
            isMoving = false;
            waitTimer = Random.Range(0, calmWaitMax);
        }
    }
}
