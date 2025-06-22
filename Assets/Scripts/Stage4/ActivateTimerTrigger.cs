using System.Collections;
using UnityEngine;

namespace Stage4
{
    [RequireComponent(typeof(Collider))]
    public class ActivateTimerTrigger : MonoBehaviour
    {
        [Header("Timers")]
        public Timer[] timers; // Drag all your timer objects here

        [Header("Audio Clips")]
        public AudioClip sfxClip;
        public AudioClip newBgmClip;
        public bool playOnlyOnce = false; // Changed to false so it can be pressed again

        private bool _hasTriggered = false;

        private void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player") || (playOnlyOnce && _hasTriggered)) return;

            _hasTriggered = true;

            // Activate all timers
            if (timers != null)
            {
                foreach (Timer timer in timers)
                {
                    if (timer != null)
                    {
                        timer.Activate();
                    }
                }
            }

            if (GameManager.Instance != null)
            {
                if (sfxClip != null)
                {
                    PlaySfxAtLowerVolume(sfxClip, 0.6f);
                }

                if (newBgmClip != null)
                {
                    StartCoroutine(PlayDelayedBgm());
                }
            }
        }

        private void PlaySfxAtLowerVolume(AudioClip clip, float volumeScale)
        {
            GameManager.Instance.PlaySfxWithBgmPause(clip, 0.6f, 3.0f);
        }

        private IEnumerator PlayDelayedBgm()
        {
            yield return new WaitForSeconds(0.3f);
            GameManager.Instance.PlayBgm(newBgmClip);
        }
    }
}