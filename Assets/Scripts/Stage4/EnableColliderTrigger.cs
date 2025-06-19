using UnityEngine;

namespace Stage4
{
    public class EnableColliderTrigger : MonoBehaviour
    {
        public Collider targetCollider;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            targetCollider.enabled = true;
        }
    }
}
