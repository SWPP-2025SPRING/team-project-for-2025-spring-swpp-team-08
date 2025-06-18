using System.Collections;
using UnityEngine;

namespace Stage4
{
    public class SimpleMoveRestoreBehaviour : PredefinedBehaviour
    {
        public Vector3 destination;
        public Vector3 destinationRotation;
        public float duration;
        public float delayBeforeMove;
        public float delayAfterMove;
        public bool isDestinationRelative;

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        protected override void PerformInternal()
        {
            Rigidbody.isKinematic = true;

            StartCoroutine(MoveRestore());
            return;

            IEnumerator MoveRestore()
            {
                yield return new WaitForSeconds(delayBeforeMove);

                var initialPosition = transform.position;
                var initialRotation = transform.rotation;
                var finalPosition = isDestinationRelative
                    ? transform.position + transform.rotation * destination
                    : destination;
                var finalRotation = isDestinationRelative
                    ? transform.rotation * Quaternion.Euler(destinationRotation)
                    : Quaternion.Euler(destinationRotation);

                /* Move from initial to final position */
                var elapsedTime = 0f;

                while (elapsedTime <= duration)
                {
                    var progress = elapsedTime / duration;
                    var position = Vector3.Lerp(initialPosition, finalPosition, progress);
                    var rotation = Quaternion.Lerp(initialRotation, finalRotation, progress);
                    Rigidbody.MovePosition(position);
                    Rigidbody.MoveRotation(rotation);

                    elapsedTime += Time.deltaTime;

                    yield return null;
                }

                Rigidbody.MovePosition(finalPosition);
                Rigidbody.MoveRotation(finalRotation);
                Rigidbody.velocity = Vector3.zero;
                Rigidbody.angularVelocity = Vector3.zero;

                yield return new WaitForSeconds(delayAfterMove);

                /* Restore to initial position */
                elapsedTime = 0f;

                while (elapsedTime <= duration)
                {
                    var progress = elapsedTime / duration;
                    var position = Vector3.Lerp(finalPosition, initialPosition, progress);
                    Rigidbody.MovePosition(position);
                    Rigidbody.MoveRotation(initialRotation);

                    elapsedTime += Time.deltaTime;

                    yield return null;
                }

                Rigidbody.MovePosition(initialPosition);
                Rigidbody.MoveRotation(initialRotation);
                Rigidbody.velocity = Vector3.zero;
                Rigidbody.angularVelocity = Vector3.zero;
            }
        }
    }
}
