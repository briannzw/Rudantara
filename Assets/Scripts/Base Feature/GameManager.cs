using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager instance;

    public UpgradeUIManager upgradeUIManager;

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
            return;
        }

        instance = this;
    }
    #endregion
}
