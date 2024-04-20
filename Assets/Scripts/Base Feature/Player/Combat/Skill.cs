using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Skill", fileName = "New Skill")]
public class Skill : ScriptableObject
{
    public string Name;
    public string Trigger;
    public int Loop = 0;
    public int ManaRequired;
    public float Cooldown = 10;
    public int AttackPercent;
}
