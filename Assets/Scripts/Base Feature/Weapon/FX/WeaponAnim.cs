using System.Drawing;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.VFX;

public class WeaponAnim : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Transform weaponTransform;
    //private Weapon weapon;

    [Header("VFX Parameters")]
    [SerializeField] private Vector3 size = Vector3.one;

    private void Start()
    {
        //weapon = WeaponTransform.GetComponentInChildren<Weapon>();
    }

    public void SpawnVFX(GameObject VFX)
    {
        GameObject go = Instantiate(VFX, weaponTransform.transform.position, weaponTransform.transform.rotation);
        go.transform.localScale = size;
        Destroy(go, VFX.GetComponent<VisualEffect>().GetFloat("Lifetime"));
    }

    public void SpawnVFXGlobal(AnimationEvent myEvent)
    {
        GameObject VFX = (GameObject)myEvent.objectReferenceParameter;
        float offset = myEvent.floatParameter;
        GameObject go = Instantiate(VFX, transform.position + transform.forward * offset, Quaternion.identity);
        go.transform.forward = transform.forward;
        //WeaponHit hit = go.GetComponent<WeaponHit>();
        //if (hit != null)
        //{
        //    hit.Weapon = weapon;
        //    hit.Skill = weapon.SkillMap[(hit.Skill.isBaseSkill) ? hit.Skill : hit.Skill.baseSkill];
        //}
        Destroy(go, VFX.GetComponent<VisualEffect>().GetFloat("Lifetime"));
    }

    public void PlayAttackSound()
    {
        //weapon.PlaySound();
    }

    public void ResetSkill()
    {
        //weapon.LocalHit.Skill = null;
    }
}
