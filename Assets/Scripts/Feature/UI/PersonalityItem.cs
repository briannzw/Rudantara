using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PersonalityItem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text personalityName;
    [SerializeField] private TMP_Text sliderText;
    [SerializeField] private Slider slider;

    public void Initialize(string name, float value)
    {
        personalityName.text = name;
        slider.value = value;
        SetText(value);
    }

    public float GetValue()
    {
        return slider.value;
    }

    public void SetText(float value) => sliderText.text = Math.Round(value, 2).ToString();
}
