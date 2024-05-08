using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    [Header("References")]
    private Character character;
    [SerializeField] private Animator animator;
    [SerializeField] private List<GameObject> disabledGameObjectsAfterDie;
    [SerializeField] private bool disableAllComponents;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        character.OnCharacterDie += (Character chara) =>
        {
            animator.SetBool("Dead", true);
            GetComponent<Collider>().enabled = false;

            foreach(var obj in disabledGameObjectsAfterDie)
                obj.SetActive(false);

            foreach(var c in GetComponents<MonoBehaviour>())
                c.enabled = false;
        };
    }
}
