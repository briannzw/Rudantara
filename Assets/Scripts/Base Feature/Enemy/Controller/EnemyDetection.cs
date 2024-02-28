using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    [SerializeField] private EnemyController controller;

    private void Awake()
    {
        //GetComponent<SphereCollider>().radius = controller.ChaseRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Companion"))
        {
            controller.SetTarget(other.transform);
        }
    }
}
