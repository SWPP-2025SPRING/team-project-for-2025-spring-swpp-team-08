using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stage2
{
    public class StackGround : MonoBehaviour
    {
        public float disappearDelay = 0.5f;
        public bool destroyCompletely = false;
        public static bool RESPAWN_START = true;
        private bool _triggered = false;

        void Update()
        {
            if (RESPAWN_START && !gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            Debug.Log("hi");
            if (!_triggered && collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("collision");
                _triggered = true;
                RESPAWN_START = false;
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
