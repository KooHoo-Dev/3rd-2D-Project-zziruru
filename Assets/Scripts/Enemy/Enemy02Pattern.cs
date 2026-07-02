using UnityEngine;
using System.Collections;

public class Enemy02Pattern : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform[] movePoints;

    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform player;

    [SerializeField] private float danceIdleDuration = 1.5f;
    [SerializeField] private float danceAttackDuration = 1.2f;
    [SerializeField] private float celebrationAttackDuration = 2f;

    private string currentAnimation;

    private Rigidbody2D rb;
    private Animator animator;
    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        StartCoroutine(PatternRoutine());
    }

    private IEnumerator PatternRoutine()
    {
        animator.Play("Enemy02_Idle");
        yield return new WaitForSeconds(1f);

        while (true)
        {
            yield return MoveToRandomPoint(); // 나중에는 플레이어랑 가까운 movePoint 로 이동하도록 고르는 메서드 넣기

            animator.Play("Enemy02_DanceIdle_02");
            yield return new WaitForSeconds(danceIdleDuration);

            animator.Play("Enemy02_DanceAttack");
            FireDanceAttack();
            yield return new WaitForSeconds(danceAttackDuration);

            yield return MoveToRandomPoint();

            animator.Play("Enemy02_DanceIdle_02");
            yield return new WaitForSeconds(danceIdleDuration);

            animator.Play("Enemy02_DanceAttack");
            FireDanceAttack();
            yield return new WaitForSeconds(danceAttackDuration);
        }
    }

    private IEnumerator MoveToRandomPoint()
    {
        if (movePoints == null || movePoints.Length == 0) yield break;
        Transform targetPoint = movePoints[Random.Range(0, movePoints.Length)];
        while (Vector2.Distance(rb.position, targetPoint.position) > 0.05f)
        {
            if (enemy != null && enemy.IsKnockbacking)
            {
                yield return new WaitForFixedUpdate();
                continue;
            }

            Vector2 direction = ((Vector2)targetPoint.position - rb.position).normalized;
            PlayWalkAnimation(direction);
            // Vector2 nextPosition = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
            Vector2 nextPosition = Vector2.MoveTowards(
                rb.position,
                targetPoint.position,
                moveSpeed * Time.fixedDeltaTime
            );
            rb.MovePosition(nextPosition);

            yield return new WaitForFixedUpdate();
        }
    }

    private void PlayWalkAnimation(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0f)
                PlayAnimation("Enemy02_WalkRight");
            else
                PlayAnimation("Enemy02_WalkLeft");
        }
        else
        {
            if (direction.y > 0f)
                PlayAnimation("Enemy02_WalkBack");
            else
                PlayAnimation("Enemy02_WalkFront");
        }
    }

    private void PlayAnimation(string animationName)
    {
        if (currentAnimation == animationName) return;

        currentAnimation = animationName;
        animator.Play(animationName);
    }

    void Update()
    {

    }

    private void FireDanceAttack()
    {
        if (enemyBulletPrefab == null || player == null) return;
        Vector2 baseDirection = (player.position - firePoint.position).normalized;
        FireBullet(baseDirection);
        FireBullet(RotateDirection(baseDirection, 20f));
        FireBullet(RotateDirection(baseDirection, -20f));
    }

    private void FireCelebrationAttack()
    {
        int bulletCount = 12;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = 360f / bulletCount * i;
            Vector2 direction = RotateDirection(Vector2.right, angle);
            FireBullet(direction);
        }
    }

    private void FireBullet(Vector2 direction)
    {
        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;
        GameObject bulletObject = Instantiate(enemyBulletPrefab, spawnPosition, Quaternion.identity);
        EnemyBullet bullet = bulletObject.GetComponent<EnemyBullet>();

        if (bullet != null)
            bullet.Initialize(direction);
    }

    private Vector2 RotateDirection(Vector2 direction, float angle)
    {
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
        return rotation * direction;
    }
}
