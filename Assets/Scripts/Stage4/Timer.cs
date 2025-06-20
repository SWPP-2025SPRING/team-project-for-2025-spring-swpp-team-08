using TMPro;

using UnityEngine;

namespace Stage4
{
    public class Timer : MonoBehaviour
    {
        public float duration;

        private TMP_Text _timerText;
        private bool _isActive;
        private float _remainingTime;

        private void Awake()
        {
            _timerText = GetComponent<TMP_Text>();
        }

        private void Start()
        {
            _isActive = false;
        }

        private void Update()
        {
            if (!_isActive) return;

            _remainingTime -= Time.deltaTime;
            _timerText.text = _remainingTime.ToString("0.0");

            var progress = _remainingTime / duration;
            _timerText.color = Color.Lerp(Color.red, Color.green, progress);
        }

        public void Activate()
        {
            _remainingTime = duration;
            _isActive = true;
        }
    }
}
