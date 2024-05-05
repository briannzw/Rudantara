using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRandomDrop : MonoBehaviour
{
    public Character character;
    [SerializeField] private PlayerUpgrades playerUpgrades;
    // LCG parameters
    public UpgradeDatabase upgradeDatabase;
    int firstSeed; 
    [SerializeField] bool firstInit = false;
    [SerializeField] int seed;
    [SerializeField] int m;
    [SerializeField] int a;
    [SerializeField] int c;
    [SerializeField] int higherChance = 0;
    [SerializeField] private GameObject randomDrop;

    void Awake(){
        
        playerUpgrades = FindObjectOfType<PlayerUpgrades>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    int LCG(int seed, int a, int c, int m){
        return ((a * seed + c) % m);
    } 

     public void RandomizeRandomDrop(Vector3 randomPos, Quaternion quaternion){
        m = upgradeDatabase.commonUpgrades.Count + upgradeDatabase.rareUpgrades.Count;   // modulus
        a = upgradeDatabase.commonUpgrades.Count;
        // Multiplier
        c = upgradeDatabase.rareUpgrades.Count; // Increment
   
        if (!firstInit){
            firstSeed = (int)System.DateTime.Now.Ticks;
            seed = LCG(firstSeed, a, c, m);
            firstInit = true;
        }
        else{
            seed = LCG(seed, a, c, m);
        }
        
        if (seed < 0){
            seed = -seed;
        }
        
        Debug.Log("Seed random drop:"+seed);

        float dropChance = (upgradeDatabase.commonUpgrades.Count + upgradeDatabase.rareUpgrades.Count) / 4; 

        bool drop = (seed - higherChance) < dropChance;

        if (drop)
        {
            Instantiate(randomDrop, randomPos, quaternion);
            higherChance = 0;
        }
        else{
            higherChance++;
        }
    }
}
