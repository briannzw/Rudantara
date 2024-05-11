using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonalityStateController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PersonalityAction actionModule;

    private HashSet<Character> enemiesDetected = new();
    public bool IsEnemyDetected()
    {
        bool IsEnemiesAlive = false;

        foreach(var enemy in enemiesDetected)
        {
            if (!enemy.IsDead)
            {
                IsEnemiesAlive = true;
                break;
            }
        }

        return IsEnemiesAlive;
    }

    private bool prevState = false;

    private void Start()
    {
        DungeonGenerator.Instance.OnDungeonComplete += () => actionModule.CreatePrompt();
    }

    public void AddEnemy(Character chara)
    {
        if (!enemiesDetected.Contains(chara)) enemiesDetected.Add(chara);

        CheckStateChange();
    }

    public void RemoveEnemy(Character chara)
    {
        enemiesDetected.Remove(chara);
        CheckStateChange();
    }

    private void CheckStateChange()
    {
        // Every Enemy detected/undetected
        if (prevState != IsEnemyDetected())
        {
            actionModule.CreatePrompt();
        }
        prevState = IsEnemyDetected();
    }
}
