using UnityEngine;
using UnityEngine.UI;

public class CharacterEXPBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterLeveling charaLevel;

    [Header("UI References")]
    [SerializeField] private Image barImage;

    private void Awake()
    {
        barImage.enabled = true;
    }

    private void OnEnable()
    {
        charaLevel.OnExperienceChanged += UpdateUI;
    }

    private void OnDisable()
    {
        charaLevel.OnExperienceChanged -= UpdateUI;
    }

    private void UpdateUI()
    {
        barImage.fillAmount = charaLevel.Experiences / charaLevel.ExpNeeded;
    }
}
