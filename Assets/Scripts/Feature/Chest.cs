using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{   
    Animator animator;
    public UpgradeUIManager upgradeUIManager;
    [SerializeField] private bool playerInRange = false; 

    void Start()
    {
        upgradeUIManager = GameManager.Instance.upgradeUIManager;
        animator = GetComponentInParent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && playerInRange)
        {
            StartCoroutine(OpenChest());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    IEnumerator OpenChest()
    {
        animator.Play("Open");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);
        
        upgradeUIManager.TriggerUI();
        Destroy(transform.parent.gameObject);
     
    }
}
