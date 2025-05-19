using UnityEngine;

namespace Stage4
{
    public class ActivateButton : MonoBehaviour
    {
        public PredefinedBehaviour[] behaviours;

        public bool isActivated;

        private void Start()
        {
            isActivated = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isActivated && other.CompareTag("Player"))
            {
                isActivated = true;
                Activate();
            }
        }

        public void Activate()
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.Perform();
            }
        }
    }
}
