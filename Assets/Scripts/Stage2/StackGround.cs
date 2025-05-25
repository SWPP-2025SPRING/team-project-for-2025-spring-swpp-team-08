using UnityEngine;
using System.Collections;

namespace Stage2
{
    public class StackGround : MonoBehaviour
    {
        public static bool RESPAWN_START = true;
        public float disappearDelay = 5f;
        private bool _disappearStarted = false;
        private Coroutine _disappearCoroutine;
        private bool _wasRespawnStart = true;
        private bool _resetCalled = false;

        private Renderer _renderer;
        private Collider _collider;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _collider = GetComponent<Collider>();
        }

        private void Update()
        {
            if (RESPAWN_START && !_wasRespawnStart && !_resetCalled)
            {
                _resetCalled = true;
                ResetAll();
            }
            if (!RESPAWN_START)
            {
                _resetCalled = false;
            }
            _wasRespawnStart = RESPAWN_START;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player") && !_disappearStarted)
            {
                _disappearStarted = true;
                _disappearCoroutine = StartCoroutine(DisappearAfterDelay(disappearDelay));
            }
        }

        private IEnumerator DisappearAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (_renderer != null) _renderer.enabled = false;
            if (_collider != null) _collider.enabled = false;
        }

        public void ResetAll()
        {
            Debug.Log("ResetAll called");

            if (_disappearCoroutine != null)
            {
                StopCoroutine(_disappearCoroutine);
                _disappearCoroutine = null;
            }

            _disappearStarted = false;

            if (_renderer != null) _renderer.enabled = true;
            if (_collider != null) _collider.enabled = true;
        }
    }
}
