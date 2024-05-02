using System.Collections.Generic;
using UnityEngine;

public class PersonalityStateController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PersonalityAction actionModule;

    private HashSet<Character> enemiesDetected = new();
    public bool IsEnemyDetected => enemiesDetected.Count > 0;

    private bool prevState = false;

    private void Start()
    {
        actionModule.CreatePrompt();
    }

    public void AddEnemy(Character chara)
    {
        if (!enemiesDetected.Contains(chara)) enemiesDetected.Add(chara);

        // Every Enemy detected/undetected
        if (prevState != IsEnemyDetected)
        {
            actionModule.CreatePrompt();
        }
        prevState = IsEnemyDetected;
    }

    public void RemoveEnemy(Character chara) => enemiesDetected.Remove(chara);
}
