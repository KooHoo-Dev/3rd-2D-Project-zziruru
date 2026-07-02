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

    private void Awake()
    {
        currentHp = maxHp;
        currentExp = 0;
    }
    
    private void Start()
    {
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
        currentExp = Mathf.Clamp(currentExp + amount, 0, maxExp);
        OnStatsChanged?.Invoke();
    }
}
