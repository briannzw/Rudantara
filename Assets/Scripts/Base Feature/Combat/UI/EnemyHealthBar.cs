using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : FloatingHealthBar
{
    [Header("References")]
    [SerializeField] private EnemyCombatController enemyCombat;
    [SerializeField] private Image bg;

    protected override void Awake()
    {
        base.Awake();
        bg.enabled = false;
        barImage.enabled = false;
    }

    protected override void Start()
    {
        base.Start();

        enemyCombat.OnCombatEngaged += () =>
        {
            bg.enabled = true;
            barImage.enabled = true;
            barImage.fillAmount = character.CheckStat(DynamicStatEnum.Health) / character.CheckStatMax(DynamicStatEnum.Health);
        };

        enemyCombat.OnReset += () =>
        {
            bg.enabled = false;
            barImage.enabled = false;
        };
    }
}
