using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    private NavMeshAgent agent;
    private Transform targetTransform;

    public float StopDistance = StatsConst.STOPPING_DISTANCE;

    public bool IsTargetInRange { get; private set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 2f; //attackRange;
        agent.baseOffset = -.1f;
    }

    private void Update()
    {
        MoveToTarget();
    }

    public void SetTarget(Transform target)
    {
        targetTransform = target;
    }

    private void MoveToTarget()
    {
        if (targetTransform == null) return;

        agent.stoppingDistance = StopDistance;
        agent.SetDestination(targetTransform.position);
        animator.SetBool("IsMoving", true);

        // Agent reaches target destination
        if(!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            animator.SetBool("IsMoving", false);
            FaceTarget();
            IsTargetInRange = true;
        }
        else
        {
            IsTargetInRange = false;
        }
    }

    private void FaceTarget()
    {
        Vector3 targetDirection;
        Quaternion lookRotation;
        targetDirection = (targetTransform.position - transform.position).normalized;
        lookRotation = Quaternion.LookRotation(new Vector3(targetDirection.x, 0f, targetDirection.z));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 175f * Time.deltaTime);
    }
}
