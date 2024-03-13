using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade Database",menuName ="Upgrade Database")]
public class UpgradeDatabase : ScriptableObject
{
    public List<Upgrade> commonUpgrades = new List<Upgrade>();
    public List<Upgrade> rareUpgrades = new List<Upgrade>();  

    public Upgrade GetUpgradeByID(int id)
    {
        Upgrade upgrades = commonUpgrades.Find(upgrade => upgrade.upgradeID == id);
        if (upgrades = null)
        {
            upgrades = rareUpgrades.Find(upgrade => upgrade.upgradeID == id);
        }
        return upgrades;
    }

}

