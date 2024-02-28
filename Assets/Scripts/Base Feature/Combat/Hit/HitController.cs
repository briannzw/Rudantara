using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class HitController : MonoBehaviour
{
    [Header("References")]
    public Character sourceChara;

    [Header("VFX")]
    [SerializeField] private GameObject hitVFX;

    private void Awake()
    {
        if(!sourceChara) sourceChara = GetComponentInParent<Character>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CompareTags(other))
        {
            Character chara = other.GetComponent<Character>();
            if (!chara) return;

            // Final Damage = Source Chara Damage - Target Chara Defense
            float finalDamage = sourceChara.GetDamage() - chara.CheckStat(StatEnum.Defense);
            if (finalDamage < 0) finalDamage = 0;
            chara.ChangeDynamicValue(DynamicStatEnum.Health, -finalDamage);

            if(chara.CheckStat(DynamicStatEnum.Health) <= 0f)
                sourceChara.OnCharacterKill?.Invoke(chara);

            // VFX
            if (hitVFX == null) return;
            Vector3 pos = other.ClosestPointOnBounds(transform.position);
            GameObject go = Instantiate(hitVFX, pos, Quaternion.identity);
            Destroy(go, hitVFX.GetComponent<VisualEffect>().GetFloat("Lifetime"));
        }
    }

    private bool CompareTags(Collider other)
    {
        foreach(var tag in sourceChara.EnemyTags)
        {
            if (other.CompareTag(tag)) return true;
        }

        return false;
    }
}
