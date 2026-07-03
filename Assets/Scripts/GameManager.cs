using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Playing,
        ItemSelect,
        Clear,
        GameOver
    }

    [SerializeField] private List<ItemData> allItems = new List<ItemData>();
    [SerializeField] private TMP_Text[] itemTitleTexts;
    [SerializeField] private TMP_Text[] itemDescriptionTexts;
    [SerializeField] private Image[] itemIconImages;
    private List<ItemData> currentChoices = new List<ItemData>();

    [SerializeField] private SynergyData[] synergies;
    [SerializeField] private GameState currentState;

    [SerializeField] private GameObject itemSelectionPanel;

    [SerializeField] private Player player;

    [SerializeField] private PlayerItemController playerItemController;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gamePausedPanel;
    [SerializeField] private GameObject clearPanel;
    [SerializeField] private PlayerStats playerStats;

    private bool isPaused;

    [SerializeField] private int[] expRequirementsByLevel = { 100, 150, 220, 300 };
    private int currentLevel = 1;

    private void OnEnable()
    {
        if (playerStats != null)
            playerStats.OnExpFull += HandleExpFull;
    }

    private void OnDisable()
    {
        if (playerStats != null)
            playerStats.OnExpFull -= HandleExpFull;
    }

    private void Start()
    {
        currentLevel = 1;
        SetupCurrentLevelExp();
        ChangeState(GameState.Playing);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (gamePausedPanel != null)
            gamePausedPanel.SetActive(false);

        if (clearPanel != null)
            clearPanel.SetActive(false);
    }

    private void SetupCurrentLevelExp()
    {
        if (playerStats == null) return;

        int expIndex = Mathf.Clamp(currentLevel - 1, 0, expRequirementsByLevel.Length - 1);
        playerStats.SetMaxExp(expRequirementsByLevel[expIndex]);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            TogglePause();
    }

    private void HandleExpFull()
    {
        if (currentState != GameState.Playing) return;

        ChangeState(GameState.ItemSelect);
    }

    private void TogglePause()
    {
        if (!isPaused && currentState != GameState.Playing) return;

        if (isPaused)
            Resume();
        else
            Pause();
    }

    private void ChangeState(GameState nextState)
    {
        currentState = nextState;

        switch (currentState)
        {
            case GameState.Playing:
                StartPlaying();
                break;

            case GameState.ItemSelect:
                OpenItemSelection(GetCurrentItemPhase());
                break;

            case GameState.Clear:
                Clear();
                break;

            case GameState.GameOver:
                GameOver();
                break;
        }
    }

    private void StartPlaying()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (itemSelectionPanel != null)
            itemSelectionPanel.SetActive(false);

        if (gamePausedPanel != null)
            gamePausedPanel.SetActive(false);

        player.SetControlEnabled(true);

        Debug.Log($"Playing Start - Level {currentLevel}");
    }

    private ItemPhase GetCurrentItemPhase()
    {
        if (currentLevel <= 2)
            return ItemPhase.Base;

        return ItemPhase.Upgrade;
    }

    private void OpenItemSelection(ItemPhase phase)
    {
        Time.timeScale = 0f;
        player.SetControlEnabled(false);

        itemSelectionPanel.SetActive(true);

        currentChoices.Clear();

        List<ItemData> candidates = new List<ItemData>();

        foreach (ItemData item in allItems)
        {
            if (item.phase == phase)
            {
                candidates.Add(item);
            }
        }

        for (int i = 0; i < 3; i++)
        {
            if (candidates.Count == 0) break;

            int randomIndex = Random.Range(0, candidates.Count);
            ItemData selectedItem = candidates[randomIndex];
            // 임시
            // ItemData selectedItem = candidates[i];

            currentChoices.Add(selectedItem);

            // 임시
            // candidates.RemoveAt(i);
            candidates.RemoveAt(randomIndex);

            itemTitleTexts[i].text = selectedItem.itemName;
            itemDescriptionTexts[i].text = selectedItem.description;
            itemIconImages[i].sprite = selectedItem.icon;
        }
    }

    private void Clear()
    {
        Time.timeScale = 0f;
        player.SetControlEnabled(false);

        if (gamePausedPanel != null)
            gamePausedPanel.SetActive(false);

        if (clearPanel != null)
        {
            clearPanel.SetActive(true);
            clearPanel.transform.SetAsLastSibling();
        }

        Debug.Log("Clear");
    }

    private void GameOver()
    {
        Time.timeScale = 0f;
        player.SetControlEnabled(false);

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            gameOverPanel.transform.SetAsLastSibling();
        }

        Debug.Log("Game Over");
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        player.SetControlEnabled(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoHome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    private void Pause()
    {
        isPaused = true;

        Time.timeScale = 0f;
        player.SetControlEnabled(false);

        if (gamePausedPanel != null)
        {
            gamePausedPanel.SetActive(true);
            gamePausedPanel.transform.SetAsLastSibling();
        }
    }
    public void Resume()
    {
        isPaused = false;

        Time.timeScale = 1f;
        player.SetControlEnabled(true);

        if (gamePausedPanel != null)
            gamePausedPanel.SetActive(false);
    }

    public void SelectItem(int index)
    {
        if (index < 0 || index >= currentChoices.Count) return;

        ItemData selectedItem = currentChoices[index];

        ApplyItem(selectedItem);

        itemSelectionPanel.SetActive(false);

        LevelUp();

        if (HasClearedAllLevels())
            ChangeState(GameState.Clear);
        else
            ChangeState(GameState.Playing);
    }

    private void LevelUp()
    {
        currentLevel++;

        if (playerStats != null)
        playerStats.ResetExp();

        SetupCurrentLevelExp();
    }

    private bool HasClearedAllLevels()
    {
        return currentLevel > expRequirementsByLevel.Length;
    }

    private void ApplyItem(ItemData item)
    {
        Debug.Log($"Selected: {item.itemName}");
        playerItemController.ApplyItem(item);
    }

    private SynergyData FindSynergy(ItemData candidateItem)
    {
        foreach (SynergyData synergy in synergies)
        {
            bool candidateIsFirst = synergy.firstEffect == candidateItem.effectType;
            bool candidateIsSecond = synergy.secondEffect == candidateItem.effectType;

            if (!candidateIsFirst && !candidateIsSecond) continue;

            ItemEffectType pairEffect = candidateIsFirst
                ? synergy.secondEffect
                : synergy.firstEffect;

            if (playerItemController.HasAcquiredEffect(pairEffect)) return synergy;
        }

        return null;
    }

    public void RequestGameOver()
    {
        ChangeState(GameState.GameOver);
    }

    public void RequestClear()
    {
        ChangeState(GameState.Clear);
    }
}
