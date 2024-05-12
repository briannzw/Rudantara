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

    [Header("Distance")]
    [SerializeField] private AgentController agentController;

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

    [Header("Summary")]
    [SerializeField, TextArea] private string summaryPrompt;

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

    public int RequestFailed = 0;

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
        string prevAction = "";
        
        if(!string.IsNullOrEmpty(pastAction))
            prevAction += "You are currently " + pastAction;

        if(agentController.DistanceFrom(playerCharacter.transform) != -1)
            prevAction += " and " + Math.Round(agentController.DistanceFrom(playerCharacter.transform), 1) + " meters away from your partner.";

        string prevLine = "";
        
        if(pastLine != "\"\"" && !string.IsNullOrEmpty(pastLine))
            prevLine += "Previously, you've said " + pastLine + "\n";
        
        // Keywords
        var keywords = personalityTargetHandler.TargetsToHashset();

        if(!string.IsNullOrEmpty(pastAction))
            keywords.Add(pastAction.ToLower());

        if (personality.IsEnemyDetected)
            keywords.Add("combat");
        else
            keywords.Add("forest");

        string prompt = personality.CreatePrompt(keywords, prevAction + "\n" + prevLine) + "\n";

        // Previous result
        if (result.OwnerChara != null)
        {
            prompt += result.Describe() + "\n";
        }

        // Character Stats
        prompt += character.Describe(true) + "\n" + playerCharacter.Describe() + "\n";

        prompt += "After receiving all of the information above, based on your personality and situation, you need to define your next action by fulfilling all the JSON key and value instructions below.";

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

        prompt += "\n" + targetPrompt + "\n" + DescribeTargets();

        // Alternative
        prompt += "\n" + alternativeActionPrompt + "\n";

        for (int i = 1; i < stateActions.Count; i++)
        {
            // Indexer
            prompt += $"{i}. {stateActions[i]}";

            // Seperator
            if (i < stateActions.Count - 1) prompt += ",\n";
            else prompt += ".\n";
        }

        // Heal
        prompt += "\n" +/* skillPrompt + "\n" + skillDescribe + "\n" + */ skillTargetPrompt + "\n";

        // Add Self as Skill target
        personalityTargetHandler.AddTarget("Yourself", character);
        prompt += DescribeTargets(false);

        // Suggestion
        //if(suggestion) suggestion.CanInsert = true;

        // Line
        // If finishes combat
        if (!personality.IsEnemyDetected && result.WasInCombat)
        {
            prompt += $"(key: line, value: string) Please give your comments on the previous combat max of 20 words. Please keep it short and non-explicit.";
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

        // Summary
        prompt += "\n" + summaryPrompt + "\n";

        // Format
        prompt += "\n" + ReplaceFirst(responseFormat, "null", personality.IsEnemyDetected ? "1" : "0");

        //Debug.Log(prompt);

        Send(prompt);
        sendTime = DateTime.Now;

        // Resets
        personality.ForgetTemp();
    }

    #region Describe Functions
    private string DescribeTargets(bool update = true)
    {
        string prompt;

        prompt = personalityTargetHandler.CreatePrompt(update);

        return prompt;
    }
    #endregion

    public void Send(string prompt)
    {
        prevPrompt = prompt;
        if (requestCoroutine != null) return;
        requestCoroutine = StartCoroutine(sender.CallGoogleAppsScript(this, prompt));

        // If previous prompt failed, create new prompt
        if (RequestFailed > 1)
        {
            CreatePrompt();
            return;
        }
    }

    public void Receive(string response)
    {
        receiveTime = DateTime.Now;
        requestCoroutine = null;

        // If request failed, repeat
        if (response == null)
        {
            RequestFailed++;
            Send(prevPrompt);
            return;
        }

        RequestFailed = 0;
        StartCoroutine(RePrompt());

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
