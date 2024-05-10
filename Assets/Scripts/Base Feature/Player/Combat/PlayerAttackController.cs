using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackController : PlayerInputControl
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Character character;
    [SerializeField] private HitController weaponHitController;

    private int resistance;
    public Action<Transform> OnCombatEngaged;

    protected override void Start()
    {
        base.Start();

        resistance = StatsConst.HURT_RESISTANCE;
        character.OnCharacterHurt += () =>
        {
            // Resistance when animating attacks, etc.
            if (!animator.GetBool("CanMove")) return;

            resistance--;
            if (resistance <= 0)
            {
                animator.SetTrigger("Hurt");
                resistance = StatsConst.HURT_RESISTANCE;
            }
        };

        weaponHitController.OnHit += (other) => OnCombatEngaged?.Invoke(other);
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f || animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Blend Tree"))
        {
            animator.SetTrigger("Attack");
            weaponHitController.Name = "Basic Hit";
        }
    }

    #region Callbacks
    protected override void RegisterInputCallbacks()
    {
        if (playerControls == null) return;

        playerControls.Gameplay.Attack.performed += OnAttack;
    }

    protected override void UnregisterInputCallbacks()
    {
        if (playerControls == null) return;

        playerControls.Gameplay.Attack.performed -= OnAttack;
    }
    #endregion
}
