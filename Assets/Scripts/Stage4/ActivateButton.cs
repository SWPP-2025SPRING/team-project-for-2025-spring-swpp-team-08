using UnityEngine;

namespace Stage4
{
    public class ActivateButton : MonoBehaviour
    {
        public PredefinedBehaviour[] behaviours;

        public bool isActivated;
        public float reactivationDelay;    // Cannot be reactivated if set to 0

        private float _reactivationTimer;

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
            foreach (var behaviour in behaviours)
            {
                behaviour.Perform();
            }

            _reactivationTimer = reactivationDelay;
        }
    }
}
