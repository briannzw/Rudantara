using TMPro;
using UnityEngine;

public class Suggestion : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text suggestionTitle;
    [SerializeField] private TMP_Dropdown suggestionDropdown;
    [SerializeField] private TMP_Dropdown suggestionAltDropdown;

    [Header("References")]
    [SerializeField] private MemoryPersistence memory;

    public bool CanInsert {
        set
        {
            canInsert = value;
            suggestionTitle.text = "Suggest" + (canInsert ? "" : " (submitted)");
        }
        get
        {
            return canInsert;
        }
    }

    private bool canInsert;

    public void Proceed()
    {
        if (!CanInsert) return;

        memory.Insert("Your partner suggested to ");
        Debug.LogError("FEATURE DROPPED");
        CanInsert = false;
    }

    public void SuggestSelected(int index)
    {
        suggestionAltDropdown.enabled = (index == 0);
    }
}
