using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PersonalityAction : MonoBehaviour, IRequestResponse
{
    [Header("References")]
    [SerializeField] private GASSender sender;
    [SerializeField] private Personality personality;

    [Header("Stats")]
    [SerializeField] private Character playerCharacter;
    [SerializeField] private Character character;

    [Header("Action")]
    [SerializeField, TextArea] private string actionPrompt;
    [SerializeField, TextArea] private List<string> actions = new();

    [Header("Alternative")]
    [SerializeField, TextArea] private string alternativeActionPrompt;

    [Header("Target")]
    [SerializeField] private PersonalityTargetHandler personalityTargetHandler;

    //[Header("Suggestion")]
    //[SerializeField] private Suggestion suggestion;

    [Header("Line")]
    [SerializeField, TextArea] private string linePrompt;

    [Header("Response")]
    [SerializeField, TextArea] private string responseFormat;
    [SerializeField] private float responseTime = 5f;

    public Action<ActionResponse> OnResponseReceived;

    private void Start()
    {
        InvokeRepeating(nameof(CreatePrompt), 10f, responseTime);
    }

    private void CreatePrompt()
    {
        string prompt = personality.CreatePrompt() + "\n";
        // Character Stats
        prompt += character.Describe(true) + "\n" + playerCharacter.Describe() + "\n";

        // Available Actions
        prompt += "\n" + actionPrompt + "\n";

        for(int i = 1; i <= actions.Count; i++)
        {
            // Indexer
            prompt += $"{i}. {actions[i - 1]}";

            // Seperator
            if (i < actions.Count) prompt += ",\n";
            else prompt += ".\n";
        }

        // Targets
        foreach (var seen in personality.vision.Seen)
        {
            Character chara = seen.gameObject.GetComponentInChildren<Character>();
            if (chara == null || chara.IsDead) continue;

            personalityTargetHandler.AddTarget(seen.Name, chara);
        }

        prompt += "\n" + personalityTargetHandler.CreatePrompt();
        personalityTargetHandler.Reset();

        // Alternative
        prompt += "\n" + alternativeActionPrompt + "\n";

        for (int i = 1; i < actions.Count; i++)
        {
            // Indexer
            prompt += $"{i}. {actions[i]}";

            // Seperator
            if (i < actions.Count - 1) prompt += ",\n";
            else prompt += ".\n";
        }

        // Suggestion
        //if(suggestion) suggestion.CanInsert = true;

        // Line
        prompt += "\n" + linePrompt;

        // Format
        prompt += "\n" + responseFormat;

        Debug.Log(prompt);

        Send(prompt);

        personality.Forget();
    }

    public void Send(string prompt)
    {
        StartCoroutine(sender.CallGoogleAppsScript(this, prompt));
    }

    public void Receive(string response)
    {
        if (response == null) return;

        var _response = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);

        OnResponseReceived?.Invoke(new ActionResponse(_response));

        actionIndex = int.Parse(_response["action"]) - 1;
    }

#if UNITY_EDITOR
    int actionIndex = -1;
    private void OnGUI()
    {
        if (actionIndex == -1) return;
        GUI.skin.label.fontSize = 32;
        GUI.Label(new Rect(20f, 10f, 500f, 120f), "Current Action : " + actions[actionIndex]);
    }
#endif
}
