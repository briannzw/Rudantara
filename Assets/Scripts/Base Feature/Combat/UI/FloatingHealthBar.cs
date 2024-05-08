using UnityEngine;

public class FloatingHealthBar : HealthBar
{
    private void LateUpdate()
    {
        transform.LookAt(transform.position + Camera.main.transform.forward);
    }
}
