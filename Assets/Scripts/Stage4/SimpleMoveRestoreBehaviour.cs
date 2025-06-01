using System.Collections;
using UnityEngine;

namespace Stage4
{
    public class SimpleMoveRestoreBehaviour : PredefinedBehaviour
    {
        public Vector3 destination;
        public float duration;
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
                var initialPosition = transform.position;
                var finalPosition = isDestinationRelative
                    ? transform.position + transform.rotation * destination
                    : destination;

                /* Move from initial to final position */
                var elapsedTime = 0f;

                while (elapsedTime <= duration)
                {
                    var position = Vector3.Lerp(initialPosition, finalPosition, elapsedTime / duration);
                    Rigidbody.MovePosition(position);

                    elapsedTime += Time.deltaTime;

                    yield return null;
                }

                Rigidbody.MovePosition(finalPosition);
                Rigidbody.velocity = Vector3.zero;

                yield return new WaitForSeconds(delayAfterMove);

                /* Restore to initial position */
                elapsedTime = 0f;

                while (elapsedTime <= duration)
                {
                    var position = Vector3.Lerp(finalPosition, initialPosition, elapsedTime / duration);
                    Rigidbody.MovePosition(position);

                    elapsedTime += Time.deltaTime;

                    yield return null;
                }

                Rigidbody.MovePosition(initialPosition);
                Rigidbody.velocity = Vector3.zero;
            }
        }
    }
}
