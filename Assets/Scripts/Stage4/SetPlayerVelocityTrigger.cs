using UnityEngine;

namespace Stage4
{
    public class SetPlayerVelocityTrigger : MonoBehaviour
    {
        public Vector3 velocity;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            var playerRigidbody = other.GetComponent<Rigidbody>();

            if (playerRigidbody == null)
            {
                Debug.LogWarning("Player does not have a Rigidbody component.");
                return;
            }

            playerRigidbody.velocity = velocity;
        }
    }
}
