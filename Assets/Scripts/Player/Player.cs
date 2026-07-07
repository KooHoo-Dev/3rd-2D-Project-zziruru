using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject mousePoint;
    [SerializeField] private float moveSpeed = 5f;

    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float attackCooldown = 0.3f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private float lastAttackTime;

    private bool canControl = true;

    private PlayerStats playerStats;
    private bool isDead;
    private HitFeedback hitFeedback;

    // TODO: knockback 효과 쪽 enemy 랑 공통 분모로 뽑아낼 수 있는지
    [SerializeField] private float knockbackSpeed = 10f;
    [SerializeField] private float knockbackDuration = 0.12f;

    private Vector2 knockbackDirection;
    private float knockbackTimer;

    private bool IsKnockbacking => knockbackTimer > 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerStats = GetComponent<PlayerStats>();
        hitFeedback = GetComponent<HitFeedback>();
    }

    private void OnEnable()
    {
        if (playerStats != null)
        {
            playerStats.OnDamaged += PlayHitFeedback;
            playerStats.OnDied += Die;
        }
    }

    private void OnDisable()
    {
        if (playerStats != null)
        {
            playerStats.OnDied -= Die;
            playerStats.OnDamaged -= PlayHitFeedback;
        }

    }

    private void Update()
    {
        if (!canControl) return;
        if (isDead) return;
        if (IsKnockbacking) return;

        ReadMoveInput();
        LookAtMouse();

        animator.SetBool("IsMoving", moveInput.sqrMagnitude > 0.01f);

        Shoot();
    }

    private void Shoot()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        if (Time.time < lastAttackTime + attackCooldown) return;

        if (bulletPrefab == null || firePoint == null || mousePoint == null) return;

        Vector2 shootDirection = mousePoint.transform.position - firePoint.position;

        if (shootDirection.sqrMagnitude <= 0.001f) return;

        Bullet bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.Initialize(shootDirection);

        animator.SetTrigger("Attack");

        lastAttackTime = Time.time;
    }

    public void TeleportTo(Vector2 position)
    {
        moveInput = Vector2.zero;
        knockbackTimer = 0f;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        rb.position = position;
        transform.position = position;

        animator.SetBool("IsMoving", false);
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        if (IsKnockbacking)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            rb.MovePosition(rb.position + knockbackDirection * knockbackSpeed * Time.fixedDeltaTime);
            return;
        }

        if (!canControl) return;

        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    private void ReadMoveInput()
    {
        moveInput = Vector2.zero;

        if (Keyboard.current.wKey.isPressed) moveInput.y += 1f;
        if (Keyboard.current.sKey.isPressed) moveInput.y -= 1f;
        if (Keyboard.current.dKey.isPressed) moveInput.x += 1f;
        if (Keyboard.current.aKey.isPressed) moveInput.x -= 1f;

        moveInput = moveInput.normalized;
    }

    private void LookAtMouse()
    {
        if (mousePoint == null) return;

        Vector2 direction = mousePoint.transform.position - transform.position;

        if (direction.sqrMagnitude <= 0.001f) return;

        SetLookAnimation(direction.normalized);
    }

    private void SetLookAnimation(Vector2 direction)
    {
        bool shouldFlip = direction.x < 0f;
        spriteRenderer.flipX = shouldFlip;

        float animationX = Mathf.Abs(direction.x);
        float animationY = direction.y;

        animator.SetFloat("LookX", animationX);
        animator.SetFloat("LookY", animationY);
    }

    public void SetControlEnabled(bool enabled)
    {
        canControl = enabled;

        if (mousePoint != null)
            mousePoint.SetActive(enabled);

        if (!canControl)
        {
            moveInput = Vector2.zero;
            animator.SetBool("IsMoving", false);
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        SetControlEnabled(false);

        animator.SetTrigger("Die");

        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
        yield return new WaitForSeconds(1f);
        GameManager gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager != null)
            gameManager.RequestGameOver();
    }

    private void PlayHitFeedback(Vector2 hitDirection)
    {
        if (hitFeedback != null)
            hitFeedback.PlayHitFlash();

        if (hitDirection.sqrMagnitude > 0.001f)
        {
            knockbackDirection = hitDirection.normalized;
            knockbackTimer = knockbackDuration;
        }

        moveInput = Vector2.zero;
        animator.SetBool("IsMoving", false);
    }
}
