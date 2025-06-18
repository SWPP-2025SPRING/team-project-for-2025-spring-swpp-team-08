using UnityEngine;

namespace Stage4
{
    public abstract class PredefinedBehaviour : MonoBehaviour
    {
        public bool performed;

        protected Rigidbody Rigidbody;

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            performed = false;
        }

        public void Perform()
        {
            if (performed)
            {
                return;
            }

            Debug.Log("Perform");
            PerformInternal();
        }

        protected abstract void PerformInternal();
    }
}
