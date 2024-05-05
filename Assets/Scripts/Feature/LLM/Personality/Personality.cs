using UnityEngine;

[RequireComponent(typeof(Describable))]
public class Personality : MonoBehaviour
{
    private Describable me;
    public string Name => me.Name;
    [SerializeField, TextArea] private string characterDescription;
    [SerializeField] private BigFivePersonality personalityTraits;

    // Sensors
    public Vision vision;

    // Memory
    private MemoryPersistence memory;

    // State Controller
    [SerializeField] private PersonalityStateController stateController;

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

    public void EnemyDetected(Character chara) => stateController.AddEnemy(chara);
    public void EnemyRemoved(Character chara) => stateController.RemoveEnemy(chara);
    public bool IsEnemyDetected => stateController.IsEnemyDetected;

    public void Forget()
    {
        memory.Reset();
    }

    public string CreatePrompt(string addPrev = null)
    {
        string prompt = characterDescription + "\n\n" + "You are an adventurer that have following personality that is based on the Big Five Personality Traits (scales from 0 to 100):\n";

        prompt += personalityTraits.Describe() + "\n\n";

        prompt += "Here is your response and its result on past action, please utilize this information when choosing actions:\n";
        prompt += "Previous memories:\n";
        prompt += memory.Reflect();
        if(addPrev != null) prompt += addPrev + "\n\n";

        // Memory persistence
        prompt += "Here are your memories after your past action and its frequency:\n";
        prompt += memory.Describe();

        return prompt;
    }
}
