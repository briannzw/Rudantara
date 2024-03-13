using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using Unity.VisualScripting;

public class UpgradeRandomizer : MonoBehaviour
{
    public static UpgradeRandomizer Instance { get; private set; }
    public UpgradeDatabase upgradeDatabase;
    public UpgradeManager upgradeManager;
    public TMP_Text[] upgradeNameText;
    public TMP_Text[] upgradeDescText;
    public Image[] upgradeBgColor;
    public Button[] upgradeButtons;

    private List<Upgrade> availableUpgrades = new List<Upgrade>();
    [SerializeField] List<Upgrade> randomizedUpgrades = new List<Upgrade>();
    private Dictionary<UpgradeRarity, int> rarityCounts = new Dictionary<UpgradeRarity, int>();

    void Start()
    {

        StartUpgrade();
    }

    public void TestTrigger()
    {
        if (Input.GetKeyDown("space"))
        {
            StartUpgrade();
        }
    }

    public void StartUpgrade()
    {
        availableUpgrades.Clear();
        randomizedUpgrades.Clear();
        availableUpgrades.AddRange(upgradeDatabase.commonUpgrades);
        availableUpgrades.AddRange(upgradeDatabase.rareUpgrades);

        InitializeRarityCounts();
        RandomizeUpgradesWithLCG();
        UpdateUI();
    }

    private void InitializeRarityCounts()
    {
        rarityCounts.Clear();

        foreach (var rarity in System.Enum.GetValues(typeof(UpgradeRarity)))
        {
            rarityCounts[(UpgradeRarity)rarity] = 0;
            
        }

        foreach (var upgrade in availableUpgrades)
        {
            rarityCounts[upgrade.rarity]++;
        }
    }

/*
   public void RandomizeUpgrades()
    {
    randomizedUpgrades.Clear();

    if (rarityCounts[UpgradeRarity.Common] < 1 || rarityCounts[UpgradeRarity.Rare] < 1)
    {
        Debug.Log("Not enough common or rare upgrades available.");
        return;
    }

    for (int i = 0; i < 3; i++)
    {
        UpgradeRarity selectedRarity = GetRandomRarityWithWeightedDistribution();
        Upgrade selectedUpgrade = GetRandomUpgrade(selectedRarity);

        while (randomizedUpgrades.Contains(selectedUpgrade))
        {
            selectedUpgrade = GetRandomUpgrade(selectedRarity);
        }

        randomizedUpgrades.Add(selectedUpgrade);
    }
}

    private UpgradeRarity GetRandomRarityWithWeightedDistribution()
    {
        /*Amount Randomizer
        int commonAmount = 0;
        int rareAmount = 0;
        int legendAmount = 0;
    
        foreach(var upgrades in upgradeDatabase.commonUpgrades)
        {
            commonAmount += 6;
        }

        foreach(var upgrades in upgradeDatabase.rareUpgrades)
        {
            rareAmount += 3;
        }

        foreach(var upgrades in upgradeDatabase.legendUpgrades)
        {
            legendAmount += 1;
        }

        int totalAmount = commonAmount + rareAmount + legendAmount;

        int randVal = Random.Range(1, totalAmount);

        if(randVal >= 1 && randVal <= commonAmount)
        {
            return UpgradeRarity.Common;
        }
        else if(randVal > commonAmount && randVal <= (commonAmount + rareAmount))
        {
            return UpgradeRarity.Rare;
        }
        else 
        {
            return UpgradeRarity.Legendary;
        }
        
        
        //|| Percent randomizer;
        float randomValue = Random.Range(0f, 1f);
        
        // 60% chance for common, 30% chance for rare, 10% change for legend
        if (randomValue <= 0.60f)
        {
            return UpgradeRarity.Common;
        }
        else if (randomValue > 0.60f && randomValue <= 1f)
        {
            return UpgradeRarity.Rare;
        }
        else
        {
            return UpgradeRarity.Common;
        }
    }
*/
    private Upgrade GetRandomUpgrade(UpgradeRarity rarity, long seed)
    {
         List<Upgrade> eligibleUpgrades = availableUpgrades.FindAll(upgrade => upgrade.rarity == rarity);
 
    if (eligibleUpgrades.Count > 0)
    {
        int randomIndex = (int)(seed % eligibleUpgrades.Count);
        return eligibleUpgrades[randomIndex];
    }

        return null;
    }


   private void RandomizeUpgradesWithLCG()
{
    // LCG parameters
    long m = availableUpgrades.Count; // Modulus
    long a = upgradeDatabase.commonUpgrades.Count;    // Multiplier
    long c = upgradeDatabase.rareUpgrades.Count; // Increment
    long seed = System.DateTime.Now.Ticks % m; 
    Debug.Log("Modulus: "+m);
    Debug.Log("Pengali: "+a);
    Debug.Log("Penambah: "+c);
    Debug.Log("Seed: "+seed);

    for (int i = 0; i < 3; i++)
    {
        seed = (a * seed + c) % m;

        Debug.Log(seed); 

        UpgradeRarity selectedRarity = GetRarityFromLCG(seed);

        Upgrade selectedUpgrade = GetRandomUpgrade(selectedRarity, seed);

        while (randomizedUpgrades.Contains(selectedUpgrade))
        {
            seed = (a * seed + c) % m;
            selectedRarity = GetRarityFromLCG(seed);
            selectedUpgrade = GetRandomUpgrade(selectedRarity, seed);
        }

        randomizedUpgrades.Add(selectedUpgrade);
    }
}

private UpgradeRarity GetRarityFromLCG(long seed)
{
    float normalized = (float)seed % availableUpgrades.Count;

    if (normalized <= upgradeDatabase.commonUpgrades.Count)
    {
        return UpgradeRarity.Common;
    }
    else
    {
        return UpgradeRarity.Rare;
    }
}


    public void UpdateUI()
    {
        for (int i = 0; i < 3; i++)
        {
            upgradeNameText[i].text = randomizedUpgrades[i].upgradeName;
            
            if (randomizedUpgrades[i].rarity == UpgradeRarity.Common)
            {
                //upgradeNameText[i].color = new Color(0.545f, 0.761f, 0.808f); // Cyan color
                 upgradeBgColor[i].color = new Color(0.545f, 0.761f, 0.808f);
                 }
            else if (randomizedUpgrades[i].rarity == UpgradeRarity.Rare)
            {
                //upgradeNameText[i].color = new Color(0.518f, 0.157f, 0.741f); // Purple color
                upgradeBgColor[i].color = new Color(0.518f, 0.157f, 0.741f);
                   }
            else
            {
                //upgradeNameText[i].color = Color.white;
                upgradeBgColor[i].color = Color.white;
                  }
            upgradeNameText[i].color = Color.white;
            upgradeDescText[i].text = GetUpgradeDescription(randomizedUpgrades[i]);
            
            UpgradeButton upgradeButton = upgradeButtons[i].GetComponent<UpgradeButton>();
            upgradeButton.SetUpgrade(randomizedUpgrades[i]);
            Debug.Log(randomizedUpgrades[i]);

            upgradeButtons[i].interactable = true;

            foreach (var upgrade in randomizedUpgrades)
            {
                upgradeManager.AddSelectedUpgrade(upgrade);
            }
        }
    }

    private string GetUpgradeDescription(Upgrade upgrade)
    {
        string description = "";

        if(upgrade.upgradeDesc != null)
        {
            description = upgrade.upgradeDesc;
        }
        
        return description;
    }

}
