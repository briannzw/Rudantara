using AYellowpaper.SerializedCollections;
using Kryz.CharacterStats;
using System.Collections;
using UnityEngine;

public class CharacterLeveling : MonoBehaviour
{
    [Header("References")]
    private Character character;

    [Header("Parameters")]
    [SerializeField] private int expPerSeconds;
    [SerializeField] private int expPerSecondsCombat;

    [Header("Distribution")]
    [SerializeField] private AnimationCurve expPerLevel;

    [Header("Stats")]
    [SerializeField] private CharacterStatLevelPattern statsPattern;

    public int CurrentLevel { get; private set; }
    private int expNeeded => Mathf.FloorToInt(expPerLevel.Evaluate(CurrentLevel + 1));
    public float Experiences { get; private set; }

    // Enemy Specific
    private AgentController enemyController;

    [Header("VFX")]
    [SerializeField] private GameObject levelUpVFX;

    private void OnEnable()
    {
        CurrentLevel = 1;
        Experiences = 0;
        // Assign stats to character based on level
        character.Initialize(this);
        character.ResetDynamicValue();
    }

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        StartCoroutine(ExpPerSecond());

        character.OnCharacterKill += (chara) =>
        {
            var charLevel = chara.GetComponent<CharacterLeveling>();
            if (charLevel) Experiences += Mathf.RoundToInt(charLevel.Experiences * StatsConst.KILL_EXP_MODIFIER * character.CheckStat(StatEnum.ExpMultiplier));
        };

        character.OnCharacterDie += (Character chara) => StopAllCoroutines();

        // Enemy only
        if(GetComponent<AgentController>())
            enemyController = GetComponent<AgentController>();
    }

    private IEnumerator ExpPerSecond()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (enemyController && enemyController.IsTargetInRange) Experiences += expPerSecondsCombat * character.CheckStat(StatEnum.ExpMultiplier);
            else Experiences += expPerSeconds * character.CheckStat(StatEnum.ExpMultiplier);

            if (CurrentLevel < 100 && Experiences >= expNeeded)
            {
                // Level Up
                CurrentLevel++;
                Experiences = 0;

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