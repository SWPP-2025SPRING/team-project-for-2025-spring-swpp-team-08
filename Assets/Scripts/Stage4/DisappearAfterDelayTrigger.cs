using System.Collections;

using UnityEngine;

namespace Stage4
{
    public class DisappearAfterDelayTrigger : MonoBehaviour
    {
        public GameObject[] targetObjects;

        public float delay;

        private void Start()
        {
            SetObjectsActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            StartCoroutine(PerformCoroutine());
            Debug.Log("collided");
            return;

            IEnumerator PerformCoroutine()
            {
                SetObjectsActive(true);

                yield return new WaitForSeconds(delay);

                SetObjectsActive(false);
            }
        }

        private void SetObjectsActive(bool value)
        {
            foreach (var obj in targetObjects)
            {
                obj.SetActive(value);
            }
        }
    }
}
