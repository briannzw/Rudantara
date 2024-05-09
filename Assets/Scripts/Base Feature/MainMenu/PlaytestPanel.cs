using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlaytestPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text instructionText;
    [SerializeField] private TMP_Text indexText;
    [SerializeField] private GameObject prevButton;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject closeButton;

    [Header("Parameters")]
    [SerializeField] private List<string> instructionTitles;
    [SerializeField, TextArea] private List<string> instructions;
    private int index;

    private void OnEnable()
    {
        index = 0;
        Navigate(0);
        closeButton.SetActive(false);
    }

    public void Navigate(int value)
    {
        index += value;
        prevButton.SetActive(index > 0);
        nextButton.SetActive(index < instructions.Count - 1);

        indexText.text = "Page " + (index + 1).ToString() + " / " + instructions.Count.ToString();

        titleText.text = instructionTitles[index];
        instructionText.text = instructions[index];

        if (index == instructions.Count - 1) closeButton.SetActive(true);
    }
}
