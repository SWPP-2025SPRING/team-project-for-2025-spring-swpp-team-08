using System.Collections;
using UnityEngine;

namespace Stage4
{
    public class StopMoveBehaviour : PredefinedBehaviour
    {
        public float duration;

        public Vector3 initialVelocity;
        public Vector3 initialAngularVelocity;

        private Vector3 _velocity;
        private Vector3 _angularVelocity;

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            _velocity = initialVelocity;
            _angularVelocity = initialAngularVelocity;
        }

        private void FixedUpdate()
        {
            var deltaPosition = _velocity * Time.fixedDeltaTime;
            Rigidbody.MovePosition(transform.position + deltaPosition);

            var deltaAngle = Quaternion.Euler(Time.fixedDeltaTime * Mathf.Deg2Rad * _angularVelocity);
            Rigidbody.MoveRotation(Rigidbody.rotation * deltaAngle);
        }

        protected override void PerformInternal()
        {
            StartCoroutine(StopMoveCoroutine());
            return;

            IEnumerator StopMoveCoroutine()
            {
                var elapsedTime = 0f;

                while (elapsedTime < duration)
                {
                    var progress = elapsedTime / duration;
                    _velocity = initialVelocity * (1 - progress);
                    _angularVelocity = initialAngularVelocity * (1 - progress);

                    elapsedTime += Time.deltaTime;

                    yield return null;
                }

                _velocity = Vector3.zero;
                _angularVelocity = Vector3.zero;
            }
        }
    }
}
