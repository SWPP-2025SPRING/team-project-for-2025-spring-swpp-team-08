using System.Collections;
using UnityEngine;

namespace Stage4
{
    public class SimpleMoveBehaviour : PredefinedBehaviour
    {
        public Vector3 destination;
        public float duration;
        public bool isDestinationRelative;

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        protected override void PerformInternal()
        {
            _rigidbody.isKinematic = true;

            StartCoroutine(StopAfterDelay());
            return;

            IEnumerator StopAfterDelay()
            {
                var initialPosition = transform.position;
                var finalPosition = isDestinationRelative ? initialPosition + destination : destination;
                var elapsedTime = 0f;

                while (elapsedTime <= duration)
                {
                    transform.position = Vector3.Lerp(initialPosition, finalPosition, elapsedTime / duration);

                    elapsedTime += Time.deltaTime;

                    yield return null;
                }

                transform.position = finalPosition;

                _rigidbody.velocity = Vector3.zero;
            }
        }
    }
}
