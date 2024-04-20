using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{ Animator animator;
    public UpgradeUIManager upgradeUIManager;
    [SerializeField] private bool playerInRange = false; // Flag to track if the player is in range

    void Start()
    {
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
        
        if (upgradeUIManager != null)
        {
            upgradeUIManager.TriggerUI();
            Destroy(transform.parent.gameObject);
        }
        else
        {
            Debug.LogWarning("UpgradeUIManager reference is not set in the Chest script.");
        }
    }
}
