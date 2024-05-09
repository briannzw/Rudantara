using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonalityAction : MonoBehaviour, IRequestResponse
{
    [Header("Prompt")]
    [SerializeField, TextArea] private string prevPrompt;

    [Header("References")]
    [SerializeField] private GASSender sender;
    [SerializeField] private Personality personality;

    [Header("Stats")]
    [SerializeField] private Character playerCharacter;
    [SerializeField] private Character character;

    [Header("Action")]
    [SerializeField, TextArea] private string actionStatePrompt;
    [SerializeField, TextArea] private string actionPrompt;
    [SerializeField, TextArea] private List<string> actions = new();
    [SerializeField, TextArea] private string combatActionPrompt;
    [SerializeField, TextArea] private List<string> combatActions = new();

    [Header("Alternative")]
    [SerializeField, TextArea] private string alternativeActionPrompt;

    [Header("Target")]
    [SerializeField, TextArea] private string targetPrompt;
    [SerializeField] private PersonalityTargetHandler personalityTargetHandler;

    [Header("Skill")]
    [SerializeField, TextArea] private string skillPrompt;
    [TextArea] public string skillDescribe;
    [SerializeField, TextArea] private string skillTargetPrompt;

    //[Header("Suggestion")]
    //[SerializeField] private Suggestion suggestion;

    [Header("Line")]
    [SerializeField, TextArea] private string linePrompt;

    [Header("Emotion")]
    [SerializeField] private List<string> emotions;
    [SerializeField, TextArea] private string emotionPrompt;

    [Header("Response")]
    [SerializeField, TextArea] private string responseFormat;
    [SerializeField] private float responseTime = 5f;

    public Action<ActionResponse> OnResponseReceived;

    private DateTime sendTime;
    private DateTime receiveTime;
    private Coroutine requestCoroutine;

    #region Reflect
    private ActionResult result = new();
    private string pastAction;
    private string pastLine;
    #endregion

    private void Start()
    {
        character.OnCharacterKill += (Character chara) => result.OwnEnemiesKilled++;
        playerCharacter.OnCharacterKill += (Character chara) => result.PlayerEnemiesKilled++;
    }

    private IEnumerator RePrompt()
    {
        yield return new WaitForSeconds(responseTime);
        CreatePrompt();
        //InvokeRepeating(nameof(CreatePrompt), 10f, responseTime);
    }

    public void CreatePrompt()
    {
        string prompt = personality.CreatePrompt("Past Action: " + pastAction + "\nPast Line: " + pastLine) + "\n";

        // Previous result
        if (result.OwnerChara != null)
        {
            prompt += result.Describe() + "\n";
        }

        // Character Stats
        prompt += character.Describe(true) + "\n" + playerCharacter.Describe() + "\n";

        // Combat State
        prompt += "\n" + actionStatePrompt + (personality.IsEnemyDetected ? "1" : "0") + "\n";

        // Available Actions
        var stateActions = personality.IsEnemyDetected ? combatActions : actions;
        prompt += "\n" + (personality.IsEnemyDetected ? combatActionPrompt : actionPrompt) + "\n";

        for(int i = 1; i <= stateActions.Count; i++)
        {
            // Indexer
            prompt += $"{i}. {stateActions[i - 1]}";

            // Seperator
            if (i < stateActions.Count) prompt += ",\n";
            else prompt += ".\n";
        }

        // Initialize Targets
        foreach (var seen in personality.vision.Seen)
        {
            Character chara = seen.gameObject.GetComponentInChildren<Character>();
            if (chara == null || chara.IsDead) continue;

            personalityTargetHandler.AddTarget(seen.Name, chara);
        }

        prompt += "\n" + targetPrompt + "\n" + DescribeTargets();

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

        // Skills
        prompt += "\n" + skillPrompt + "\n" + skillDescribe + "\n" + skillTargetPrompt + "\n";

        // Add Self as Skill target
        personalityTargetHandler.AddTarget(personality.Name, character);
        prompt += DescribeTargets();

        // Suggestion
        //if(suggestion) suggestion.CanInsert = true;

        // Line
        // If finishes combat
        if (!personality.IsEnemyDetected && result.WasInCombat)
        {
            prompt += $"(key: line, value: string) Please give your comments on the previous combat max of 15 words. Please keep it short and non-explicit.";
        }
        else prompt += "\n" + linePrompt;

        // Emotion
        prompt += "\n" + emotionPrompt + "\n";

        for (int i = 1; i < emotions.Count; i++)
        {
            // Indexer
            prompt += $"{i}. {emotions[i]}";

            // Seperator
            if (i < emotions.Count - 1) prompt += ",\n";
            else prompt += ".\n";
        }

        // Format
        prompt += "\n" + ReplaceFirst(responseFormat, "null", personality.IsEnemyDetected ? "1" : "0");

        //Debug.Log(prompt);

        Send(prompt);
        sendTime = DateTime.Now;

        // Resets
        personalityTargetHandler.Reset();
        personality.Forget();
    }

    #region Describe Functions
    private string DescribeTargets()
    {
        string prompt;

        prompt = personalityTargetHandler.CreatePrompt();

        return prompt;
    }
    #endregion

    public void Send(string prompt)
    {
        prevPrompt = prompt;
        if (requestCoroutine != null) StopCoroutine(requestCoroutine);
        requestCoroutine = StartCoroutine(sender.CallGoogleAppsScript(this, prompt));
    }

    public void Receive(string response)
    {
        receiveTime = DateTime.Now;

        // If request failed, repeat
        if (response == null)
        {
            Send(prevPrompt);
            return;
        }

        requestCoroutine = StartCoroutine(RePrompt());

        var _response = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);

        OnResponseReceived?.Invoke(new ActionResponse(_response));

        state = int.Parse(_response["state"]);
        actionIndex = int.Parse(_response["action"]) - 1;

        // Past Action and Line
        pastLine = "\"" + _response["line"] + "\"";
        pastAction = state == 0 ? actions[actionIndex] : combatActions[actionIndex];

        // First Received Response
        result.WasInCombat = personality.IsEnemyDetected;
        result.OwnerChara = character;
        result.PlayerChara = playerCharacter;
        result.New();
    }

    int state = -1;
    int actionIndex = -1;
#if UNITY_EDITOR
    private void OnGUI()
    {
        if (state == -1 || actionIndex == -1) return;
        GUI.skin.label.fontSize = 32;
        GUI.Label(new Rect(20f, 10f, 500f, 120f), "Current Action : " + (state == 0 ? actions[actionIndex] : combatActions[actionIndex]));
        TimeSpan ts = receiveTime - sendTime;
        GUI.Label(new Rect(20f, 50f, 500f, 120f), "Response Time : " + ts.TotalMilliseconds + "ms");
    }
#endif
    public string ReplaceFirst(string text, string search, string replace)
    {
        int pos = text.IndexOf(search);
        if (pos < 0)
        {
            return text;
        }
        return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
    }
}
