using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageUIFadeInFadeOut : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    private float _fadeDuration = 1f;
    private float _visibleDuration = 2f;

    public void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
    }

    public IEnumerator FadeInOut()
    {
        // fade in
        float t = 0f;
        while (t < _fadeDuration)
        {
            t += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / _fadeDuration);
            yield return null;
        }
        _canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(_visibleDuration);

        // fade out
        t = 0f;
        while (t < _fadeDuration)
        {
            t += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / _fadeDuration);
            yield return null;
        }
        _canvasGroup.alpha = 0f;
    }
}
