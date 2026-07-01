using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Image hpFillImage;
    [SerializeField] private Image expFillImage;

    private void Start()
    {
        playerStats.OnStatsChanged += UpdateUI;
        UpdateUI();
    }

    private void OnDestroy()
    {
        if (playerStats == null) return;

        playerStats.OnStatsChanged -= UpdateUI;
    }

    private void UpdateUI()
    {
        hpFillImage.fillAmount = (float)playerStats.CurrentHp / playerStats.MaxHp;
        expFillImage.fillAmount = (float)playerStats.CurrentExp / playerStats.MaxExp;
    }
}
