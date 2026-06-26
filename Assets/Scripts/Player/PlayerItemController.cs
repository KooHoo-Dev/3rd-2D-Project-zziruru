using System.Collections.Generic;
using UnityEngine;

public class PlayerItemController : MonoBehaviour
{
    [SerializeField] private SynergyData[] synergies;

    private readonly HashSet<ItemEffectType> acquiredEffects = new();
    private readonly HashSet<ItemEffectType> activatedSynergies = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ApplyItem(ItemData item)
    {
        if (item == null) return;

        acquiredEffects.Add(item.effectType);
        ApplyEffect(item.effectType, item.value);
        CheckSynergies();
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
    }

    public bool HasAcquiredEffect(ItemEffectType effectType)
    {
        return acquiredEffects.Contains(effectType);
    }
}
