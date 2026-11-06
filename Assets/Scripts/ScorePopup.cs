using System.Collections;
using TMPro;
using UnityEngine;

// Short text that floats up and fades out, e.g. when getting bonus points
[RequireComponent(typeof(TMP_Text))]
public class ScorePopup : MonoBehaviour
{
    private const float Duration = 1.0f;
    private const float RiseDistance = 0.15f;

    private TMP_Text text;
    private RectTransform rectTransform;
    private Vector2 startPosition;
    private Coroutine routine;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        rectTransform = (RectTransform)transform;
        startPosition = rectTransform.anchoredPosition;
    }

    public void Show(string message)
    {
        // Awake runs here the first time, since the popup starts inactive
        gameObject.SetActive(true);
        text.text = message;

        // A new popup restarts the animation instead of stacking
        if (routine != null)
        {
            StopCoroutine(routine);
        }
        routine = StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        for (var t = 0f; t < Duration; t += Time.deltaTime)
        {
            var progress = t / Duration;
            rectTransform.anchoredPosition = startPosition + Vector2.up * (RiseDistance * progress);
            // Fully visible for the first half, then fade out
            text.alpha = Mathf.Clamp01(2f * (1f - progress));
            yield return null;
        }

        routine = null;
        gameObject.SetActive(false);
    }
}
