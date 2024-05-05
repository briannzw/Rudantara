using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<Character> characters = new();
    private Character SelectedChara => characters[selectedIndex];
    private int selectedIndex = 0;

    [Header("UI References")]
    [SerializeField] private TMP_Text charaName;
    [SerializeField] private TMP_Text charaLevel;
    [SerializeField] private Image charaImage;
    [SerializeField] private Transform statsList;
    [SerializeField] private GameObject statItem;

    [Header("Nav Buttons")]
    [SerializeField] private GameObject prevButton;
    [SerializeField] private GameObject nextButton;

    private void UpdateNavButton()
    {
        prevButton.SetActive(selectedIndex > 0);
        nextButton.SetActive(selectedIndex < characters.Count - 1);
    }

    public void ChangeChara(int indexChange)
    {
        selectedIndex = Mathf.Clamp(selectedIndex + indexChange, 0, characters.Count - 1);
        UpdateNavButton();
        charaName.text = SelectedChara.gameObject.name;
        charaLevel.text = "Lv. " + SelectedChara.GetLevel().ToString();
        charaImage.sprite = SelectedChara.characterImage;

        // Remove all children
        foreach(Transform child in statsList)
        {
            Destroy(child.gameObject);
        }

        // Populate List
        foreach(StatEnum statEnum in Enum.GetValues(typeof(StatEnum)))
        {
            GameObject go = Instantiate(statItem, statsList);
            go.GetComponent<StatItemUI>().Initialize(statEnum.ToString(), SelectedChara.CheckStat(statEnum));
        }

        foreach (DynamicStatEnum dynamicStatEnum in Enum.GetValues(typeof(DynamicStatEnum)))
        {
            GameObject go = Instantiate(statItem);
            go.GetComponent<StatItemUI>().Initialize(dynamicStatEnum.ToString(), SelectedChara.CheckStat(dynamicStatEnum), SelectedChara.CheckStatMax(dynamicStatEnum));
        }
    }

    public void Open()
    {
        gameObject.SetActive(true);
        selectedIndex = 0;
        ChangeChara(0);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
