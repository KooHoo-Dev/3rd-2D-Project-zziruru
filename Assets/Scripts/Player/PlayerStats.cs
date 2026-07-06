using System.Collections;
using UnityEngine;
using System;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int maxHp = 100;
    [SerializeField] private int maxExp = 100;
    [SerializeField] private float invincibleDuration = 0.8f;

    private int currentHp;
    private int currentExp;

    public int CurrentHp => currentHp;
    public int MaxHp => maxHp;
    public int CurrentExp => currentExp;
    public int MaxExp => maxExp;

    public event Action OnStatsChanged;
    public event Action OnDied;
    public event Action<Vector2> OnDamaged;

    private bool isDead;
    private bool isInvincible;

    public event Action OnExpFull;

    // 이미 꽉 찼는데 더 채우려고 하는것 방지
    private bool isExpFull;

    private void Awake()
    {
        currentHp = maxHp;
        currentExp = 0;
    }

    private void Start()
    {
        OnStatsChanged?.Invoke();
    }

    public void IncreaseMaxHp(int amount)
    {
        if (isDead) return;

        maxHp += amount;
        currentHp += amount;

        OnStatsChanged?.Invoke();
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHp = Mathf.Clamp(currentHp + amount, 0, maxHp);
        OnStatsChanged?.Invoke();
    }

    public void TakeDamage(int damage, Vector2 hitDirection)
    {
        if (isDead) return;
        if (isInvincible) return;

        currentHp = Mathf.Clamp(currentHp - damage, 0, maxHp);
        OnStatsChanged?.Invoke();
        OnDamaged?.Invoke(hitDirection);

        if (currentHp <= 0)
        {
            isDead = true;
            OnDied?.Invoke();
        }
        else
        {
            StartCoroutine(InvincibleRoutine());
        }
    }

    private IEnumerator InvincibleRoutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleDuration);
        isInvincible = false;
    }

    public void AddExp(int amount)
    {
        if (isExpFull) return;

        currentExp = Mathf.Clamp(currentExp + amount, 0, maxExp);
        OnStatsChanged?.Invoke();

        if (currentExp >= maxExp)
        {
            isExpFull = true;
            OnExpFull?.Invoke();
        }
    }

    public void ResetExp()
    {
        currentExp = 0;
        isExpFull = false;
        OnStatsChanged?.Invoke();
    }

    public void SetMaxExp(int value)
    {
        maxExp = value;
        currentExp = Mathf.Clamp(currentExp, 0, maxExp);
        OnStatsChanged?.Invoke();
    }
}
