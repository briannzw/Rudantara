using Save;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SaveManager saveManager;

    [Header("UI References")]
    [SerializeField] private Button continueButton;
    [SerializeField] private TMP_Text continueIterationText;

    private void Awake()
    {
        if (!saveManager.CheckDataExists()) NewGame();
        saveManager.Load();
    }

    public void NewGame()
    {
        //saveManager.SaveData.Iteration = 1;
        saveManager.SaveData = new();
        saveManager.Save();
    }

    private void Start()
    {
        continueButton.interactable = saveManager.SaveData.PlayerExp > 0;
        if (saveManager.SaveData.PlayerExp > 0) continueIterationText.text = "Iteration " + saveManager.SaveData.Iteration;
        else continueIterationText.text = "";
    }
}
