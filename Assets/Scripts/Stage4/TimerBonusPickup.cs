using UnityEngine;

namespace Stage4
{
    [RequireComponent(typeof(Collider))]
    public class TimerBonusPickup : MonoBehaviour
    {
        [Header("Timer Bonus Settings")]
        public Timer[] timers; // Drag your timer objects here
        public float bonusTime = 5f; // How many seconds to add
        
        [Header("Pickup Settings")]
        public bool destroyOnPickup = false; // Changed to false so we can respawn
        public bool playOnlyOnce = false; // Changed to false so it can be picked up multiple times
        public float respawnTime = 5f; // Time in seconds before respawning
        
        [Header("Audio Settings")]
        public AudioClip pickupSound;
        public float volume = 0.8f;
        public float soundDelay = 0f; // Optional delay before playing sound
        
        [Header("Visual Effects (Optional)")]
        public GameObject pickupEffect; // Particle effect or visual feedback
        
        private bool _hasBeenPickedUp = false;
        private AudioSource _audioSource;
        private float _respawnTimer = 0f;

        private void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
            
            // Get or add AudioSource component
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }
            
            // Configure AudioSource
            _audioSource.playOnAwake = false;
            _audioSource.volume = volume;
        }

        private void Update()
        {
            // Handle automatic respawning
            if (_hasBeenPickedUp && _respawnTimer > 0f)
            {
                _respawnTimer -= Time.deltaTime;
                
                if (_respawnTimer <= 0f)
                {
                    RespawnPickup();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            
            if (playOnlyOnce && _hasBeenPickedUp) return;

            _hasBeenPickedUp = true;
            
            // Play pickup sound
            PlayPickupSound();
            
            // Add time to all timers
            if (timers != null)
            {
                foreach (Timer timer in timers)
                {
                    if (timer != null)
                    {
                        timer.AddTime(bonusTime);
                    }
                }
            }
            
            // Show visual effect
            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, transform.rotation);
            }
            
            // Handle pickup behavior
            if (destroyOnPickup)
            {
                // Hide visuals immediately
                if (GetComponent<Renderer>() != null)
                    GetComponent<Renderer>().enabled = false;
                GetComponent<Collider>().enabled = false;
                
                // If we have a sound with delay, wait for it to play
                if (pickupSound != null)
                {
                    float destroyDelay = soundDelay + pickupSound.length;
                    Destroy(gameObject, destroyDelay);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                // Just hide the object and start respawn timer
                GetComponent<Renderer>().enabled = false;
                GetComponent<Collider>().enabled = false;
                _respawnTimer = respawnTime;
            }
            
            Debug.Log($"Added {bonusTime} seconds to timer(s)!");
        }
        
        // Method to respawn the pickup (called automatically or by Timer)
        public void RespawnPickup()
        {
            _hasBeenPickedUp = false;
            _respawnTimer = 0f;
            
            // Make sure all components are enabled
            if (GetComponent<Renderer>() != null)
                GetComponent<Renderer>().enabled = true;
            GetComponent<Collider>().enabled = true;
            
            // If the object was deactivated, reactivate it
            gameObject.SetActive(true);
            
            Debug.Log($"{gameObject.name} respawned!");
        }
        
        private void PlayPickupSound()
        {
            if (pickupSound != null && _audioSource != null)
            {
                if (soundDelay > 0f)
                {
                    _audioSource.clip = pickupSound;
                    _audioSource.PlayDelayed(soundDelay);
                }
                else
                {
                    _audioSource.PlayOneShot(pickupSound, volume);
                }
            }
        }
    }
}