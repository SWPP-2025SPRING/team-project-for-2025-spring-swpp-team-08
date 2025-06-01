using System.Collections;
using UnityEngine;

namespace Stage4
{
    public class StopMoveBehaviour : PredefinedBehaviour
    {
        public float duration;

        public Vector3 initialVelocity;
        public Vector3 initialAngularVelocity;

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            Rigidbody.velocity = initialVelocity;
            Rigidbody.angularVelocity = initialAngularVelocity;
        }

        protected override void PerformInternal()
        {
            StartCoroutine(StopMoveCoroutine());
            return;

            IEnumerator StopMoveCoroutine()
            {
                var initialVelocity = Rigidbody.velocity;
                var initialAngularVelocity = Rigidbody.angularVelocity;
                var elapsedTime = 0f;

                while (elapsedTime < duration)
                {
                    var progress = elapsedTime / duration;
                    Rigidbody.velocity = Vector3.Lerp(initialVelocity, Vector3.zero, progress);
                    Rigidbody.angularVelocity = Vector3.Lerp(initialAngularVelocity, Vector3.zero, progress);

                    elapsedTime += Time.deltaTime;

                    yield return null;
                }

                Rigidbody.velocity = Vector3.zero;
                Rigidbody.angularVelocity = Vector3.zero;
                Rigidbody.isKinematic = true;
            }
        }
    }
}
