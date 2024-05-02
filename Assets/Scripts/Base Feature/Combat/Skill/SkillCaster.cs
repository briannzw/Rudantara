using UnityEngine;

public class SkillCaster : MonoBehaviour
{
    public Character Target { get => target; set { target = value; } }

    private Character target;

    public void Cast(Skill skill)
    {
        target.ChangeDynamicValue(DynamicStatEnum.Health, skill.AttackPercent);
        Instantiate(skill.VFX, target.transform);
    }
}
