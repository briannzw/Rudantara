using TMPro;
using UnityEngine;

public class StatItemUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text statName;
    [SerializeField] private TMP_Text statValue;

    public void Initialize(string name, float value)
    {
        statName.text = name;
        statValue.text = System.Math.Round(value, 2).ToString();
    }

    public void Initialize(string name, float value, float maxValue)
    {
        statName.text = name;
        statValue.text = System.Math.Round(value, 2).ToString() + " / " + System.Math.Round(maxValue, 2).ToString();
    }
}
