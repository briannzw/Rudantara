using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyLevelBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterLeveling charaLevel;

    [Header("UI References")]
    [SerializeField] private Image barImage;
    [SerializeField] private TMP_Text levelText;

    private void Awake()
    {
        barImage.enabled = true;
    }

    private void OnEnable()
    {
        UpdateUI();
        charaLevel.OnExperienceChanged += UpdateUI;
    }

    private void OnDisable()
    {
        charaLevel.OnExperienceChanged -= UpdateUI;
    }

    private void UpdateUI()
    {
        barImage.fillAmount = charaLevel.Experiences / charaLevel.ExpNeeded;
        levelText.text = "LV. " + charaLevel.CurrentLevel.ToString();
    }
}
