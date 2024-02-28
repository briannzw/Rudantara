using System;
using UnityEngine;
using Kryz.CharacterStats;

// Handler for Dynamic Stats (Stat that its value can decrease/increase with a maximum value, e.g. Health, Mana)
[Serializable]
public class CharacterDynamicStat : CharacterStat
{

    public float CurrentValue
    {
        get { return Mathf.Clamp(_currentValue, 0, Value); }
        private set { _currentValue = value; }
    }

    [SerializeField] protected float _currentValue;

    public CharacterDynamicStat()
    {
        _currentValue = BaseValue;
    }

    public CharacterDynamicStat(float baseValue) : this()
    {
        BaseValue = baseValue;
        _currentValue = BaseValue;
    }

    public override void AddModifier(StatModifier mod)
    {
        //float ratio = CurrentValue / Value;
        base.AddModifier(mod);
        //CurrentValue = (float)Math.Round(Value * ratio, 4);
    }

    public void ResetCurrentValue()
    {
        _currentValue = BaseValue;
    }

    public void ChangeCurrentValue(float value)
    {
        float finalValue = CurrentValue + value;
        CurrentValue = (float)Math.Round(finalValue, 4);
    }
}