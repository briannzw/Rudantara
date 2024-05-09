using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Image barImage;
    [SerializeField] protected TMP_Text barText;
    public Character character;

    protected virtual void Awake()
    {
        if(character == null)
            character = GetComponentInParent<Character>();
    }

    protected virtual void Start()
    {
        barImage.fillAmount = character.CheckStat(DynamicStatEnum.Health) / character.CheckStatMax(DynamicStatEnum.Health);
        if (barText != null) barText.text = Mathf.RoundToInt(character.CheckStat(DynamicStatEnum.Health)).ToString() + " / "+ Mathf.RoundToInt(character.CheckStatMax(DynamicStatEnum.Health)).ToString();

        character.OnCharacterDynamicStatsChanged += () =>
        {
            barImage.fillAmount = character.CheckStat(DynamicStatEnum.Health) / character.CheckStatMax(DynamicStatEnum.Health);
            if (barText != null) barText.text = Mathf.RoundToInt(character.CheckStat(DynamicStatEnum.Health)).ToString() + " / " + Mathf.RoundToInt(character.CheckStatMax(DynamicStatEnum.Health)).ToString();
        };
    }
}
