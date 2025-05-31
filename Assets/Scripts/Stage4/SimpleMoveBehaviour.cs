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
                var finalPosition = isDestinationRelative
                    ? transform.position + transform.rotation * destination
                    : destination;
                var elapsedTime = 0f;

                Debug.Log($"moving from {initialPosition} to {finalPosition} over {duration} seconds");

                while (elapsedTime <= duration)
                {
                    var position = Vector3.Lerp(initialPosition, finalPosition, elapsedTime / duration);
                    _rigidbody.MovePosition(position);

                    elapsedTime += Time.deltaTime;

                    yield return null;
                }

                _rigidbody.MovePosition(finalPosition);

                _rigidbody.velocity = Vector3.zero;
            }
        }
    }
}
