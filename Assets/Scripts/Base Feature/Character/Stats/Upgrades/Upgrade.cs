using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kryz.CharacterStats;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Upgrade")]
public class Upgrade : ScriptableObject
{
    public string upgradeName;
   public int upgradeID;
   public Sprite upgradeIcon;
   
   public string upgradeDesc;

   public UpgradeRarity rarity;
    public UpgradeStats[] stats;
}

[System.Serializable]
public class UpgradeStats
{
    public float upgradeValueStatic;
    public int upgradeLimitDown;
    public int upgradeLimitUp;
    public StatEnum upgradeEnum;
    public StatModType statModType;
}

public enum UpgradeRarity
{
    Common,
    Rare
}

