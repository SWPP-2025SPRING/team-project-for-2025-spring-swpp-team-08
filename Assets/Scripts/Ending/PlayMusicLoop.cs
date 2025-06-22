using UnityEngine;

public class PlayMusicLoop : MonoBehaviour
{
    public AudioClip bgmClip;

    void Start()
    {
        if (bgmClip != null)
        {
            AudioSource bgmSource = GameManager.Instance.GetComponent<AudioSource>();
            if (bgmSource != null)
            {
                bgmSource.loop = true; 
            }

            GameManager.Instance.PlayBgm(bgmClip);
        }
    }
}
