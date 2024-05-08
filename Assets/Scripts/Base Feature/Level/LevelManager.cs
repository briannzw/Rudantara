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
        playerLeveling.SetInitialLevel(saveManager.SaveData.PlayerExp);
        companionLeveling.SetInitialLevel(saveManager.SaveData.CompanionExp);

        // Check Player and Companion
        playerLeveling.GetComponent<Character>().OnCharacterDie += (Character chara) =>
        {
            playerDied = true;
            if (companionDied) GameOver();
        };
        companionLeveling.GetComponent<Character>().OnCharacterDie += (Character chara) =>
        {
            companionDied = true;
            if (playerDied) GameOver();
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
        GameManager.Instance.NewGame();
        Time.timeScale = 1f;
        sceneLoader.LoadScene("MainMenu");
    }

    public void NextIteration()
    {
        if (CurrentIteration >= enemyBaseLevels.Count) return;

        CurrentIteration++;
        saveManager.SaveData.Iteration = CurrentIteration;
        saveManager.SaveData.PlayerExp = playerLeveling.TotalExperiences;
        saveManager.SaveData.CompanionExp = companionLeveling.TotalExperiences;

        saveManager.Save();
        sceneLoader.LoadScene(SceneManager.GetActiveScene().name);
    }
}