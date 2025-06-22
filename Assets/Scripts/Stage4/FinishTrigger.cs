using UnityEngine;

namespace Stage4
{
    public class FinishTrigger : MonoBehaviour
    {
        public Collider targetCollider;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            targetCollider.enabled = true;
            GameManager.Instance.playManager.FinishGame();
        }
    }
}
