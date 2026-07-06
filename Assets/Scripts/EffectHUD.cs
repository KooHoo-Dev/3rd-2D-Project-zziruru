using UnityEngine;
using UnityEngine.UI;

public class EffectHUD : MonoBehaviour
{
    [SerializeField] private PlayerItemController playerItemController;
    [SerializeField] private Transform iconParent;
    [SerializeField] private Image iconPrefab;

    private void OnEnable()
    {
        playerItemController.OnItemApplied += AddItemIcon;
    }

    private void OnDisable()
    {
        playerItemController.OnItemApplied -= AddItemIcon;
    }

    private void AddItemIcon(ItemData item)
    {
        Image icon = Instantiate(iconPrefab, iconParent);
        icon.sprite = item.icon;
    }
}
