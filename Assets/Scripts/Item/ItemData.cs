using System;
using UnityEngine;

public enum ItemPhase
{
    Base,
    Upgrade
}

public enum ItemEffectType
{
    None,
    Base0,
    Base1,
    Base2,
    Base3,
    Base4,
    Base5,

    Upgrade0,
    Upgrade1,
    Upgrade2,
    Synergy0,
    Synergy1,
    Synergy2
}

[CreateAssetMenu(menuName = "Items/Item Data")]
public class ItemData : ScriptableObject
{
    public int itemId;
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;

    public ItemPhase phase;
    public ItemEffectType effectType;
    public float value = 1f;
}