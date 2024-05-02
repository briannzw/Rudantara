using UnityEngine;
using UnityEngine.VFX;

public class WeaponAnim : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Character character;
    [SerializeField] private Transform weaponTransform;
    [SerializeField] private Transform weaponTransformAlt;
    [SerializeField] private SkillCaster skillCaster;

    [Header("VFX Parameters")]
    [SerializeField] private Vector3 size = Vector3.one;

    private HitController hitController;
    private HitController hitControllerAlt;

    private void Awake()
    {
        hitController = weaponTransform.GetComponentInChildren<HitController>(includeInactive: true);
        if(weaponTransformAlt) hitControllerAlt = weaponTransformAlt.GetComponentInChildren<HitController>(includeInactive: true);
    }

    public void SpawnVFX(GameObject VFX)
    {
        GameObject go = Instantiate(VFX, weaponTransform.transform.position, weaponTransform.transform.rotation);
        go.transform.localScale = size;
        Destroy(go, VFX.GetComponent<VisualEffect>().GetFloat("Lifetime"));

        if (!weaponTransformAlt) return;
        go = Instantiate(VFX, weaponTransformAlt.transform.position, weaponTransformAlt.transform.rotation);
        go.transform.localScale = size;
        Destroy(go, VFX.GetComponent<VisualEffect>().GetFloat("Lifetime"));
    }

    public void SpawnVFXGlobal(AnimationEvent myEvent)
    {
        GameObject VFX = (GameObject)myEvent.objectReferenceParameter;
        float offset = myEvent.floatParameter;
        GameObject go = Instantiate(VFX, transform.position + transform.forward * offset, Quaternion.identity);
        go.transform.forward = transform.forward;
        // Global Skill
        HitController hit = go.GetComponent<HitController>();
        if (hit != null)
        {
            hit.sourceChara = character;
        }
        Destroy(go, VFX.GetComponent<VisualEffect>().GetFloat("Lifetime"));
    }

    public void PlayAttackSound()
    {
        //weapon.PlaySound();
    }

    public void SetSkill(AnimationEvent myEvent)
    {
        if(myEvent.objectReferenceParameter == null) return;

        Skill skill = (Skill)myEvent.objectReferenceParameter;

        hitController.Skill = skill;
        hitController.Name = skill.Name;

        if (hitControllerAlt)
        {
            hitControllerAlt.Skill = skill;
            hitControllerAlt.Name = skill.Name;
        }
    }

    public void ResetSkill()
    {
        hitController.Skill = null;
        
        if(hitControllerAlt) hitControllerAlt.Skill = null;
    }

    public void CastSkill(AnimationEvent myEvent)
    {
        if (myEvent.objectReferenceParameter == null) return;

        Skill skill = (Skill)myEvent.objectReferenceParameter;

        skillCaster.Cast(skill);
    }
}
