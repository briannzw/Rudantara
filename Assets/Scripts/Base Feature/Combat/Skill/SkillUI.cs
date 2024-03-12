using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{
    public int Index;
    public PlayerSkillController SkillController;
    [SerializeField] private Character character;
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text manaText;
    [SerializeField] private TMP_Text cooldownText;

    private Skill skill;

    private void Awake()
    {
        skill = SkillController.GetSkill(Index);
        manaText.text = skill.ManaRequired.ToString();

        manaText.color = (character.CheckStat(DynamicStatEnum.Mana) < skill.ManaRequired) ? Color.red : Color.white;

        character.OnCharacterDynamicStatsChanged += () =>
        {
            manaText.color = (character.CheckStat(DynamicStatEnum.Mana) < skill.ManaRequired) ? Color.red : Color.white;
        };
    }

    private void Update()
    {
        cooldownText.text = Mathf.RoundToInt(SkillController.GetCooldown(Index)).ToString();
        if (cooldownText.text == "0") cooldownText.text = "";
        fillImage.fillAmount = SkillController.GetCooldown(Index) / skill.Cooldown;
    }
}
