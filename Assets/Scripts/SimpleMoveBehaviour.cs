using System.Collections;
using UnityEngine;

namespace Stage4
{
    public class SimpleMoveBehaviour : PredefinedBehaviour
    {
        public Vector3 destination;
        public float duration;
        public bool isDestinationRelative;

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        protected override void PerformInternal()
        {
            Rigidbody.isKinematic = true;

            StartCoroutine(StopAfterDelay());
            return;

            IEnumerator StopAfterDelay()
            {
                var initialPosition = transform.position;
                var finalPosition = isDestinationRelative
                    ? transform.position + transform.rotation * destination
                    : destination;
                var elapsedTime = 0f;

                Debug.Log($"moving from {initialPosition} to {finalPosition} over {duration} seconds");

                while (elapsedTime <= duration)
                {
                    var position = Vector3.Lerp(initialPosition, finalPosition, elapsedTime / duration);
                    Rigidbody.MovePosition(position);

                    elapsedTime += Time.deltaTime;

                    yield return null;
                }

                Rigidbody.MovePosition(finalPosition);

                Rigidbody.velocity = Vector3.zero;
            }
        }
    }
}
