using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkillController : PlayerInputControl
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Character character;
    [SerializeField] private PlayerAutoTarget autoTarget;

    [Header("Parameters")]
    [SerializeField] private List<Skill> Skills = new();
    private List<float> SkillCooldowns = new();

    public Skill GetSkill(int skillIndex) => Skills[skillIndex];
    public float GetCooldown(int skillIndex) => SkillCooldowns[skillIndex];

    private void Awake()
    {
        for (int i = 0; i < Skills.Count; i++) SkillCooldowns.Add(0f);
    }

    private void Update()
    {
        for (int i = 0; i < SkillCooldowns.Count; i++) if(SkillCooldowns[i] > 0f) SkillCooldowns[i] -= Time.deltaTime;
    }

    protected override void RegisterInputCallbacks()
    {
        if (playerControls == null) return;
        playerControls.Gameplay.Skill1.performed += OnSkill1;
        playerControls.Gameplay.Skill2.performed += OnSkill2;
    }

    protected override void UnregisterInputCallbacks()
    {
        if (playerControls == null) return;
        playerControls.Gameplay.Skill1.performed -= OnSkill1;
        playerControls.Gameplay.Skill2.performed -= OnSkill2;
    }

    private void OnSkill1(InputAction.CallbackContext context)
    {
        CastSkill(0);
    }

    private void OnSkill2(InputAction.CallbackContext context)
    {
        CastSkill(1);
    }

    private void CastSkill(int index)
    {
        if (!playerControls.Gameplay.Attack.enabled) return;
        if (!CheckMana(Skills[index].ManaRequired)) return;
        if (SkillCooldowns[index] > 0f) return;
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Blend Tree")) return;

        autoTarget.FaceTarget();

        if (Skills[index].Loop > 0)
            animator.SetInteger(Skills[index].Trigger, Skills[index].Loop);
        else
            animator.SetTrigger(Skills[index].Trigger);

        character.ChangeDynamicValue(DynamicStatEnum.Mana, -Skills[index].ManaRequired);
        SkillCooldowns[index] = Skills[index].Cooldown;
    }

    private bool CheckMana(int amountNeeded)
    {
        return amountNeeded <= character.CheckStat(DynamicStatEnum.Mana);
    }
}
