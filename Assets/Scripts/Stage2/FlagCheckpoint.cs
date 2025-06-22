using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FlagCheckpoint : MonoBehaviour
{
    [Header("Jingle Settings")]
    public AudioClip jingleSound;
    [Range(0f, 1f)] public float volume = 0.7f;

    private bool _hasTriggered = false;

    private void Start()
    {
        // Ensure the collider is a trigger
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            _hasTriggered = true;

            if (jingleSound != null)
            {
                GameObject tempSoundObj = new GameObject("TempFlagJingle");
                tempSoundObj.transform.position = transform.position;

                AudioSource audioSource = tempSoundObj.AddComponent<AudioSource>();
                audioSource.clip = jingleSound;
                audioSource.volume = volume;
                audioSource.Play();

                Destroy(tempSoundObj, jingleSound.length);
            }
        }
    }
}
