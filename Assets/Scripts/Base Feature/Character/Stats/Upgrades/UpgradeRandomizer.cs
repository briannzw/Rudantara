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
    [SerializeField] private UpgradeManager upgradeManager;
    public TMP_Text[] upgradeNameText;
    public TMP_Text[] upgradeDescText;
    public Image[] upgradeBgColor;
    public Image[] upgradeImage;
    public Button[] upgradeButtons;

    public CharacterLeveling playerLevel;
    public int currentPlayerLevel;

    private bool seedInitialized = false;     
    private bool upgradeSeedInitialized = false; 
    private bool statSeedInitialized = false; 


    private List<Upgrade> availableUpgrades = new List<Upgrade>();
    [SerializeField] List<Upgrade> randomizedUpgrades = new List<Upgrade>();
    private Dictionary<UpgradeRarity, int> rarityCounts = new Dictionary<UpgradeRarity, int>();

    // LCG parameters
    [SerializeField] long firstSeed;
    [SerializeField] long seed;
    [SerializeField] long m;
    [SerializeField] long a;
    [SerializeField] long c;
    [SerializeField] int upgradeSeed;
    [SerializeField] int statSeed;

    void Start()
    {
        upgradeManager = FindObjectOfType<UpgradeManager>();
        currentPlayerLevel = playerLevel.CurrentLevel;
    }

    void Update()
    {
        if (playerLevel.CurrentLevel != currentPlayerLevel)
        {
            StartUpgrade();
            currentPlayerLevel = playerLevel.CurrentLevel;
        }
    }

    private long LCG(long a, long c, long m, long seed)
    {
            return ((a * seed) + c) % m;
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

            UpgradeRarity rarity = GetRarityFromLCG();
            Debug.Log("rarity ke-"+i+" : "+rarity);

            Upgrade selectedUpgrade = GetSelectedUpgrade(rarity);
         
            Debug.Log("selected upgrade :"+selectedUpgrade.upgradeName); 

            /*while (randomizedUpgrades.Contains(selectedUpgrade))
            {
                seed = LCG(a, c, m, seed);
                Debug.Log("Seed ke-"+i+" pilih Upgrade ulang karena sama:"+seed);
                selectedUpgrade = GetSelectedUpgrade(rarity);
                Debug.Log("Upgrade ke-"+i+" mengulang :"+selectedUpgrade.upgradeName);

            }*/

            foreach (var stat in selectedUpgrade.stats){
                if(statSeed < 0){
                    statSeed = -statSeed;
                }
                Debug.Log("Seed ke-"+i+" random stats :"+statSeed);
                
                if(!statSeedInitialized){
                    statSeed = (int)LCG(a, c, (stat.upgradeLimitUp+1), seed);
                }
                else{
                    statSeed = (int)LCG(a, c, (stat.upgradeLimitUp+1), statSeed);
                }
                     
                stat.upgradeValueStatic = statSeed;

                if(stat.upgradeLimitDown > 0){
                    if(stat.upgradeValueStatic < stat.upgradeLimitDown){
                        stat.upgradeValueStatic += stat.upgradeLimitDown;
                    }
                }
                else if (stat.upgradeLimitDown < 0){
                    if(stat.upgradeValueStatic > stat.upgradeLimitUp){
                        stat.upgradeValueStatic = stat.upgradeValueStatic % stat.upgradeLimitDown;
                        stat.upgradeValueStatic += stat.upgradeLimitUp;
                    }
                } 

                if (stat.statModType == StatModType.PercentAdd){
                    stat.upgradeValueStatic = stat.upgradeValueStatic / 100;
                }
            }

            randomizedUpgrades.Add(selectedUpgrade);
            foreach (var stat in selectedUpgrade.stats){
               Debug.Log("Selected upgrade stats of "+selectedUpgrade.upgradeName+": "+stat.upgradeValueStatic);
            }
        }
    }

    private Upgrade GetSelectedUpgrade(UpgradeRarity rarity){
        List<Upgrade> eligibleUpgrades = availableUpgrades.FindAll(upgrade => upgrade.rarity == rarity);

        if (eligibleUpgrades.Count > 0)
        {
            if(!upgradeSeedInitialized){
                upgradeSeed = (int)LCG(a, c, m, seed);
            }
            else{
                upgradeSeed = (int)LCG(a, c, m, upgradeSeed);     
            }
            Debug.Log("Upgrade seed sebelum dibagi eligible upgrades ="+upgradeSeed);
            upgradeSeed = upgradeSeed % eligibleUpgrades.Count;
            Debug.Log("Upgrade seed sesudah dibagi eligible upgrades ="+upgradeSeed);
            return eligibleUpgrades[upgradeSeed];
        }

        return null;
    }

    private UpgradeRarity GetRarityFromLCG()
    {
        if (seed < upgradeDatabase.commonUpgrades.Count)
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
