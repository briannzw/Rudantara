using Module.Detector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class CompanionCombatController : MonoBehaviour
{
    private enum CompanionCombatState
    {
        Attack, Flee, Idle, Inactive
    }

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Character character;
    [SerializeField] private AgentController controller;

    [Header("Player References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private PlayerAttackController playerAttack;

    [Header("Action Related")]
    [SerializeField] private PersonalityAction actionModule;
    [SerializeField] private PersonalityTargetHandler actionTarget;
        
    [Header("Normal Attack")]
    [SerializeField] private string normalAttackAnimTrigger = "Attack";

    [Header("Skills")]
    [SerializeField] private List<Skill> skills = new();
    [SerializeField] private SkillCaster skillCaster;

    private NavMeshAgent agent;
    private Dictionary<Skill, float> SkillCooldowns = new();

    private int resistance;

    private float normalTimer;

    private float fleeTimer;

    private CompanionCombatState currentState;
    private int nextActionIndex;

    private bool taunting;

    private void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        DescribeSkills();
    }

    private void Start()
    {
        // Reset Cooldowns for all Skills
        foreach (var skill in skills)
        {
            SkillCooldowns.Add(skill, 0f);
        }

        resistance = StatsConst.HURT_RESISTANCE;
        character.OnCharacterHurt += () =>
        {
            // Resistance when animating attacks, etc.
            if (!animator.GetBool("CanMove")) return;

            resistance--;
            if (resistance <= 0)
            {
                animator.SetTrigger("Hurt");
                resistance = StatsConst.HURT_RESISTANCE;
            }
        };

        // Player Combat Engage
        playerAttack.OnCombatEngaged += (other) =>
        {
            if (currentState != CompanionCombatState.Inactive) return;

            controller.SetTarget(other);
            currentState = CompanionCombatState.Attack;
            // Default to attack nearest enemy
            nextActionIndex = 1;
        };
    }

    private void OnEnable()
    {
        // Action
        actionModule.OnResponseReceived += ChangeState;
        currentState = CompanionCombatState.Idle;
    }

    private void OnDisable()
    {
        actionModule.OnResponseReceived -= ChangeState;
        taunting = false;
    }

    private void ChangeState(ActionResponse actionResponse)
    {
        if (actionResponse.State != 1)
        {
            currentState = CompanionCombatState.Inactive;
            return;
        }
        int chosenIndex = actionResponse.ActionIndex;

        taunting = false;

        nextActionIndex = 0;

        if(actionResponse.ActionIndex == 0)
        {
            // If there is no valid target
            if (actionResponse.TargetName == null || actionTarget.GetTarget(actionResponse.TargetName) == null)
            {
                chosenIndex = actionResponse.AlternativeActionIndex + 1;
                // If alternative is not valid
                if(actionResponse.AlternativeActionIndex == -1)
                    chosenIndex = 1;
            }

            // If alternative is not valid
            if (actionResponse.AlternativeActionIndex == -1)
                nextActionIndex = 1;
        }

        switch (chosenIndex)
        {
            // Focus attacking one target
            case 0:
                controller.SetTarget(actionTarget.GetTarget(actionResponse.TargetName).transform);
                currentState = CompanionCombatState.Attack;
                nextActionIndex = actionResponse.AlternativeActionIndex + 1;
                break;
            // Attack nearest seen enemy
            case 1:
                controller.SetTarget(GetNearestEnemy());
                currentState = CompanionCombatState.Attack;
                // Repeat self
                nextActionIndex = 1;
                break;
            // Protect your partner by taunting enemies, tanking the damages
            case 2:
                controller.SetTarget(GetNearestEnemy());
                currentState = CompanionCombatState.Attack;
                StartCoroutine(Taunt(transform));
                break;
            // Protect your partner by luring enemies far from your partner
            case 3:
                controller.SetTarget(null);
                StartCoroutine(Taunt(transform));
                currentState = CompanionCombatState.Flee;
                fleeTimer = 0f;
                break;
            // Seek protection from your partner
            case 4:
                controller.SetTarget(playerTransform);
                currentState = CompanionCombatState.Idle;
                StartCoroutine(Taunt(playerTransform));
                break;
            // Flee from the battle to the safe place
            case 5:
                controller.SetTarget(null);
                currentState = CompanionCombatState.Flee;
                fleeTimer = 0f;
                break;
            // Regroup to your partner
            case 6:
                controller.SetTarget(playerTransform);
                currentState = CompanionCombatState.Inactive;
                break;
        }

        if(!string.IsNullOrEmpty(actionResponse.HealTarget))
        {
            Cast(skills[0], actionTarget.GetTarget(actionResponse.HealTarget));
        }
    }

    private void Update()
    {
        // Every Attack will freezes enemy, so enemy need to wait animation to be finished before starts other attack again
        if (!animator.GetBool("CanMove") || animator.GetBool("Dead")) return;

        switch (currentState)
        {
            case CompanionCombatState.Attack:
                {
                    if ((controller.IsTargetNull || controller.IsTargetDied()) && nextActionIndex != 0)
                    {
                        if (nextActionIndex == 1 && GetNearestEnemy() == null)
                        {
                            controller.SetTarget(playerTransform);
                            currentState = CompanionCombatState.Inactive;
                            return;
                        }
                        ChangeState(new ActionResponse(nextActionIndex));
                    }
                    if (!controller.IsTargetInRange)
                        return;

                    normalTimer -= Time.deltaTime;
                    if (normalTimer < 0f)
                    {
                        animator.SetTrigger(normalAttackAnimTrigger);

                        normalTimer = StatsConst.N_SPEED_MOD / character.CheckStat(StatEnum.Speed);
                    }
                    break;
                }
            case CompanionCombatState.Flee:
                {
                    fleeTimer += Time.deltaTime;

                    if (fleeTimer >= .2f)
                    {
                        Transform nearestEnemy = GetNearestEnemy();
                        if (nearestEnemy == null) return;

                        if (Vector3.Distance(transform.position, nearestEnemy.position) < 5f)
                        {
                            agent.SetDestination(transform.position + (transform.position - nearestEnemy.position) * 4f);
                        }
                        fleeTimer = 0f;
                    }
                    break;
                }
            case CompanionCombatState.Idle :
            case CompanionCombatState.Inactive:
                break;
        }
    }

    private IEnumerator Taunt(Transform target)
    {
        taunting = true;
        var enemyList = new List<EnemyCombatController>();
        while (taunting)
        {
            enemyList = ColliderDetector.Find<EnemyCombatController>(transform.position, 5f, LayerMask.GetMask("Enemy"));
            for (int i = 0; i < enemyList.Count; i++)
            {
                enemyList[i].Taunted(target);
            }

            yield return new WaitForSeconds(1f);
        }

        // Cancel taunt
        for (int i = 0; i < enemyList.Count; i++)
            enemyList[i].Taunted(null);
    }

    private void Cast(Skill skill, Character target)
    {
        if (!skills.Contains(skill)) return;

        skillCaster.Target = target;
        animator.SetTrigger("Heal");
    }

    private Transform GetNearestEnemy()
    {
        return ColliderDetector.FindNearest<Transform>(transform.position, 10f, LayerMask.GetMask("Enemy"));
    }

    private void DescribeSkills()
    {
        string description = "";

        for (int i = 1; i <= skills.Count; i++)
        {
            // Indexer
            description += $"{i}. {skills[i - 1].Name}";

            // Seperator
            if (i < skills.Count) description += ",\n";
            else description += ".\n";
        }

        actionModule.skillDescribe = description;
    }
}
