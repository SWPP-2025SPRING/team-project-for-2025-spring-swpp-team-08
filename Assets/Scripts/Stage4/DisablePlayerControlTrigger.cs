using System.Collections;
using UnityEngine;

namespace Stage4
{
    public class DisablePlayerControlTrigger : MonoBehaviour
    {
        public float duration;

        private NewPlayerControl _playerControl;
        private Coroutine _coroutine;

        private void Awake()
        {
            _playerControl = GameObject.FindGameObjectWithTag("Player").GetComponent<NewPlayerControl>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (_coroutine != null) StopCoroutine(_coroutine);

            _coroutine = StartCoroutine(DisablePlayerControl());
            return;

            IEnumerator DisablePlayerControl()
            {
                _playerControl.canControl = false;
                Debug.Log("Player control disabled");

                yield return new WaitForSeconds(duration);

                _playerControl.canControl = true;
                Debug.Log("Player control enabled");
            }
        }
    }
}
