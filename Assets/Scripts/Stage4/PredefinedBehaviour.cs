using UnityEngine;

namespace Stage4
{
    public abstract class PredefinedBehaviour : MonoBehaviour
    {
        public bool performed;

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
