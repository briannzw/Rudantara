using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUpgrades : MonoBehaviour
{
    
    public UpgradeUIManager upgradeUIManager;
    public int killCount;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void KillCountUp()
    {
        killCount++;
        if (killCount >= 1){
                upgradeUIManager.TriggerUI();
                killCount = 0;
        }
    }
}
