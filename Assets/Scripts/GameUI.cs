using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Image hpFillImage;
    [SerializeField] private Image expFillImage;
    [SerializeField] private TMP_Text hpValueText;
    [SerializeField] private TMP_Text expValueText;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TMP_Text timeText;

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

    private void Update()
    {
        if (gameManager == null || timeText == null) return;

        timeText.text = FormatTime(gameManager.PlayTime);
    }

    private string FormatTime(float time)
    {
        int totalSeconds = Mathf.FloorToInt(time);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        return $"{minutes:00}:{seconds:00}";
    }

    private void UpdateUI()
    {
        hpFillImage.fillAmount = (float)playerStats.CurrentHp / playerStats.MaxHp;
        expFillImage.fillAmount = (float)playerStats.CurrentExp / playerStats.MaxExp;

        if (hpValueText != null)
            hpValueText.text = $"{playerStats.CurrentHp} / {playerStats.MaxHp}";

        if (expValueText != null)
            expValueText.text = $"{playerStats.CurrentExp} / {playerStats.MaxExp}";
    }
}
