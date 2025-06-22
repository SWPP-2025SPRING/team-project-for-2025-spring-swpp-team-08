using UnityEngine;

namespace Stage4
{
    public class ActivateButton : MonoBehaviour
    {
        public PredefinedBehaviour[] behaviours;

        public bool isActivated;
        public float reactivationDelay;    // Cannot be reactivated if set to 0

        [Header("Audio Settings")]
        public AudioClip buttonClickSound;
        public float volume = 1f;
        public float soundDelay = 0f;      // Optional delay before playing sound

        private MeshRenderer _meshRenderer;
        private AudioSource _audioSource;

        private float _reactivationTimer;

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            
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

        private void Start()
        {
            isActivated = false;

            _reactivationTimer = reactivationDelay;
        }

        private void Update()
        {
            if (_reactivationTimer <= 0f)
            {
                return;
            }

            _reactivationTimer -= Time.deltaTime;

            if (_reactivationTimer <= 0f)
            {
                isActivated = false;
                _reactivationTimer = 0f;
                _meshRenderer.enabled = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isActivated || !other.CompareTag("Player"))
            {
                return;
            }

            isActivated = true;
            Activate();
        }

        public void Activate()
        {
            // Play button click sound
            PlayButtonSound();
            
            foreach (var behaviour in behaviours)
            {
                behaviour.Perform();
            }

            _reactivationTimer = reactivationDelay;
            _meshRenderer.enabled = false;
        }
        
        private void PlayButtonSound()
        {
            if (buttonClickSound != null && _audioSource != null)
            {
                if (soundDelay > 0f)
                {
                    _audioSource.clip = buttonClickSound;
                    _audioSource.PlayDelayed(soundDelay);
                }
                else
                {
                    _audioSource.PlayOneShot(buttonClickSound, volume);
                }
            }
        }
    }
}