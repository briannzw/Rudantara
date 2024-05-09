using Save;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("There's more than one GameManager is detected.");
            Destroy(instance);
            return;
        }

        instance = this;

        if (!saveManager.CheckDataExists()) NewGame();
        saveManager.Load();
        LevelManager.CurrentIteration = saveManager.SaveData.Iteration;
    }
    #endregion

    [Header("References")]
    public UpgradeUIManager upgradeUIManager;
    public DungeonNavMesh DungeonNavMesh;
    public LevelManager LevelManager;
    public SaveManager saveManager;
    public BossHUD bossHUD;

    public void NewGame()
    {
        saveManager.SaveData.Iteration = 1;
        // saveManager.SaveData = new();
        saveManager.Save();
    }
}
