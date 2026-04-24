using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class EndMenuFadeToCredits : MonoBehaviour
{
    public Image fadeImage;
    public float delayBeforeStart = 2f;
    public float fadeInDuration = 1f;
    public float fadeOutDuration = 1f;
    public UnityEvent onFadeInComplete;

    private void Start()
    {
        StartCoroutine(FadeSequence());
    }

    private IEnumerator FadeSequence()
    {
        SetAlpha(0f);
        yield return new WaitForSeconds(delayBeforeStart);
        yield return StartCoroutine(Fade(0f, 1f, fadeInDuration));
        onFadeInComplete?.Invoke();
        yield return StartCoroutine(Fade(1f, 0f, fadeOutDuration));
    }

    private IEnumerator Fade(float start, float end, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            float alpha = Mathf.Lerp(start, end, t);
            SetAlpha(alpha);

            yield return null;
        }

        SetAlpha(end);
    }

    private void SetAlpha(float alpha)
    {
        Color c = fadeImage.color;
        c.a = alpha;
        fadeImage.color = c;
    }
}