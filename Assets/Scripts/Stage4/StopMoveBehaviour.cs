using System.Collections;
using UnityEngine;

namespace Stage4
{
    public class StopMoveBehaviour : PredefinedBehaviour
    {
        public float duration;

        public Vector3 initialVelocity;
        public Vector3 initialAngularVelocity;

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            _rigidbody.velocity = initialVelocity;
            _rigidbody.angularVelocity = initialAngularVelocity;
        }

        protected override void PerformInternal()
        {
            StartCoroutine(StopMoveCoroutine());
            return;

            IEnumerator StopMoveCoroutine()
            {
                var initialVelocity = _rigidbody.velocity;
                var initialAngularVelocity = _rigidbody.angularVelocity;
                var elapsedTime = 0f;

                while (elapsedTime < duration)
                {
                    var progress = elapsedTime / duration;
                    _rigidbody.velocity = Vector3.Lerp(initialVelocity, Vector3.zero, progress);
                    _rigidbody.angularVelocity = Vector3.Lerp(initialAngularVelocity, Vector3.zero, progress);

                    elapsedTime += Time.deltaTime;

                    yield return null;
                }

                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
                _rigidbody.isKinematic = true;
            }
        }
    }
}
