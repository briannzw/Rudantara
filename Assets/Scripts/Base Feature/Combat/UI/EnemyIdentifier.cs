using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyIdentifier : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Describable describable;
    [SerializeField] private EnemyCombatController controller;

    [Header("UI References")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image iconImage;

    private void OnEnable()
    {
        iconImage.enabled = false;
        controller.OnTaunted += SetIcon;
        Invoke(nameof(SetName), .1f);
    }

    private void SetName()
    {
        nameText.text = describable.Name;
    }

    private void OnDisable()
    {
        controller.OnTaunted -= SetIcon;
    }

    private void SetIcon(bool active)
    {
        iconImage.enabled = active;
    }
}
