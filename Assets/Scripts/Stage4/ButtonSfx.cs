using UnityEngine;

public class ButtonSfx : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip soundClip;
    public float volume = 1f;
    public float delayTime = 0.5f;
    
    [Header("Trigger Settings")]
    public string playerTag = "Player"; // Only trigger for objects with this tag
    public bool playOnlyOnce = false; // Set to true if you want the sound to play only once
    
    private AudioSource audioSource;
    private bool hasPlayed = false;
    
    void Start()
    {
        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Configure AudioSource
        audioSource.clip = soundClip;
        audioSource.volume = volume;
        audioSource.playOnAwake = false;
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object has the correct tag
        if (other.CompareTag(playerTag))
        {
            // If playOnlyOnce is true, check if we've already played the sound
            if (playOnlyOnce && hasPlayed)
                return;
            
            // Play the sound with delay
            if (soundClip != null && audioSource != null)
            {
                audioSource.PlayDelayed(delayTime);
                hasPlayed = true;
            }
            else
            {
                Debug.LogWarning("Sound clip or AudioSource is missing on " + gameObject.name);
            }
        }
    }
    
    // Optional: Reset the hasPlayed flag if you want to allow the sound to play again
    public void ResetSound()
    {
        hasPlayed = false;
    }
}