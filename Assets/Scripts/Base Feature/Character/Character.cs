using AYellowpaper.SerializedCollections;
using Kryz.CharacterStats;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Character : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected SerializedDictionary<DynamicStatEnum, CharacterDynamicStat> DynamicStats = new();
    [SerializeField] protected SerializedDictionary<StatEnum, CharacterStat> Stats = new();
    
    [Header("Parameters")]
    [Cinemachine.TagField]
    public List<string> EnemyTags = new();

    [Header("Misc")]
    public Sprite characterImage;

    public bool IsDead { get => isDead; }
    private bool isDead = false;

    #region Events
    public Action OnCharacterHurt;
    public Action OnCharacterStatsChanged;
    public Action OnCharacterDynamicStatsChanged;

    public Action<Character> OnCharacterDie;
    public Action<Character> OnCharacterKill;
    #endregion

    private CharacterLeveling charLevel;

    // Constructor
    private Character() { InitializeStats(); }

    private void InitializeStats()
    {
        DynamicStats = new SerializedDictionary<DynamicStatEnum, CharacterDynamicStat>
            {
                { DynamicStatEnum.Health, new CharacterDynamicStat(100) },
                { DynamicStatEnum.Mana, new CharacterDynamicStat(20) }
            };
        Stats = new SerializedDictionary<StatEnum, CharacterStat>
            {
                { StatEnum.Attack, new CharacterStat(20) },
                { StatEnum.Defense, new CharacterStat(10) },
                { StatEnum.Speed, new CharacterStat(20) },
                { StatEnum.Accuracy, new CharacterStat(75) },
                { StatEnum.HealthRegen, new CharacterStat(3) },
                { StatEnum.ManaRegen, new CharacterStat(1) },
                { StatEnum.ExpMultiplier, new CharacterStat(1) },
            };
    }

    public void Initialize(CharacterLeveling charLevel)
    {
        this.charLevel = charLevel;
        DynamicStats = charLevel.InitialDynamicStats();
        Stats = charLevel.InitialStats();
    }

    public int GetLevel() => charLevel.CurrentLevel;

    public void ResetDynamicValue()
    {
        isDead = false;
        foreach(var dynamicStat in DynamicStats)
        {
            dynamicStat.Value.ResetCurrentValue();
        }
    }

    #region Getter
    // Check Stats
    public float CheckStat(StatEnum statEnum) => Stats[statEnum].Value;
    public float CheckStat(DynamicStatEnum dynamicStatEnum) => DynamicStats[dynamicStatEnum].CurrentValue;
    public float CheckStatMax(DynamicStatEnum dynamicStatEnum) => DynamicStats[dynamicStatEnum].Value;
    #endregion

    // Modify Stat value
    public void ModifyStat(StatEnum statEnum, StatModifier modifier)
    {
        Stats[statEnum].AddModifier(modifier);

        // Invoke Stats Changed
        OnCharacterStatsChanged?.Invoke();
    }

    // Modify Dynamic Stat max value (e.g. Max HP up, etc.)
    public void ModifyStat(DynamicStatEnum dynamicStatEnum, StatModifier modifier)
    {
        DynamicStats[dynamicStatEnum].AddModifier(modifier);

        // Invoke Stats Changed
        OnCharacterStatsChanged?.Invoke();

        // Invoke Stats Changed
        OnCharacterDynamicStatsChanged?.Invoke();
    }

    // Modify Dynamic Stat current value (e.g. Take Damage, etc.)
    public void ChangeDynamicValue(DynamicStatEnum dynamicStatEnum, float value)
    {
        DynamicStats[dynamicStatEnum].ChangeCurrentValue(value);

        // Check Character's HP
        if (dynamicStatEnum == DynamicStatEnum.Health)
        {
            // Hurt
            if (value < 0f) OnCharacterHurt?.Invoke();

            // Die
            if (DynamicStats[DynamicStatEnum.Health].CurrentValue <= 0f && !isDead)
            {
                OnCharacterDie?.Invoke(this);
                isDead = true;
            }
        }

        // Invoke Stats Changed
        OnCharacterDynamicStatsChanged?.Invoke();
    }

    public void RemoveModifierFromSource(object source)
    {
        foreach(var stat in Stats)
            stat.Value.RemoveAllModifiersFromSource(source);
        foreach(var dynamicStat in DynamicStats)
            dynamicStat.Value.RemoveAllModifiersFromSource(source);
    }

    public float GetDamage()
    {
        float damage = Stats[StatEnum.Attack].Value;

        return Random.Range(Stats[StatEnum.Accuracy].Value / 100f * damage, damage);
    }

    #region Dynamic Stats Regeneration
    private void Start()
    {
        InvokeRepeating(nameof(DynamicStatsRegen), 0f, 1f);
    }

    private void DynamicStatsRegen()
    {
        ChangeDynamicValue(DynamicStatEnum.Health, Stats[StatEnum.HealthRegen].Value);
        ChangeDynamicValue(DynamicStatEnum.Mana, Stats[StatEnum.ManaRegen].Value);
    }
    #endregion

    public string Describe(bool self = false)
    {
        Describable desc = GetComponentInChildren<Describable>();

        if (desc == null) return null;

        string description = $"Here are { (self ? "your" : desc.Name) } current stats:\n";

        description += $"Health Point: {Mathf.Round(CheckStat(DynamicStatEnum.Health))}/{CheckStatMax(DynamicStatEnum.Health)}";

        float percent = CheckStat(DynamicStatEnum.Health)/CheckStatMax(DynamicStatEnum.Health) * 100f;
        string info = (percent < 5f) ? "Really Low" : (percent < 20f) ? "Low" : (percent < 50f) ? "Medium" : (percent < 90f) ? "High" : "Full";

        description += " (" + info + " Health)";

        if(charLevel != null)
            description += $"Level: {charLevel.CurrentLevel}/100\n";

        return description;
    }
}
