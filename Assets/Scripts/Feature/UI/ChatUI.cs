using System.Collections;
using TMPro;
using UnityEngine;

public class ChatUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PersonalityAction actionModule;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text lineText;

    [Header("Parameters")]
    [SerializeField] private float showDuration = 3f;
    [SerializeField] private float hideTransition = 1.5f;
    [SerializeField] private float showTransition = .2f;

    private void Awake()
    {
        canvasGroup.alpha = 0f;
    }

    private void Start()
    {
        actionModule.OnResponseReceived += (value) => ShowText(value.Line);
    }

    public void ShowText(string text)
    {
        StopAllCoroutines();
        lineText.text = text;
        StartCoroutine(SetAlpha(1));
    }

    private IEnumerator SetAlpha(float alpha)
    {
        while (Mathf.Abs(canvasGroup.alpha - alpha) > 0)
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, alpha, Time.deltaTime / (alpha < .5f ? hideTransition : showTransition));
            yield return null;
        }

        // Hide after
        if (alpha < .5f) yield break;
        
        yield return new WaitForSeconds(showDuration);

        StartCoroutine(SetAlpha(0f));
    }
}
