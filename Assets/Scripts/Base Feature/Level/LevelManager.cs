using Save;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private PauseController pauseController;
    [SerializeField] private CharacterLeveling playerLeveling;
    [SerializeField] private CharacterLeveling companionLeveling;

    [Header("UI References")]
    [SerializeField] private GameObject nextLevelPanel;
    [SerializeField] private TMP_Text nextLevelIterationText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Minimap minimap;

    [Header("Parameters")]
    // Enemy base level
    [SerializeField] private List<int> enemyBaseLevels = new();

    private HashSet<Character> spawnedEnemies = new();

    private bool playerDied = false;
    private bool companionDied = false;

    // Iteration Number
    public int CurrentIteration { get; set; }

    public int CurrentEnemyBaseLevel => enemyBaseLevels[CurrentIteration - 1];

    private void Start()
    {
        playerLeveling.SetInitialExp(saveManager.SaveData.PlayerExp);
        companionLeveling.SetInitialExp(saveManager.SaveData.CompanionExp);

        // Check Player and Companion
        playerLeveling.GetComponent<Character>().OnCharacterDie += (Character chara) =>
        {
            playerDied = true;
            GameOver();
        };
        companionLeveling.GetComponent<Character>().OnCharacterDie += (Character chara) =>
        {
            companionDied = true;
        };
    }

    private void GameOver()
    {
        Time.timeScale = 0f;
        InputManager.ToggleActionMap(InputManager.PlayerAction.Panel);
        pauseController.enabled = false;
        gameOverPanel.SetActive(true);
    }

    public void SetBoss(Character bossChara)
    {
        minimap.SetBoss(bossChara.transform);

        bossChara.OnCharacterDie += (value) =>
        {
            KillAll();
            InputManager.ToggleActionMap(InputManager.PlayerAction.Panel);
            pauseController.enabled = false;
            nextLevelIterationText.text = "ITERATION " + CurrentIteration + " CLEARED!";
            nextLevelPanel.SetActive(true);
        };
    }

    public void RegisterEnemy(Character chara)
    {
        if (spawnedEnemies.Contains(chara)) return;

        spawnedEnemies.Add(chara);
    }

    private void KillAll()
    {
        playerLeveling.ShareEXP = false;
        companionLeveling.ShareEXP = false;

        Character playerChara = playerLeveling.GetComponent<Character>();
        Character companionChara = companionLeveling.GetComponent<Character>();
        foreach(var enemy in spawnedEnemies)
        {
            playerChara.OnCharacterKill?.Invoke(enemy);
            companionChara.OnCharacterKill?.Invoke(enemy);
            enemy.ChangeDynamicValue(DynamicStatEnum.Health, -999999);
        }
    }

    public void Restart()
    {
        UpdateSaveData();
        GameManager.Instance.NewGame();
        Time.timeScale = 1f;
        sceneLoader.LoadScene("MainMenu");
    }

    public void NextIteration()
    {
        if (CurrentIteration >= enemyBaseLevels.Count) return;

        CurrentIteration++;

        GameManager.Instance.SaveGame();
    }

    public void UpdateSaveData()
    {
        saveManager.SaveData.Iteration = CurrentIteration;
        saveManager.SaveData.PlayerExp = playerLeveling.TotalExperiences;
        saveManager.SaveData.CompanionExp = companionLeveling.TotalExperiences;
    }
}