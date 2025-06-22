using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class TransitionBehaviour : MonoBehaviour
{
    public bool isTransitioning;
    private Image _image;

    private void Awake()
    {
        _image = GetComponentInChildren<Image>();
    }

    public void StartTransition(float duration)
    {
        Debug.Log("StartTransition");
        StartCoroutine(FadeCoroutine(duration, Color.clear, Color.black));
    }

    public void EndTransition(float duration)
    {
        Debug.Log("EndTransition");
        StartCoroutine(FadeCoroutine(duration, Color.black, Color.clear));
    }

    private IEnumerator FadeCoroutine(float duration, Color initialColor, Color finalColor)
    {
        if (duration == 0f)
        {
            _image.color = finalColor;
            yield break;
        }

        isTransitioning = true;
        var elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            var progress = elapsedTime / duration;
            _image.color = Color.Lerp(initialColor, finalColor, progress);

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        _image.color = finalColor;
        isTransitioning = false;
    }
}
