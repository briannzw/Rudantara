using System.Collections.Generic;
using UnityEngine;

public class PersonalityTargetHandler : MonoBehaviour
{
    private Dictionary<string, Character> prevTargets = new();
    private Dictionary<string, Character> targets = new();

    public void AddTarget(string name, Character chara)
    {
        if (targets.ContainsKey(name)) return;
        
        targets.Add(name, chara);
    }

    public Transform GetTarget(string key)
    {
        if (targets.ContainsKey(key))
            return targets[key].transform;
        else if (prevTargets.ContainsKey(key))
            return prevTargets[key].transform;

        return null;
    }

    public Character GetTargetChara(string key)
    {
        if (targets.ContainsKey(key))
            return targets[key];
        else if (prevTargets.ContainsKey(key))
            return prevTargets[key];

        return null;
    }

    public void Reset()
    {
        prevTargets = new Dictionary<string, Character>(targets);
        targets.Clear();
    }

    public string CreatePrompt()
    {
        string prompt = "";

        foreach(var name in targets)
        {
            // Indexer
            prompt += $"{name.Key}\n";
        }

        if (targets.Count == 0) prompt += "none\n";

        return prompt;
    }
}
