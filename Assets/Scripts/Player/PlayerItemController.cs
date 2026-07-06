using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

public class PlayerItemController : MonoBehaviour
{
    [SerializeField] private SynergyData[] synergies;

    private readonly HashSet<ItemEffectType> acquiredEffects = new();
    private readonly HashSet<ItemEffectType> activatedSynergies = new();
    [SerializeField] private PlayerStats playerStats;

    // Keyboard
    [SerializeField] private GameObject keyboardOrbitObject;
    // Monitor
    [SerializeField] private MonitorProjectile monitorProjectilePrefab;
    [SerializeField] private float monitorInterval = 5f;

    // 적들이 몰려있다고 판단할 범위
    [SerializeField] private float monitorClusterRadius = 2f;

    [SerializeField] private float monitorExplosionRadius = 1.5f;
    private Coroutine monitorCoroutine;
    // Pumkin
    [SerializeField] private float pumpkinInterval = 5f;
    [SerializeField] private float pumpkinConfuseDuration = 1.5f;
    private Coroutine pumpkinCoroutine;
    // Skull Mask
    [SerializeField] private float skullInterval = 4f;
    [SerializeField] private float skullFearChance = 0.35f;
    [SerializeField] private float skullEffectRadius = 8f;
    private Coroutine skullCoroutine;
    // Steak
    [SerializeField] private float steakHealInterval = 3f;
    private Coroutine steakCoroutine;

    public event Action<ItemData> OnItemApplied;

    private void Awake()
    {
        if (playerStats == null)
            playerStats = GetComponent<PlayerStats>();
    }

    public void ApplyItem(ItemData item)
    {
        if (item == null) return;

        acquiredEffects.Add(item.effectType);
        ApplyEffect(item.effectType, item.value);
        CheckSynergies();

        OnItemApplied?.Invoke(item);
    }

    private bool HasBoth(ItemEffectType a, ItemEffectType b)
    {
        return acquiredEffects.Contains(a) && acquiredEffects.Contains(b);
    }

    private void CheckSynergies()
    {
        foreach (SynergyData synergy in synergies)
        {
            if (activatedSynergies.Contains(synergy.resultEffect)) continue;
            if (!HasBoth(synergy.firstEffect, synergy.secondEffect)) continue;

            activatedSynergies.Add(synergy.resultEffect);
            ApplyEffect(synergy.resultEffect, synergy.value);
        }
    }

    private void ApplyEffect(ItemEffectType effectType, float value)
    {
        Debug.Log($"Apply effect: {effectType}, value: {value}");

        switch (effectType)
        {
            case ItemEffectType.Base0:
                EnableKeyboardOrbit();
                break;
            case ItemEffectType.Base1:
                EnableMonitor(value);
                break;
            case ItemEffectType.Base2:
                EnableHollowPumpkin();
                break;
            case ItemEffectType.Base3:
                EnableSkullMask(value);
                break;
            case ItemEffectType.Base4:
                IncreaseMaxHp(value);
                break;
            case ItemEffectType.Base5:
                EnablePowerSteak(value);
                break;
        }
    }

    private void EnableMonitor(float value)
    {
        if (monitorCoroutine != null)
            StopCoroutine(monitorCoroutine);

        monitorCoroutine = StartCoroutine(MonitorRoutine((int)value));
    }

    private IEnumerator MonitorRoutine(int damage)
    {
        while (true)
        {
            yield return new WaitForSeconds(monitorInterval);

            if (!TryFindBestMonitorTarget(out Vector2 targetPosition)) continue;
            if (monitorProjectilePrefab == null) continue;

            MonitorProjectile projectile = Instantiate(
                monitorProjectilePrefab,
                transform.position,
                Quaternion.identity
            );

            projectile.Initialize(
                targetPosition,
                damage,
                monitorExplosionRadius
            );
        }
    }

    private bool TryFindBestMonitorTarget(out Vector2 targetPosition)
    {
        targetPosition = Vector2.zero;

        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        if (enemies.Length == 0) return false;

        int bestCount = 0;
        Vector2 bestCenter = enemies[0].transform.position;

        foreach (Enemy centerEnemy in enemies)
        {
            int count = 0;
            Vector2 positionSum = Vector2.zero;

            foreach (Enemy otherEnemy in enemies)
            {
                float distance = Vector2.Distance(
                    centerEnemy.transform.position,
                    otherEnemy.transform.position
                );

                if (distance > monitorClusterRadius) continue;

                count++;
                positionSum += (Vector2)otherEnemy.transform.position;
            }

            if (count > bestCount)
            {
                bestCount = count;
                bestCenter = positionSum / count;
            }
        }

        targetPosition = bestCenter;
        return true;
    }

    private void IncreaseMaxHp(float value)
    {
        if (playerStats == null) return;

        playerStats.IncreaseMaxHp((int)value);
    }

    private void EnableSkullMask(float value)
    {
        if (skullCoroutine != null)
            StopCoroutine(skullCoroutine);

        skullCoroutine = StartCoroutine(SkullMaskRoutine(value));
    }

    private IEnumerator SkullMaskRoutine(float duration)
    {
        while (true)
        {
            yield return new WaitForSeconds(skullInterval);

            EnemyChaseMovement[] chasingEnemies =
                FindObjectsByType<EnemyChaseMovement>(FindObjectsSortMode.None);

            foreach (EnemyChaseMovement enemy in chasingEnemies)
            {
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance > skullEffectRadius) continue;

                if (UnityEngine.Random.value > skullFearChance) continue;

                enemy.FleeFromPlayer(duration);
            }
        }
    }

    private void EnablePowerSteak(float value)
    {
        if (steakCoroutine != null)
            StopCoroutine(steakCoroutine);

        steakCoroutine = StartCoroutine(PowerSteakRoutine((int)value));
    }

    private IEnumerator PowerSteakRoutine(int healAmount)
    {
        while (true)
        {
            yield return new WaitForSeconds(steakHealInterval);

            if (playerStats != null)
                playerStats.Heal(healAmount);
        }
    }

    private void EnableHollowPumpkin()
    {
        if (pumpkinCoroutine != null)
            StopCoroutine(pumpkinCoroutine);

        pumpkinCoroutine = StartCoroutine(HollowPumpkinRoutine());
    }

    private IEnumerator HollowPumpkinRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(pumpkinInterval);

            EnemyChaseMovement[] chasingEnemies =
                FindObjectsByType<EnemyChaseMovement>(FindObjectsSortMode.None);

            foreach (EnemyChaseMovement enemy in chasingEnemies)
                enemy.LosePlayer(pumpkinConfuseDuration);
        }
    }

    private void EnableKeyboardOrbit()
    {
        if (keyboardOrbitObject == null) return;

        keyboardOrbitObject.SetActive(true);

        KeyboardOrbit keyboardOrbit = keyboardOrbitObject.GetComponent<KeyboardOrbit>();
        if (keyboardOrbit != null)
            keyboardOrbit.SetTarget(transform);
    }

    public bool HasAcquiredEffect(ItemEffectType effectType)
    {
        return acquiredEffects.Contains(effectType);
    }
}
