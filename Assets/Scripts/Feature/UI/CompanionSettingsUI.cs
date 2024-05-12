using Save;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CompanionSettingsUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Personality personality;

    [Header("UI References")]
    [SerializeField] private Transform personalityItemParent;
    [SerializeField] private GameObject personalityItemPrefab;

    private Dictionary<string, PersonalityItem> personalityItems = new();

    private void OnEnable()
    {
        GameObject go = Instantiate(personalityItemPrefab, personalityItemParent);
        PersonalityItem item = go.GetComponent<PersonalityItem>();
        item.Initialize("Openness", personality.Traits.Openness);
        personalityItems.Add("Openness", item);

        go = Instantiate(personalityItemPrefab, personalityItemParent);
        item = go.GetComponent<PersonalityItem>();
        item.Initialize("Conscientiousness", personality.Traits.Conscientiousness);
        personalityItems.Add("Conscientiousness", item);

        go = Instantiate(personalityItemPrefab, personalityItemParent);
        item = go.GetComponent<PersonalityItem>();
        item.Initialize("Extraversion", personality.Traits.Extraversion);
        personalityItems.Add("Extraversion", item);

        go = Instantiate(personalityItemPrefab, personalityItemParent);
        item = go.GetComponent<PersonalityItem>();
        item.Initialize("Agreeableness", personality.Traits.Agreeableness);
        personalityItems.Add("Agreeableness", item);

        go = Instantiate(personalityItemPrefab, personalityItemParent);
        item = go.GetComponent<PersonalityItem>();
        item.Initialize("Neuroticism", personality.Traits.Neuroticism);
        personalityItems.Add("Neuroticism", item);
    }

    private void OnDisable()
    {
        foreach(var item in personalityItems.ToList())
        {
            Destroy(item.Value.gameObject);
        }
        personalityItems.Clear();
    }

    public void Save()
    {
        personality.Traits.Openness = personalityItems["Openness"].GetValue();
        personality.Traits.Conscientiousness = personalityItems["Conscientiousness"].GetValue();
        personality.Traits.Extraversion = personalityItems["Extraversion"].GetValue();
        personality.Traits.Agreeableness = personalityItems["Agreeableness"].GetValue();
        personality.Traits.Neuroticism = personalityItems["Neuroticism"].GetValue();

        personality.Forget();

        GameManager.Instance.saveManager.SaveData.CompanionPersonalities = personality.Traits;
        GameManager.Instance.saveManager.Save();
    }
}
