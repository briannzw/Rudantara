using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRandomDrop : MonoBehaviour
{
    public Character character;
    [SerializeField] private PlayerUpgrades playerUpgrades;
    // LCG parameters
    public UpgradeDatabase upgradeDatabase;
    long firstSeed = System.DateTime.Now.Ticks; 
    [SerializeField] bool firstInit = false;
    [SerializeField] float seed;
    [SerializeField] float m;
    [SerializeField] float a;
    [SerializeField] float c;
    [SerializeField] private GameObject randomDrop;

    void Awake(){
        
        playerUpgrades = FindObjectOfType<PlayerUpgrades>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     public void RandomizeRandomDrop(Vector3 randomPos, Quaternion quaternion){
        m = upgradeDatabase.commonUpgrades.Count + upgradeDatabase.rareUpgrades.Count;   // modulus
        a = upgradeDatabase.commonUpgrades.Count;
        // Multiplier
        c = upgradeDatabase.rareUpgrades.Count; // Increment
   
        if (!firstInit){
            seed = (a * firstSeed + c) % m;
            firstInit = true;
        }
        else{
            seed = (a * seed + c) % m; 
        }
        Debug.Log("LCG Dijalankan");

        float dropChance = (upgradeDatabase.commonUpgrades.Count + upgradeDatabase.rareUpgrades.Count) / 2; 

        bool drop = seed > dropChance;

        if (drop)
        {
            Instantiate(randomDrop, randomPos, quaternion);
        }
    }
}
