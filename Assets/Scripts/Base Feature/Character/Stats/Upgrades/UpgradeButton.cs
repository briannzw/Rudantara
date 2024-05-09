using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kryz.CharacterStats;

public class UpgradeButton : MonoBehaviour
{
    public UpgradeRandomizer upgradeRandomizer;
    [SerializeField] private UpgradeOwnedUI upgradeOwnedUI;
    private GraphicRaycaster canvasRaycast;
     [SerializeField] private Character character;

    public Image hoverImage;
    private UpgradeManager upgradeManager;
    [SerializeField] private UpgradeList upgradeList;
    private Upgrade upgrade;
    public static int id = 1;
    private RectTransform rectTransform;
    [SerializeField] private float hoverHighValue = 10f;


    void Start()
    {
        upgradeManager = FindObjectOfType<UpgradeManager>();
        rectTransform = GetComponent<RectTransform>();
        upgradeList = FindObjectOfType<UpgradeList>();
        character = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();

        canvasRaycast= GetComponentInParent<GraphicRaycaster>();

        if(upgradeManager == null)
        {
            Debug.Log("Upgrade not found");
            return;
        }
    }
    
    public void SetUpgrade(Upgrade Upgrade)
    {
        upgrade = Upgrade;
    }

    public void OnButtonClick()
    {
        if (upgrade != null)
        {
            upgradeList.chosenUpgrades.Add(upgrade);
            for(int i = 0;i<upgrade.stats.Length;i++ ){                character.ModifyStat(upgrade.stats[i].upgradeEnum, new StatModifier(upgrade.stats[i].upgradeValueStatic, upgrade.stats[i].statModType));
            }            
            
            canvasRaycast.enabled = false;

            Invoke(nameof(SetFalseUpgradeCanvas), 1f);

            Invoke(nameof(EnableCanvasAfterDelay), 1f);
            upgradeOwnedUI.UpdateBuffDisplay(UpgradeList.Instance.chosenUpgrades);

            Time.timeScale = 1f;
        }
    }
    void EnableCanvasAfterDelay() {
        canvasRaycast.enabled = true;
    }

    void SetFalseUpgradeCanvas() {
        upgradeRandomizer.gameObject.SetActive(false);
    }

    private string GetUpgradeDescription(Upgrade upgrade)
    {
        string description = "";
        foreach (var stat in upgrade.stats)
        {
            if(stat.upgradeValueStatic > 0)
            {
                description += stat.upgradeEnum.ToString() + " +" + stat.upgradeValueStatic + "\n";
            }
            else if(stat.upgradeValueStatic < 0)
            {
                description += stat.upgradeEnum.ToString() + " -" + stat.upgradeValueStatic + "\n";
            }
        }

        if(upgrade.upgradeDesc != null)
        {
            description += upgrade.upgradeDesc;
        }
        return description;
    }
}
