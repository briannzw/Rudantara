using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyCombatController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Character character;
    [SerializeField] private AgentController controller;
    [SerializeField] private HitController hitController;
    private Describable describable;

    [Header("Normal Attack")]
    [SerializeField] private string normalAttackDescribe = "Basic Hit";
    [SerializeField] private string normalAttackAnimTrigger;

    [Header("Skills")]
    [SerializeField] private int castSkillChance = 25;
    [SerializeField] private List<Skill> Skills = new();
    
    private NavMeshAgent agent;
    private Dictionary<Skill, float> SkillCooldowns = new();

    private float betweenSkillCooldown = 3f;
    private float resetAfterLeaving = 5f;
    
    private float normalTimer;
    private float skillTimer;
    private float leaveTimer;

    private float wanderTime;
    private float wanderTimer;

    private int resistance;

    private Vector3 origin;

    #region Events
    public Action OnCombatEngaged;
    public Action OnReset;
    #endregion

    private void OnEnable()
    {
        animator.SetBool("Dead", false);
        agent.enabled = true;
        GetComponent<Collider>().enabled = true;
        controller.enabled = true;
        controller.SetTarget(null);
        Reset();
    }

    private void Reset()
    {
        origin = transform.position;

        character.ResetDynamicValue();
        skillTimer = StatsConst.SKILL_CHECK_INTERVAL;
        normalTimer = StatsConst.N_SPEED_MOD / character.CheckStat(StatEnum.Speed);

        foreach (var skill in Skills)
        {
            if(SkillCooldowns.ContainsKey(skill))
                SkillCooldowns[skill] = 0f;
        }

        OnReset?.Invoke();
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        character = GetComponent<Character>();
        controller = GetComponent<AgentController>();
        agent = GetComponent<NavMeshAgent>();
        if (!describable) describable = GetComponentInChildren<Describable>();
    }

    private void Start()
    {
        // Reset Cooldowns for all Skills
        foreach(var skill in Skills)
        {
            SkillCooldowns.Add(skill, 0f);
        }

        // Set Movement Speed
        agent.speed = character.CheckStat(StatEnum.Speed) / StatsConst.MOV_SPEED_MOD;

        character.OnCharacterStatsChanged += () =>
            agent.speed = character.CheckStat(StatEnum.Speed) / StatsConst.MOV_SPEED_MOD;

        // Hurt Animation
        resistance = StatsConst.HURT_RESISTANCE;
        character.OnCharacterHurt += () =>
        {
            // Resistance when animating attacks, etc.
            if (!animator.GetBool("canMove")) return;

            resistance--;
            if (resistance <= 0)
            {
                animator.SetTrigger("Hurt");
                resistance = StatsConst.HURT_RESISTANCE;
                describable.OnEvent?.Invoke("[" + describable.Name + "] died and will be respawned.");
            }
        };

        // Died
        character.OnCharacterDie += () =>
        {
            animator.SetBool("Dead", true);
            GetComponent<Collider>().enabled = false;
            controller.enabled = false;

            StartCoroutine(AnimateDead());
        };

        // Reset to Default
        leaveTimer = resetAfterLeaving;
        wanderTime = Random.Range(0.7f * StatsConst.WANDER_INTERVAL, 1.25f * StatsConst.WANDER_INTERVAL);
    }

    private void CastSkill()
    {
        if(Random.Range(0, 100) < castSkillChance)
        {
            List<Skill> availableSkills = new();
            for(int i = 0; i < Skills.Count; i++)
            {
                // Check if skill is not in cooldown and mana required is fulfilled
                if(SkillCooldowns[Skills[i]] <= 0f && character.CheckStat(DynamicStatEnum.Mana) > Skills[i].ManaRequired)
                    availableSkills.Add(Skills[i]);
            }

            if (availableSkills.Count == 0) return;

            // == Fulfilled ==
            int randomSkill = Random.Range(0, availableSkills.Count);
            skillTimer = betweenSkillCooldown;

            // Add Skill Cooldown
            SkillCooldowns[availableSkills[randomSkill]] = availableSkills[randomSkill].Cooldown;

            // Decrease Mana
            character.ChangeDynamicValue(DynamicStatEnum.Mana, -availableSkills[randomSkill].ManaRequired);

            // Set trigger
            animator.SetTrigger(availableSkills[randomSkill].Trigger);

            // Set HitController Name
            hitController.Name = availableSkills[randomSkill].Name;
        }
    }

    private IEnumerator AnimateDead()
    {
        agent.enabled = false;
        yield return new WaitForSeconds(2f);
        float timer = 0f;
        while (timer < 2f)
        {
            timer += Time.deltaTime;
            transform.position -= 1f * Time.deltaTime * Vector3.up;
            yield return null;
        }

        // Destroy(gameObject);
        GetComponent<SpawnObject>().Release();
    }

    private void Update()
    {
        // Every Attack will freezes enemy, so enemy need to wait animation to be finished before starts other attack again
        if (!animator.GetBool("canMove") || animator.GetBool("Dead")) return;

        // Reduce Cooldown of all skills
        for (int i = 0; i < Skills.Count; i++)
        {
            if (SkillCooldowns[Skills[i]] > 0f)
                SkillCooldowns[Skills[i]] -= Time.deltaTime;
        }

        if (controller.IsTargetInRange)
        {
            skillTimer -= Time.deltaTime;

            // Each 1 second try to cast skill
            if (skillTimer < 0f)
            {
                skillTimer = StatsConst.SKILL_CHECK_INTERVAL;
                CastSkill();
            }

            normalTimer -= Time.deltaTime;
            if (normalTimer < 0f)
            {
                hitController.Name = normalAttackDescribe;
                animator.SetTrigger(normalAttackAnimTrigger);
                // TODO: Distribution
                normalTimer = StatsConst.N_SPEED_MOD / character.CheckStat(StatEnum.Speed);
            }

            if (leaveTimer > 0f)
                OnCombatEngaged?.Invoke();
            
            leaveTimer = 0f;
        }
        else
        {
            if (leaveTimer >= resetAfterLeaving)
            {
                Reset();
            }
            else
                leaveTimer += Time.deltaTime;

            Wander();
        }
    }

    private void Wander()
    {
        wanderTimer += Time.deltaTime;

        if(wanderTimer >= wanderTime)
        {
            wanderTime = Random.Range(0.7f * StatsConst.WANDER_INTERVAL, 1.25f * StatsConst.WANDER_INTERVAL);
            wanderTimer = 0f;

            Vector3 randomPos;
            if(Vector3.Distance(transform.position, origin) > StatsConst.MAX_WANDER_FROM_SPAWN)
            {
                randomPos = origin + Random.insideUnitSphere * Random.Range(StatsConst.MIN_WANDER_DISTANCE, StatsConst.MAX_WANDER_DISTANCE);
                agent.stoppingDistance = 0f;
                agent.SetDestination(randomPos);
                return;
            }

            randomPos = transform.position + Random.insideUnitSphere * Random.Range(StatsConst.MIN_WANDER_DISTANCE, StatsConst.MAX_WANDER_DISTANCE);
            if (NavMesh.SamplePosition(randomPos, out var hit, StatsConst.NAV_SAMPLE_POS_MAX_DISTANCE, 1 << NavMesh.GetAreaFromName("Walkable")))
            {
                agent.stoppingDistance = 0f;
                agent.SetDestination(hit.position);
            }
        }
    }
}
