using UnityEngine;

public class DoorBreakTrigger : MonoBehaviour
{
    [Header("Effect Settings")]
    public AudioClip breakSound;
    public GameObject breakEffectPrefab;
    public float destroyDelay = 2f;
    public float effectOffset = 1.5f;
    public float verticalOffset = 1f;
    public float soundVolume = 0.6f; // 60% volume

    [Header("Optional Linked Objects")]
    public GameObject textObjectToDestroy; // <- Reference to a 3D Text object

    private bool _hasBroken = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_hasBroken) return;

        if (other.CompareTag("Player"))
        {
            _hasBroken = true;

            // Play breaking sound at 60% volume
            if (breakSound != null)
            {
                GameObject soundObject = new GameObject("TempSound");
                soundObject.transform.position = transform.position;
                AudioSource audioSource = soundObject.AddComponent<AudioSource>();
                audioSource.clip = breakSound;
                audioSource.volume = soundVolume;
                audioSource.Play();
                Destroy(soundObject, breakSound.length);
            }

            // Spawn break effect opposite to player direction
            if (breakEffectPrefab != null)
            {
                Vector3 directionAwayFromPlayer = (transform.position - other.transform.position).normalized;
                Vector3 spawnPosition = transform.position + directionAwayFromPlayer * effectOffset;
                spawnPosition.y += verticalOffset;

                Instantiate(breakEffectPrefab, spawnPosition, Quaternion.identity);
            }

            // Optional: Destroy the linked 3D text object
            if (textObjectToDestroy != null)
            {
                Destroy(textObjectToDestroy);
            }

            // Destroy the door
            Destroy(gameObject, destroyDelay);
        }
    }
}
