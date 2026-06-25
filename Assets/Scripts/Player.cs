using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform mousePoint;
    [SerializeField] private float moveSpeed = 5f;

    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float attackCooldown = 0.3f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private float lastAttackTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
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

        Vector2 shootDirection = mousePoint.position - firePoint.position;

        if (shootDirection.sqrMagnitude <= 0.001f) return;

        Bullet bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.Initialize(shootDirection);

        animator.SetTrigger("Attack");

        lastAttackTime = Time.time;
    }

    // mousePoint 클릭 좌표를 받아와서 Bullet 생성(생성할때 좌표넘겨주면 그 좌표로 이동하는 bullet 생성)

    private void FixedUpdate()
    {
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

        Vector2 direction = mousePoint.position - transform.position;

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
}
