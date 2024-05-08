using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHUD : MonoBehaviour
{
    [Header("References")]
    private EnemyCombatController bossCombat;
    private Character bossChara;

    [Header("UI References")]
    [SerializeField] private GameObject healthBar;
    [SerializeField] private Image barImage;
    [SerializeField] private HealthBar hpBar;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text levelText;

    public void SetBoss(GameObject boss)
    {
        bossCombat = boss.GetComponent<EnemyCombatController>();
        bossChara = boss.GetComponent<Character>();
        hpBar.character = bossChara;
    }

    public void Spawned()
    {
        bossCombat.OnCombatEngaged += () =>
        {
            healthBar.SetActive(true);
            barImage.fillAmount = bossChara.CheckStat(DynamicStatEnum.Health) / bossChara.CheckStatMax(DynamicStatEnum.Health);
            hpText.text = bossChara.CheckStat(DynamicStatEnum.Health) + " / " + bossChara.CheckStatMax(DynamicStatEnum.Health);
            levelText.text = "LV. " + bossChara.Level.ToString();
        };

        bossCombat.OnReset += () =>
            healthBar.SetActive(false);
    }
}
