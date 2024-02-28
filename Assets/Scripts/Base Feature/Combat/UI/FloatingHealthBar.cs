using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Image barImage;
    protected Character character;

    protected virtual void Awake()
    {
        character = GetComponentInParent<Character>();
    }

    protected virtual void Start()
    {
        barImage.fillAmount = character.CheckStat(DynamicStatEnum.Health) / character.CheckStatMax(DynamicStatEnum.Health);

        character.OnCharacterDynamicStatsChanged += () =>
            barImage.fillAmount = character.CheckStat(DynamicStatEnum.Health) / character.CheckStatMax(DynamicStatEnum.Health);
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + Camera.main.transform.forward);
    }
}
