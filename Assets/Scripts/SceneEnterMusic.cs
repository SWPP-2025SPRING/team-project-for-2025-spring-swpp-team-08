using UnityEngine;
using System.Collections;

public class SceneEnterMusic : MonoBehaviour
{
    public AudioClip bgmClip;
    public int repeat = 6;
    public float fadeDuration = 3f;

    private AudioSource _bgmSource => GameManager.Instance.GetComponent<AudioSource>();

    void Start()
    {
        if (bgmClip != null)
        {
            StartCoroutine(PlayRepeatedFadeBgm());
        }
    }

    private IEnumerator PlayRepeatedFadeBgm()
    {

        for (int i = 0; i < repeat; i++)
        {
            GameManager.Instance.PlayBgm(bgmClip);
            _bgmSource.volume = 1f;

            float waitTime = bgmClip.length - fadeDuration;
            yield return new WaitForSeconds(Mathf.Max(0, waitTime));

            float timer = 0f;
            while (timer < fadeDuration)
            {
                _bgmSource.volume = Mathf.Lerp(1f, 0f, timer / fadeDuration);
                timer += Time.deltaTime;
                yield return null;
            }

            GameManager.Instance.StopBgm();
        }
    }
}
