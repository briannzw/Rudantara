using System;
using UnityEngine;
using UnityEngine.VFX;

public class HitController : Describable
{
    [Header("References")]
    public Character sourceChara;
    public Skill Skill;

    [Header("VFX")]
    [SerializeField] private GameObject hitVFX;

    private bool hit = false;
    public Action<Transform> OnHit;

    private void Awake()
    {
        gameObject.tag = "Hit";
        if(!sourceChara) sourceChara = GetComponentInParent<Character>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CompareTags(other))
        {
            Character chara = other.GetComponent<Character>();
            if (!chara) return;

            // Final Damage = Source Chara Damage - Target Chara Defense
            float finalDamage = sourceChara.GetDamage();
            finalDamage *= Skill != null ? Skill.AttackPercent / 100f : 1f;
            finalDamage -= chara.CheckStat(StatEnum.Defense);
            if (finalDamage < 0) finalDamage = 0;
            chara.ChangeDynamicValue(DynamicStatEnum.Health, -finalDamage);

            if (chara.CheckStat(DynamicStatEnum.Health) <= 0f)
                sourceChara.OnCharacterKill?.Invoke(chara);

            Describable source = sourceChara.GetComponentInChildren<Describable>();
            Describable strucked = other.GetComponentInChildren<Describable>();
            if (source != null)
                OnEvent?.Invoke("You saw [" + source.Name + "]'s [" + (Skill ? Skill.Name : Name) + "] struck " + strucked.Name + ", dealing " + Mathf.RoundToInt(finalDamage).ToString() + " damage.");

            hit = true;
            OnHit?.Invoke(other.transform);

            // VFX
            if (hitVFX == null) return;
            Vector3 pos = other.ClosestPointOnBounds(transform.position);
            GameObject go = Instantiate(hitVFX, pos, Quaternion.identity);
            Destroy(go, hitVFX.GetComponent<VisualEffect>().GetFloat("Lifetime"));
        }
    }

    private void OnDisable()
    {
        if (hit) return;

        Describable source = sourceChara.GetComponentInChildren<Describable>();
        if (source != null)
            OnEvent?.Invoke("You saw [" + source.Name + "]'s [" + (Skill ? Skill.Name : Name) + "] didn't hit anything.");
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
