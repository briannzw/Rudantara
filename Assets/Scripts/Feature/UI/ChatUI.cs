using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PersonalityAction actionModule;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text lineText;
    [SerializeField] private Image lineEmotionImage;

    [Header("Parameters")]
    [SerializeField] private float showDuration = 3f;
    [SerializeField] private float hideTransition = 1.5f;
    [SerializeField] private float showTransition = .2f;

    [Header("Emotion")]
    [SerializeField] private List<Sprite> emotionSprites = new();

    private void Awake()
    {
        canvasGroup.alpha = 0f;
    }

    private void Start()
    {
        actionModule.OnResponseReceived += (value) => ShowText(value.Line, value.EmotionIndex);
    }

    public void ShowText(string text, int emotion)
    {
        if (string.IsNullOrEmpty(text)) return;
        StopAllCoroutines();
        lineText.text = text;
        lineEmotionImage.sprite = emotionSprites[emotion];
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
