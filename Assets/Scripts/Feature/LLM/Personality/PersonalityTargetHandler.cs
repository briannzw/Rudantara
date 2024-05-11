using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

public class PersonalityTargetHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Vision vision;

    private Dictionary<string, Character> targets = new();

    public void AddTarget(string name, Character chara)
    {
        if (targets.ContainsKey(name)) return;
        targets.Add(name, chara);
    }

    public Character GetTarget(string name)
    {
        if (!targets.ContainsKey(name)) return null;
        return targets[name];
    }

    public HashSet<string> TargetsToHashset()
    {
        HashSet<string> _targets = new HashSet<string>();

        foreach(var target in targets)
        {
            if(target.Key.Contains("Thorntle")) _targets.Add("thorntle");
            if(target.Key == "Your partner")
            {
                _targets.Add("partner");
                continue;
            }
            _targets.Add(target.Key.ToLower());
        }

        return _targets;
    }

    private void Populate()
    {
        targets.Clear();

        // Initialize Targets
        foreach (var seen in vision.Seen)
        {
            if (seen == null) continue;
            Character chara = seen.GetComponentInChildren<Character>();
            if (chara == null || chara.IsDead) continue;

            AddTarget(seen.Name, chara);
        }
    }

    public string CreatePrompt(bool update = true)
    {
        if(update) Populate();

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
