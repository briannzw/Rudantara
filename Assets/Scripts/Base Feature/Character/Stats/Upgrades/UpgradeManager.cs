using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kryz.CharacterStats;


public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance; 

    public UpgradeDatabase upgradeDatabase;

    private Dictionary<int, Upgrade> selectedUpgrades = new Dictionary<int, Upgrade>();

    private StatModifier statModifier;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        LoadSelectedUpgrades();
    }

    public void SaveSelectedUpgrades()
    {
        foreach (var kvp in selectedUpgrades)
        {
            PlayerPrefs.SetInt(kvp.Key.ToString(), kvp.Value.upgradeID);
        }
        PlayerPrefs.Save();
    }

    public void LoadSelectedUpgrades()
    {
        selectedUpgrades.Clear();
        foreach (Upgrade upgrade in upgradeDatabase.commonUpgrades)
        {
            if (PlayerPrefs.HasKey(upgrade.upgradeID.ToString()))
            {
                int upgradeID = PlayerPrefs.GetInt(upgrade.upgradeID.ToString());
                Upgrade selectedUpgrade = upgradeDatabase.GetUpgradeByID(upgradeID);
                if (selectedUpgrade != null)
                {
                    selectedUpgrades.Add(upgradeID, selectedUpgrade);
                }
            }
        }

        foreach (Upgrade upgrade in upgradeDatabase.rareUpgrades)
        {
            if (PlayerPrefs.HasKey(upgrade.upgradeID.ToString()))
            {
                int upgradeID = PlayerPrefs.GetInt(upgrade.upgradeID.ToString());
                Upgrade selectedUpgrade = upgradeDatabase.GetUpgradeByID(upgradeID);
                if (selectedUpgrade != null)
                {
                    selectedUpgrades.Add(upgradeID, selectedUpgrade);
                }
            }
        }
  }

    public void AddSelectedUpgrade(Upgrade upgrade)
    {
        selectedUpgrades[upgrade.upgradeID] = upgrade;
        SaveSelectedUpgrades();
    }

    public Upgrade GetSelectedUpgrade(int upgradeID)
    {
        if (selectedUpgrades.ContainsKey(upgradeID))
        {
            return selectedUpgrades[upgradeID];
        }
        return null;
    }

     public void ApplyUpgradesToCharacter()
    {
        foreach (var kvp in selectedUpgrades)
        {
            Upgrade upgrade = kvp.Value;
        }
    }
}
