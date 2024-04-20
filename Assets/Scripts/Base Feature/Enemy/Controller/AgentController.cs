using UnityEngine;
using UnityEngine.AI;

public class AgentController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Describable describable;

    private NavMeshAgent agent;
    [SerializeField] private Transform targetTransform;

    public float StopDistance = StatsConst.STOPPING_DISTANCE;

    public bool IsTargetInRange { get; private set; }
    public bool IsTargetNull => targetTransform == null;

    private void OnEnable()
    {
        IsTargetInRange = false;
    }

    private void Awake()
    {
        if(!animator) animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = StopDistance; //attackRange;
        agent.baseOffset = -.1f;

        if(!describable) describable = GetComponentInChildren<Describable>();
    }

    private void Update()
    {
        MoveToTarget();
    }

    public void SetTarget(Transform target)
    {
        targetTransform = target;

        if (target == null)
        {
            IsTargetInRange = false;
            return;
        }

        // LLM
        Describable other = target.GetComponentInChildren<Describable>();
        if (other != null && IsTargetInRange)
        {
            describable.OnEvent?.Invoke("[" + describable.Name + "] currently targeting [" + other.Name + "]");
            describable.InitialReport = "[" + describable.Name + " currently targeting " + other.Name + "]";
        }
    }

    private void MoveToTarget()
    {
        if (targetTransform == null)
        {
            IsTargetInRange = false;
            return;
        }

        if (!targetTransform.gameObject.activeInHierarchy)
        {
            targetTransform = null;
            return;
        }

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

        // Solution : Look rotation viewing vector is zero
        if (targetDirection == Vector3.zero) return;

        lookRotation = Quaternion.LookRotation(new Vector3(targetDirection.x, 0f, targetDirection.z));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 175f * Time.deltaTime);
    }
}
