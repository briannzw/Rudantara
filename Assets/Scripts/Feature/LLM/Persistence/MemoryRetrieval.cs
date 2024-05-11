using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MemoryRetrieval : MonoBehaviour
{
    public string Retrieve(Dictionary<string, float> memory, HashSet<string> keywords)
    {
        string relevantMemory = "";

        var keyFrequency = new Dictionary<string, float>();
        int maxCount = -1;

        foreach(var mem in memory)
        {
            if (!keywords.Any(mem.Key.ToLower().Contains)) continue;

            int count = keywords.Count(k => mem.Key.ToLower().Contains(k));

            keyFrequency.Add(mem.Key, count);
            if (count > maxCount) maxCount = count;
        }

        // Normalize
        foreach(var freq in keyFrequency.ToList())
        {
            keyFrequency[freq.Key] = freq.Value / maxCount;
        }

        // Get relevancy by Recency + Freq
        var orderedList = memory/*.Where(mem => keywords.Contains(mem.Key))*/.OrderByDescending(mem => mem.Value + (keyFrequency.ContainsKey(mem.Key) ? keyFrequency[mem.Key] : 0)).ToList();
        
        if (orderedList.Count == 0) return null;

        relevantMemory = orderedList[0].Key;
#if UNITY_EDITOR
        string debugKey = "Keywords: ";
        foreach (var key in keywords)
        {
            debugKey += "\n" + key;
        }
        debugKey += "\n[Score: " + (memory[relevantMemory] + (keyFrequency.ContainsKey(relevantMemory) ? keyFrequency[relevantMemory] : 0)).ToString() + "] " + relevantMemory;
        Debug.Log(debugKey);
#endif
        return relevantMemory;
    }
}
