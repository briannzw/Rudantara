using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MemoryPersistence : MonoBehaviour
{
    private SerializedDictionary<string, int> pastMemory = new();
    private SerializedDictionary<string, int> memory = new();
    [SerializeField, TextArea] private string emptyMemoryDescription;

    public void Insert(string memoryString)
    {
        if (!memory.ContainsKey(memoryString))
            memory.Add(memoryString, 0);

        // Event Frequency
        memory[memoryString]++;
    }

    public string Describe()
    {
        string outMemory = "";

        if (memory.Count == 0) return emptyMemoryDescription + '\n';

        foreach(var mem in memory)
        {
            outMemory += mem.Key + " " + mem.Value + " time(s)\n";
        }

        return outMemory;
    }

    public string Reflect()
    {
        string outMemory = "";

        if (pastMemory.Count == 0) return emptyMemoryDescription + '\n';

        foreach (var mem in pastMemory)
        {
            outMemory += mem.Key + " " + mem.Value + " time(s)\n";
        }

        return outMemory;
    }

    public void Reset()
    {
        pastMemory = memory;
        memory.Clear();
    }

    private void Print()
    {
        foreach (var mem in memory)
        {
            // dateTime to string yyyy-MM-dd HH:mm:ssZ
            Debug.Log(String.Format("{0:u}", mem.Value) + ": " + mem.Key);
        }
    }

    private void Save()
    {

    }
}
