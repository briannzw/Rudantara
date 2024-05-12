using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpUpgrade : MonoBehaviour
{
    public UpgradeUIManager upgradeUIManager;
    public CharacterLeveling characterLeveling;

    private void OnEnable()
    {
        // Subscribe to the OnLevelUp event when enabled
    //    characterLeveling.OnLevelUp += PlayerLevelUp;
    }

    void PlayerLevelUp()
    {
        upgradeUIManager.TriggerUI();   
    }
}
