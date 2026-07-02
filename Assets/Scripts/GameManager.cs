using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Wave1,
        ItemSelect1,
        Wave2,
        ItemSelect2,
        Wave3,
        ItemSelect3,
        Wave4,
        Clear,
        GameOver
    }

    [SerializeField] private List<ItemData> allItems = new List<ItemData>();
    [SerializeField] private TMP_Text[] itemTitleTexts;
    [SerializeField] private TMP_Text[] itemDescriptionTexts;
    private List<ItemData> currentChoices = new List<ItemData>();

    [SerializeField] private SynergyData[] synergies;
    [SerializeField] private GameState currentState;

    private int collectedItemCount;
    [SerializeField] private int requiredItemCount = 3;

    [SerializeField] private GameObject itemSelectionPanel;

    [SerializeField] private Player player;

    [SerializeField] private PlayerItemController playerItemController;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gamePausedPanel;
    [SerializeField] private GameObject clearPanel;
    [SerializeField] private PlayerStats playerStats;

    private bool isPaused;

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
        ChangeState(GameState.Wave1);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (gamePausedPanel != null)
            gamePausedPanel.SetActive(false);

        if (clearPanel != null)
            clearPanel.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            TogglePause();
    }

    private void HandleExpFull()
    {
        if (currentState == GameState.Wave1)
        {
            ChangeState(GameState.ItemSelect1);
        }
        else if (currentState == GameState.Wave2)
        {
            ChangeState(GameState.ItemSelect2);
        }
        else if (currentState == GameState.Wave3)
        {
            ChangeState(GameState.ItemSelect3);
        }
        else if (currentState == GameState.Wave4)
        {
            ChangeState(GameState.Clear);
        }
    }

    private void TogglePause()
    {
        if (currentState == GameState.GameOver) return;
        if (currentState == GameState.Clear) return;

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
            case GameState.Wave1:
                StartWave1();
                break;

            case GameState.ItemSelect1:
                OpenItemSelect1();
                break;

            case GameState.Wave2:
                StartWave2();
                break;

            case GameState.ItemSelect2:
                OpenItemSelect2();
                break;

            case GameState.Wave3:
                StartWave3();
                break;

            case GameState.ItemSelect3:
                OpenItemSelect3();
                break;

            case GameState.Wave4:
                StartWave4();
                break;

            case GameState.Clear:
                Clear();
                break;

            case GameState.GameOver:
                GameOver();
                break;
        }
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

            currentChoices.Add(selectedItem);
            candidates.RemoveAt(randomIndex);

            itemTitleTexts[i].text = selectedItem.itemName;
            itemDescriptionTexts[i].text = selectedItem.description;
        }
    }

    public void CollectGoalItem()
    {
        if (currentState != GameState.Wave1) return;

        collectedItemCount++;
        Debug.Log($"Goal Item: {collectedItemCount}/{requiredItemCount}");

        // TMP: 임시
        // if (collectedItemCount >= requiredItemCount)
        // {
        //     ChangeState(GameState.ItemSelect1);
        // }
    }

    private void StartWave1()
    {
        Time.timeScale = 1f;
        player.SetControlEnabled(true);
        collectedItemCount = 0;
        Debug.Log("Wave 1 Start");
    }

    private void StartWave2()
    {
        Time.timeScale = 1f;
        player.SetControlEnabled(true);
        Debug.Log("Wave 2 Start");
    }
    private void StartWave3()
    {
        Time.timeScale = 1f;
        player.SetControlEnabled(true);
        Debug.Log("Wave 3 Start");
    }

    private void StartWave4()
    {
        Time.timeScale = 1f;
        player.SetControlEnabled(true);
        Debug.Log("Wave 4 Start");
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

    private void OpenItemSelect1()
    {
        Debug.Log("Item Select 1");
        OpenItemSelection(ItemPhase.Base);
    }

    private void OpenItemSelect2()
    {
        Debug.Log("Item Select 2");
        OpenItemSelection(ItemPhase.Base);
    }
    private void OpenItemSelect3()
    {
        Debug.Log("Item Select 3");
        OpenItemSelection(ItemPhase.Upgrade);
    }

    public void SelectItem(int index)
    {
        if (index < 0 || index >= currentChoices.Count)
        {
            return;
        }

        ItemData selectedItem = currentChoices[index];

        ApplyItem(selectedItem);

        if (playerStats != null)
            playerStats.ResetExp();

        itemSelectionPanel.SetActive(false);

        if (currentState == GameState.ItemSelect1)
        {
            ChangeState(GameState.Wave2);
        }
        else if (currentState == GameState.ItemSelect2)
        {
            ChangeState(GameState.Wave3);
        }
        else if (currentState == GameState.ItemSelect3)
        {
            ChangeState(GameState.Wave4);
        }
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
