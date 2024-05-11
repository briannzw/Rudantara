using AYellowpaper.SerializedCollections;
using Kryz.CharacterStats;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLeveling : MonoBehaviour
{
    [Header("References")]
    private Character character;
    [SerializeField] private List<CharacterLeveling> SharedEXP;

    [Header("Parameters")]
    [SerializeField] private int expPerSeconds;
    [SerializeField] private int expPerSecondsCombat;

    [Header("Distribution")]
    [SerializeField] private AnimationCurve expPerLevel;

    [Header("Stats")]
    [SerializeField] private CharacterStatLevelPattern statsPattern;

    public Action OnExperienceChanged;
    public bool ShareEXP = true;

    public int CurrentLevel { get; private set; }
    public int ExpNeeded => Mathf.FloorToInt(expPerLevel.Evaluate(CurrentLevel + 1));
    public float Experiences { get => Mathf.Clamp(experiences, 0, 999999); private set { experiences = value; OnExperienceChanged?.Invoke(); } }
    public float TotalExperiences
    { 
        get 
        {
            float temp = 0;
            for(int i = 1; i < CurrentLevel; i++)
                temp += Mathf.FloorToInt(expPerLevel.Evaluate(i + 1));
            temp += Experiences;
            return temp;
        }
    }

    private float experiences;

    // Enemy Specific
    private AgentController enemyController;

    [Header("VFX")]
    [SerializeField] private GameObject levelUpVFX;

    private void OnEnable()
    {
        Experiences = 0;
        ShareEXP = true;

        StartCoroutine(ExpPerSecond());

        character.OnCharacterKill += CharaKill;

        character.OnCharacterDie += (Character chara) => StopAllCoroutines();

        // Enemy only
        if (GetComponent<AgentController>())
            enemyController = GetComponent<AgentController>();
    }

    private void OnDisable()
    {
        character.OnCharacterKill -= CharaKill;
        character.OnCharacterDie -= (Character chara) => StopAllCoroutines();
    }

    public void Initialize(int level, float exp = 0)
    {
        CurrentLevel = level;
        Experiences = exp;
        // Assign stats to character based on level
        character.Initialize(this);
        character.ResetDynamicValue();
    }

    public void SetInitialExp(float exp)
    {
        CurrentLevel = 1;
        Experiences = exp;
        while (Experiences >= ExpNeeded)
        {
            Experiences -= ExpNeeded;
            CurrentLevel++;
            if (CurrentLevel >= 100) break;
        }

        character.Initialize(this);
        character.ResetDynamicValue();
    }

    private void Awake()
    {
        character = GetComponent<Character>();
        CurrentLevel = 1;
        Experiences = 0;
        ShareEXP = true;
        Initialize(CurrentLevel);
    }

    private IEnumerator ExpPerSecond()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (enemyController && enemyController.IsTargetInRange) Experiences += expPerSecondsCombat * character.CheckStat(StatEnum.ExpMultiplier);
            else Experiences += expPerSeconds * character.CheckStat(StatEnum.ExpMultiplier);

            if (CurrentLevel < 100 && Experiences >= ExpNeeded)
            {
                // Level Up
                CurrentLevel++;
                Experiences -= ExpNeeded;

                if (levelUpVFX)
                {
                    GameObject go =Instantiate(levelUpVFX, transform);
                    Destroy(go, 3f);
                }

                character.RemoveModifierFromSource(this);

                foreach (var statDist in statsPattern.statDistributions)
                {
                    character.ModifyStat(statDist.Key, new StatModifier(Mathf.Floor(statDist.Value.Evaluate(CurrentLevel) - character.CheckStat(statDist.Key)), StatModType.Flat, this));
                }
                foreach (var dynamicStatDist in statsPattern.dynamicStatDistributions)
                {
                    character.ModifyStat(dynamicStatDist.Key, new StatModifier(Mathf.Floor(dynamicStatDist.Value.Evaluate(CurrentLevel) - character.CheckStatMax(dynamicStatDist.Key)), StatModType.Flat, this));
                }
            }
        }
    }

    private void CharaKill(Character chara)
    {
        var charLevel = chara.GetComponent<CharacterLeveling>();
        if (charLevel)
        {
            Experiences += charLevel.CurrentLevel * StatsConst.KILL_EXP_MODIFIER * character.CheckStat(StatEnum.ExpMultiplier);
            if (ShareEXP) foreach (var leveling in SharedEXP) leveling.ReceiveSharedEXP(charLevel);
        }
    }

    public void ReceiveSharedEXP(CharacterLeveling charLevel)
    {
        Experiences += charLevel.CurrentLevel * StatsConst.KILL_EXP_MODIFIER * character.CheckStat(StatEnum.ExpMultiplier) * StatsConst.SHARED_EXP_MODIFIER;
    }

    // Initializer
    public SerializedDictionary<StatEnum, CharacterStat> InitialStats()
    {
        var stats = new SerializedDictionary<StatEnum, CharacterStat>();

        foreach(var stat in statsPattern.statDistributions)
        {
            if (stat.Key != StatEnum.ExpMultiplier)
                stats.Add(stat.Key, new CharacterStat(Mathf.Floor(stat.Value.Evaluate(CurrentLevel))));
            else
                stats.Add(stat.Key, new CharacterStat(stat.Value.Evaluate(CurrentLevel)));
        }

        return stats;
    }

    public SerializedDictionary<DynamicStatEnum, CharacterDynamicStat> InitialDynamicStats()
    {
        var stats = new SerializedDictionary<DynamicStatEnum, CharacterDynamicStat>();

        foreach(var stat in statsPattern.dynamicStatDistributions)
        {
            stats.Add(stat.Key, new CharacterDynamicStat(Mathf.Floor(stat.Value.Evaluate(CurrentLevel))));
        }

        return stats;
    }
}