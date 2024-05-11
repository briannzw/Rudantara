using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    [SerializeField] private AgentController controller;
    private HashSet<Character> detectedCharacters = new();
    private Transform currentTarget;

    private void Awake()
    {
        //GetComponent<SphereCollider>().radius = controller.ChaseRadius;
    }

    private void Update()
    {
        if (controller.IsTargetDied()) SwitchTarget();

        if (!controller.IsTargetNull && controller.IsTargetDied())
        {
            controller.SetTarget(null);
            currentTarget = null;

            SwitchTarget();
        }
    }

    public void SwitchTarget()
    {
        if (detectedCharacters.Count == 0) return;

        foreach (var chara in detectedCharacters)
        {
            if (chara.IsDead) continue;

            controller.SetTarget(chara.transform);
            currentTarget = chara.transform;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Companion"))
        {
            controller.SetTarget(other.transform);
            currentTarget = other.transform;
            detectedCharacters.Add(other.GetComponent<Character>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Companion"))
        {
            if (other.transform == currentTarget)
            {
                controller.SetTarget(null);
                currentTarget = null;
            }

            detectedCharacters.Remove(other.GetComponent<Character>());
        }
    }
}
