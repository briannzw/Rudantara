using UnityEngine;
using UnityEngine.InputSystem;
using Module.Detector;
using System.Linq;

public class PlayerAutoTarget : PlayerInputControl
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private float aimArea = 5f;
    [SerializeField] private LayerMask targetMask;

    private Transform nearestTarget;

    #region Callbacks
    protected override void RegisterInputCallbacks()
    {
        if (playerControls == null) return;
        playerControls.Gameplay.Attack.performed += DoTarget;
    }

    protected override void UnregisterInputCallbacks()
    {
        if (playerControls == null) return;
        playerControls.Gameplay.Attack.performed -= DoTarget;
    }

    private void DoTarget(InputAction.CallbackContext context)
    {
        FaceTarget();
    }

    public void FaceTarget()
    {
        if (!animator.GetBool("CanMove")) return;

        var TargetList = ColliderDetector.Find<Transform>(transform.position, aimArea, targetMask);
        if (TargetList.Count > 0)
        {
            nearestTarget = TargetList.OrderBy(
                obj => (transform.position - obj.transform.position).sqrMagnitude).ToArray()[0];

            Vector3 direction = (nearestTarget.position - transform.position).normalized;

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        }
    }
    #endregion
}
