using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stage2
{
    public class StackGround : MonoBehaviour
    {
        public float disappearDelay = 0.5f;
        public bool destroyCompletely = true;

        private bool _triggered = false;

        void OnCollisionEnter(Collision collision)
        {
            Debug.Log("hi");
            if (!_triggered && collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("collision");
                _triggered = true;
                Invoke(nameof(Disappear), disappearDelay);
            }
        }

        void Disappear()
        {
            if (destroyCompletely)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
