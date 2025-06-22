using TMPro;
using UnityEngine;

namespace Stage4
{
    public class Timer : MonoBehaviour
    {
        public float duration = 10f;
        
        [Header("Timer Complete Actions")]
        public GameObject[] objectsToRemove; // Objects to destroy/disable when timer ends
        public bool destroyObjects = true; // True = destroy, False = just disable
        
        [Header("Time Pickup Respawning")]
        public TimerBonusPickup[] timePickups; // All time pickups to respawn when timer reactivates

        private TMP_Text _timerText;
        private bool _isActive;
        private float _remainingTime;

        private void Awake()
        {
            _timerText = GetComponentInChildren<TMP_Text>();
            _remainingTime = duration;
        }

        private void Update()
        {
            if (!_isActive) return;

            _remainingTime -= Time.deltaTime;

            if (_remainingTime <= 0f)
            {
                _remainingTime = 0f;
                _isActive = false;
                OnTimerComplete();
            }

            _timerText.text = _remainingTime.ToString("0.0");

            var progress = _remainingTime / duration;
            _timerText.color = Color.Lerp(Color.red, Color.green, progress);
        }

        public void Activate()
        {
            Debug.Log($"Timer Activate called! Setting time to {duration}");
            _remainingTime = duration;
            _isActive = true;
            
            // When timer is activated, restore any removed objects
            RestoreObjects();
        }

        public void AddTime(float timeToAdd)
        {
            _remainingTime += timeToAdd;
            // Optional: Cap the time so it doesn't go over the original duration
            // _remainingTime = Mathf.Min(_remainingTime, duration);
        }
        
        private void OnTimerComplete()
        {
            Debug.Log("Timer completed! Removing objects...");
            
            if (objectsToRemove != null)
            {
                foreach (GameObject obj in objectsToRemove)
                {
                    if (obj != null)
                    {
                        if (destroyObjects)
                        {
                            Destroy(obj);
                        }
                        else
                        {
                            obj.SetActive(false);
                        }
                    }
                }
            }
        }
        
        private void RestoreObjects()
        {
            Debug.Log("Timer activated! Restoring objects and respawning pickups...");
            
            // Restore removed objects
            if (!destroyObjects && objectsToRemove != null)
            {
                foreach (GameObject obj in objectsToRemove)
                {
                    if (obj != null)
                    {
                        Debug.Log($"Restoring object: {obj.name}");
                        obj.SetActive(true);
                    }
                }
            }
            
            // Respawn all time pickups
            if (timePickups != null)
            {
                Debug.Log($"Respawning {timePickups.Length} time pickups...");
                foreach (TimerBonusPickup pickup in timePickups)
                {
                    if (pickup != null)
                    {
                        Debug.Log($"Respawning pickup: {pickup.name}");
                        pickup.RespawnPickup();
                    }
                    else
                    {
                        Debug.LogWarning("Null pickup found in timePickups array!");
                    }
                }
            }
            else
            {
                Debug.LogWarning("timePickups array is null or empty!");
            }
        }
    }
}