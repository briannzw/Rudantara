using UnityEngine;

[RequireComponent(typeof(Describable))]
public class Personality : MonoBehaviour
{
    private Describable me;
    [SerializeField, TextArea] private string characterDescription;
    [SerializeField] private BigFivePersonality personalityTraits;

    // Sensors
    public Vision vision;

    // Memory
    private MemoryPersistence memory;

    private void Awake()
    {
        me = GetComponent<Describable>();
        vision = GetComponentInChildren<Vision>();
        memory = GetComponentInChildren<MemoryPersistence>();
    }

    public void DescribeVisual(string report)
    {
        if (string.IsNullOrEmpty(report)) return;

        string _report = report.Replace(me.Name, "you");
        memory.Insert(_report);
    }

    public void Forget()
    {
        memory.Reset();
    }

    public string CreatePrompt()
    {
        string prompt = characterDescription + "\n\n" + "You are a person that have following personality that is based on the Big Five Personality Traits (scale from 1 to 100):\n";

        prompt += personalityTraits.Describe() + "\n\n";

        // Memory persistence
        prompt += "Here are your memories after your past action and its frequency:\n";
        prompt += memory.Describe();

        // Combat values (health, weapon power in scale, etc.)

        return prompt;
    }
}
