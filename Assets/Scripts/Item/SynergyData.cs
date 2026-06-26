using UnityEngine;

[CreateAssetMenu(menuName = "Items/Synergy Data")]
public class SynergyData : ScriptableObject
{
    public ItemEffectType firstEffect;
    public ItemEffectType secondEffect;

    public ItemEffectType resultEffect;
    public float value = 1f;

    public string synergyName;
    [TextArea] public string description;
    public Sprite icon;
}