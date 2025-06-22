using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BgmTriggerZone : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip sfxClip;
    public AudioClip newBgmClip;

    public bool playOnlyOnce = true;

    private bool _hasTriggered = false;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }
    private void PlaySfxAtLowerVolume(AudioClip clip, float volumeScale)
{
    
    if (sfxClip != null)
{
    GameManager.Instance.PlaySfxWithBgmPause(sfxClip, 0.3f, 3.0f); // volume 0.3, resume after 1 second

}

}


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || (playOnlyOnce && _hasTriggered)) return;

        _hasTriggered = true;

        if (GameManager.Instance != null)
        {
            if (sfxClip != null)
            {
                PlaySfxAtLowerVolume(sfxClip, 0.6f); // or any volume < 1.0
            }

            if (newBgmClip != null)
            {
                StartCoroutine(PlayDelayedBgm());
            }
        }
    }

    private IEnumerator PlayDelayedBgm()
    {
        yield return new WaitForSeconds(0.3f);
        GameManager.Instance.PlayBgm(newBgmClip);
    }


}
