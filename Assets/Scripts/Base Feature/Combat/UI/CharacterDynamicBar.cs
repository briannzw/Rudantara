using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDynamicBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Character character;
    [SerializeField] protected Image barImage;
    [SerializeField] private TMP_Text displayText;

    [Header("Stats")]
    [SerializeField] private DynamicStatEnum dynamicStat;

    protected void Awake()
    {
        barImage.enabled = true;
    }

    private void OnDisable()
    {
        character.OnCharacterDynamicStatsChanged -= UpdateUI;
    }

    protected void Start()
    {
        character.OnCharacterDynamicStatsChanged += UpdateUI;
    }

    private void UpdateUI()
    {
        float value = character.CheckStat(dynamicStat);
        float maxValue = character.CheckStatMax(dynamicStat);

        barImage.fillAmount = value / maxValue;
        displayText.text = $"{Mathf.RoundToInt(value)}/{Mathf.RoundToInt(maxValue)}";
    }
}
