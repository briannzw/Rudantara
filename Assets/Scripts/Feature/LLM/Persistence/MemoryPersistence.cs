using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MemoryPersistence : MonoBehaviour
{
    [Header("Variable")]
    public float decayFactor = .9f;

    [Header("Parameters")]
    private string pastMemory;
    private SerializedDictionary<string, float> memory = new();
    private SerializedDictionary<string, int> tempMemory = new();
    [SerializeField, TextArea] private string emptyMemoryDescription;

    [Header("References")]
    [SerializeField] private PersonalityAction actionModule;
    [SerializeField] private MemoryRetrieval memoryRetrival;

    private void OnEnable()
    {
        actionModule.OnResponseReceived += Summarize;
    }

    private void OnDisable()
    {
        actionModule.OnResponseReceived -= Summarize;
    }

    private void Start()
    {
        memory = new SerializedDictionary<string, float>(GameManager.Instance.saveManager.SaveData.Memory);
    }

    private void Summarize(ActionResponse response)
    {
        Decay();
        pastMemory = response.Summary;
        if (memory.ContainsKey(response.Summary))
        {
            memory[pastMemory] = 1;
            return;
        }

        memory.Add(pastMemory, 1);
    }

    private void Decay()
    {
        foreach (var mem in memory.ToList())
        {
            memory[mem.Key] = mem.Value * decayFactor;
            if (memory[mem.Key] < .05f) memory.Remove(mem.Key);
        }
    }

    public void Insert(string memoryString)
    {
        if (!tempMemory.ContainsKey(memoryString))
            tempMemory.Add(memoryString, 0);

        // Event Frequency
        tempMemory[memoryString]++;
    }

    public string Describe()
    {
        string outMemory = "";

        if (tempMemory.Count == 0) return emptyMemoryDescription + '\n';

        foreach(var mem in tempMemory)
        {
            outMemory += mem.Key + " " + mem.Value + " time(s)\n";
        }

        return outMemory;
    }

    public string Retrieve(HashSet<string> keywords)
    {
        string outMemory = memoryRetrival.Retrieve(memory, keywords);

        if (string.IsNullOrEmpty(outMemory)) return emptyMemoryDescription + '\n';

        return outMemory;
    }

    public void Reset()
    {
        tempMemory.Clear();
    }

    private void Print()
    {
        foreach (var mem in tempMemory)
        {
            // dateTime to string yyyy-MM-dd HH:mm:ssZ
            Debug.Log(String.Format("{0:u}", mem.Value) + ": " + mem.Key);
        }
    }

    public void Save()
    {
        GameManager.Instance.saveManager.SaveData.Memory = memory;
    }
}
