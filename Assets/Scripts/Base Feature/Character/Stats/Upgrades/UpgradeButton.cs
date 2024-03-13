using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kryz.CharacterStats;

public class UpgradeButton : MonoBehaviour
{
    public UpgradeRandomizer upgradeRandomizer;
    private GraphicRaycaster canvasRaycast;
    public Character character;

    public Image hoverImage;
    private UpgradeManager upgradeManager;
    private UpgradeList upgradeList;
    private Upgrade upgrade;
    public static int id = 1;
    private RectTransform rectTransform;
    [SerializeField] private float hoverHighValue = 10f;


    void Start()
    {
        upgradeManager = FindObjectOfType<UpgradeManager>();
        rectTransform = GetComponent<RectTransform>();
        upgradeList = FindObjectOfType<UpgradeList>();

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
            Debug.Log(upgrade);
            for(int i = 0;i<upgrade.stats.Length;i++ ){
                character.ModifyStat(upgrade.stats[i].upgradeEnum, new StatModifier(upgrade.stats[i].upgradeValueStatic, upgrade.stats[i].statModType));
                Debug.Log("upgrade enum: "+upgrade.stats[i].upgradeEnum);
                Debug.Log("Upgrade value: "+upgrade.stats[i].upgradeValueStatic);
                Debug.Log("Upgrade type: "+ upgrade.stats[i].statModType);
            }            
            
            //disable raycast wile animate
            canvasRaycast.enabled = false;

            Invoke(nameof(SetFalseUpgradeCanvas), 1f);

            //enable canvas again after animation finished
            Invoke(nameof(EnableCanvasAfterDelay), 1f);

            Time.timeScale = 1f;
        }
    }
    void EnableCanvasAfterDelay() {
        canvasRaycast.enabled = true;
        Debug.Log("canvas activated again");
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
