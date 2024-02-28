using UnityEngine;

public class EnemyHealthBar : FloatingHealthBar
{
    [Header("References")]
    [SerializeField] private EnemyCombatController enemyCombat;

    protected override void Awake()
    {
        base.Awake();
        barImage.enabled = false;
    }

    protected override void Start()
    {
        base.Start();

        enemyCombat.OnCombatEngaged += () =>
        {
            barImage.enabled = true;
            barImage.fillAmount = character.CheckStat(DynamicStatEnum.Health) / character.CheckStatMax(DynamicStatEnum.Health);
        };

        enemyCombat.OnReset += () =>
            barImage.enabled = false;
    }
}
