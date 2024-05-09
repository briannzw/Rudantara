using UnityEngine;
using UnityEngine.AI;

public class CompanionWanderController : MonoBehaviour
{
    private enum CompanionWanderState
    {
        Idle,
        Wander
    }

    [Header("References")]
    private AgentController controller;
    [SerializeField] private Transform playerTransform;

    [Header("Action Related")]
    [SerializeField] private PersonalityAction actionModule;
    [SerializeField] private PersonalityTargetHandler actionTarget;

    private CompanionWanderState currentState;

    private float wanderTime;
    private void Awake()
    {
        controller = GetComponent<AgentController>();
    }

    private float wanderTimer;

    private void Start()
    {
        wanderTime = Random.Range(0.7f * StatsConst.WANDER_INTERVAL, 1.25f * StatsConst.WANDER_INTERVAL);

        // Action
        actionModule.OnResponseReceived += ChangeState;
        currentState = CompanionWanderState.Idle;
    }

    private void ChangeState(ActionResponse actionResponse)
    {
        if (actionResponse.State != 0)
        {
            currentState = CompanionWanderState.Idle;
            return;
        }
        int chosenIndex = actionResponse.ActionIndex;
        StopAllCoroutines();

        switch (chosenIndex)
        {
            // Sightseeing
            case 0:
                currentState = CompanionWanderState.Wander;
                break;
        }
    }

    private void Update()
    {
        switch (currentState)
        {
            case CompanionWanderState.Wander:
                {
                    Wander();
                    break;
                }
        }
    }



    Vector3 randomPos;
    private void Wander()
    {
        if (controller.IsMoving) return;

        wanderTimer += Time.deltaTime;

        if (wanderTimer >= wanderTime)
        {
            wanderTime = Random.Range(0.7f * StatsConst.WANDER_INTERVAL, 1.25f * StatsConst.WANDER_INTERVAL);
            wanderTimer = 0f;


            randomPos = transform.position + Random.insideUnitSphere * Random.Range(StatsConst.MIN_WANDER_DISTANCE, StatsConst.MAX_WANDER_DISTANCE);
            if (NavMesh.SamplePosition(randomPos, out var hit, StatsConst.NAV_SAMPLE_POS_MAX_DISTANCE, 1 << NavMesh.GetAreaFromName("Walkable")))
            {
                controller.SetDestination(hit.position);
            }
        }

        if (Vector3.Distance(transform.position, playerTransform.position) > StatsConst.MAX_WANDER_FROM_SPAWN)
        {
            randomPos = playerTransform.position + Random.insideUnitSphere * Random.Range(StatsConst.MIN_WANDER_DISTANCE, StatsConst.MAX_WANDER_DISTANCE);
            controller.SetDestination(randomPos);
        }
    }
}
