using UnityEngine;

namespace Stage4
{
    public class ActivateTimerTrigger : MonoBehaviour
    {
        public Timer timer;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            timer.Activate();
        }
    }
}
