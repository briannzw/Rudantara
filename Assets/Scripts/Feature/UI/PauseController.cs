using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseController : PlayerInputControl
{
    [Header("References")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private PlayerMovementController playerMovementController;
    [SerializeField] private TMP_Text generatingText;
    [SerializeField] private TMP_Text iterationText;
    [SerializeField] private TMP_Text enemyBaseLevelText;

    [Header("Child References")]
    [SerializeField] private StatusUIController statusUIController;

    private Coroutine textAnim;

    private void Awake()
    {
        playerMovementController.enabled = false;
        textAnim = StartCoroutine(AnimateText());
        DungeonGenerator.Instance.OnDungeonComplete += () =>
        {
            generatingText.gameObject.SetActive(false);
            playerMovementController.enabled = true;
            StopCoroutine(textAnim);
        };
    }

    public void Open()
    {
        Time.timeScale = 0.0f;
        iterationText.text = "Iteration : " + GameManager.Instance.saveManager.SaveData.Iteration;
        enemyBaseLevelText.text = "Enemy Base Level : " + GameManager.Instance.LevelManager.CurrentEnemyBaseLevel;
        pauseMenu.SetActive(true);
        InputManager.ToggleActionMap(playerControls.Panel);
    }
    
    public void Close()
    {
        // Check childs
        if (statusUIController.gameObject.activeSelf)
        {
            statusUIController.Close();
            return;
        }

        Time.timeScale = 1.0f;
        pauseMenu.SetActive(false);

        InputManager.ToggleActionMap(playerControls.Gameplay);
    }

    private void Pause(InputAction.CallbackContext context)
    {
        if (pauseMenu.activeSelf) Close();
        else Open();
    }

    protected override void RegisterInputCallbacks()
    {
        if (playerControls == null) return;
        playerControls.Gameplay.Pause.performed += Pause;
        playerControls.Panel.Cancel.performed += Pause;
    }

    protected override void UnregisterInputCallbacks()
    {
        if (playerControls == null) return;
        playerControls.Gameplay.Pause.performed -= Pause;
        playerControls.Panel.Cancel.performed -= Pause;
    }

    private IEnumerator AnimateText()
    {
        int dot = 0;
        int maxDot = 3;
        while (true)
        {
            generatingText.text = "GENERATING MAP";
            for (int i = 0; i < dot; i++)
                generatingText.text += ".";
            dot++;
            dot %= maxDot + 1;
            yield return new WaitForSeconds(.15f);
        }
    }
}
