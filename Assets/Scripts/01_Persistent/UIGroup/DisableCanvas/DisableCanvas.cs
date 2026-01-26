using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class DisableCanvas : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private Coroutine fadeRoutine;
     
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        gameObject.SetActive(false);
    }
    public void ActiveFadeIn(Action onComplete = null, float duration = 1.5f)
    {
        StopFade();
         
        gameObject.SetActive(true);
        fadeRoutine = StartCoroutine(Fade(0f, 1f, duration, true, onComplete));
    }
    public void ActiveFadeOut(Action onComplete = null, float duration = 1.5f)
    {
        StopFade();

        gameObject.SetActive(true);
        fadeRoutine = StartCoroutine(Fade(1f, 0f, duration, false, () =>
        {
            gameObject.SetActive(false);
            onComplete?.Invoke();
        }));
    }
    public void ActiveInstant()
    {
        StopFade();

        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
    public void DisableInstant()
    {
        StopFade();
        gameObject.SetActive(false);
    } 

    private void StopFade()
    {
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
            fadeRoutine = null;
        }
    }
    private IEnumerator Fade(
        float from,
        float to,
        float duration,
        bool enableInteraction,
        Action onComplete)
    {
        canvasGroup.alpha = from;
        canvasGroup.interactable = enableInteraction;
        canvasGroup.blocksRaycasts = enableInteraction;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }

        canvasGroup.alpha = to;
        onComplete?.Invoke();
    }
} 
