using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Image barImage;
    public Character character;

    protected virtual void Awake()
    {
        if(character == null)
            character = GetComponentInParent<Character>();
    }

    protected virtual void Start()
    {
        barImage.fillAmount = character.CheckStat(DynamicStatEnum.Health) / character.CheckStatMax(DynamicStatEnum.Health);

        character.OnCharacterDynamicStatsChanged += () =>
            barImage.fillAmount = character.CheckStat(DynamicStatEnum.Health) / character.CheckStatMax(DynamicStatEnum.Health);
    }
}
