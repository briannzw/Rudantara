using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using Unity.VisualScripting;

using Kryz.CharacterStats;

public class UpgradeRandomizer : MonoBehaviour
{
    public static UpgradeRandomizer Instance { get; private set; }
    public UpgradeDatabase upgradeDatabase;
    public UpgradeManager upgradeManager;
    public TMP_Text[] upgradeNameText;
    public TMP_Text[] upgradeDescText;
    public Image[] upgradeBgColor;
    public Image[] upgradeImage;
    public Button[] upgradeButtons;

    private bool seedInitialized = false;     
    private bool upgradeSeedInitialized = false; 


    private List<Upgrade> availableUpgrades = new List<Upgrade>();
    [SerializeField] List<Upgrade> randomizedUpgrades = new List<Upgrade>();
    private Dictionary<UpgradeRarity, int> rarityCounts = new Dictionary<UpgradeRarity, int>();

    // LCG parameters
    [SerializeField] int firstSeed;
    [SerializeField] int seed;
    [SerializeField] int m;
    [SerializeField] int a;
    [SerializeField] int c; 

    void Start()
    {
        
    }

    public void TestTrigger()
    {
        /*if (Input.GetKeyDown("space"))
        {
            StartUpgrade();
        }*/
    }

    private int LCG(int a, int c, int m, int seed)
    {
        return (a * seed + c) % m;
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

   private void RandomizeUpgradesWithLCG()
{
    //LCG Constant
    m = availableUpgrades.Count; // Modulus
    a = upgradeDatabase.commonUpgrades.Count;    // Multiplier
    c = upgradeDatabase.rareUpgrades.Count; // Increment

    for (int i = 0; i < 3; i++)
    {
        if (!seedInitialized)
        {
            firstSeed = (int)System.DateTime.Now.Ticks;
            seedInitialized = true;
            seed = LCG(a, c, m, firstSeed);
        }
        else
        {
            seed = LCG(a,c,m, seed);
        }
     
        if(seed < 0){
            seed = -seed;
        }

        Debug.Log("Seed ke-"+i+" pilih Upgrade :"+seed);

        UpgradeRarity rarity = GetRarityFromLCG(seed);

        Upgrade selectedUpgrade = availableUpgrades[seed];

        while (randomizedUpgrades.Contains(selectedUpgrade))
        {
            seed = LCG(a, c, m, seed);
            Debug.Log("Seed ke-"+i+" pilih Upgrade ulang karena sama:"+seed);
            selectedUpgrade = availableUpgrades[seed];
        }

        foreach (var stat in selectedUpgrade.stats){
            seed = LCG(a, c, m, seed);
            Debug.Log("Seed ke-"+i+" random stats :"+seed);
            
            if(stat.upgradeLimitUp >= 0){
                stat.upgradeValueStatic = (seed % (stat.upgradeLimitUp+1));   
            }                       
        
            if(stat.upgradeValueStatic < stat.upgradeLimitDown){
                stat.upgradeValueStatic = stat.upgradeLimitDown;
            } 

            Debug.Log("selected upgrade :"+selectedUpgrade.upgradeName);
        }

        
        randomizedUpgrades.Add(selectedUpgrade);
    }
}

    private UpgradeRarity GetRarityFromLCG(int seed)
    {
        if (seed <= upgradeDatabase.commonUpgrades.Count)
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
            if (i < randomizedUpgrades.Count)
            {
                upgradeNameText[i].text = randomizedUpgrades[i].upgradeName;

                if (randomizedUpgrades[i].rarity == UpgradeRarity.Common)
                {
                    upgradeBgColor[i].color = new Color(0.545f, 0.761f, 0.808f);
                }
                else if (randomizedUpgrades[i].rarity == UpgradeRarity.Rare)
                {
                    upgradeBgColor[i].color = new Color(0.518f, 0.157f, 0.741f);
                }
                else
                {
                    upgradeBgColor[i].color = Color.white;
                }
                upgradeNameText[i].color = Color.white;
                upgradeDescText[i].text = GetUpgradeDescription(randomizedUpgrades[i]);

                upgradeImage[i].sprite = randomizedUpgrades[i].upgradeIcon;
                upgradeImage[i].preserveAspect = true;

                UpgradeButton upgradeButton = upgradeButtons[i].GetComponent<UpgradeButton>();
                upgradeButton.SetUpgrade(randomizedUpgrades[i]);

                upgradeButtons[i].interactable = true;

                upgradeManager.AddSelectedUpgrade(randomizedUpgrades[i]);
            }
            else
            {
                upgradeNameText[i].text = "";
                upgradeBgColor[i].color = Color.white;
                upgradeNameText[i].color = Color.white;
                upgradeDescText[i].text = "";
                upgradeImage[i].sprite = null;
                upgradeButtons[i].interactable = false;
            }
        }
    }

    private string GetUpgradeDescription(Upgrade upgrade)
    {
        string description = "";

        if(upgrade.upgradeDesc != null)
        {
            description = upgrade.upgradeDesc + "\n";

             foreach (var stat in upgrade.stats)
            {
                description += $"{stat.upgradeEnum}: {stat.upgradeValueStatic} ({stat.upgradeLimitDown} - {stat.upgradeLimitUp})\n";
            }
        }
        
        return description;
    }

}
