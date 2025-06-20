using System.Collections;

using UnityEngine;

public class CameraResultPosition : MonoBehaviour
{
    public Vector3 finalOffset;
    public float rotationOffsetAngle;
    public float delayBeforeMove;
    public float duration;

    private bool _isAfterMove;
    private Vector3 _finalOffsetGlobal;

    private Transform _characterTransform;
    private Transform _ballTransform;
    private Transform _cameraTransform;
    private SimpleOrbitCamera _simpleOrbitCamera;

    private void Awake()
    {
        _characterTransform = GameObject.Find("player_backpack").transform;
        _ballTransform = GameObject.FindWithTag("Player").transform;
        _cameraTransform = Camera.main?.transform;
        _simpleOrbitCamera = GetComponent<SimpleOrbitCamera>();
    }

    private void Start()
    {
        _isAfterMove = false;
    }

    private void LateUpdate()
    {
        if (!_isAfterMove) return;

        transform.position = _ballTransform.position + _finalOffsetGlobal;
    }

    public void MoveCamera()
    {
        var initialOffsetGlobal = _cameraTransform.position - _ballTransform.position;
        var finalOffsetGlobal = _characterTransform.rotation * finalOffset;
        var initialRotation = _cameraTransform.rotation;
        var finalRotation =
            Quaternion.LookRotation(Quaternion.AngleAxis(rotationOffsetAngle, Vector3.up) * -finalOffsetGlobal);

        _simpleOrbitCamera.enabled = false;

        StartCoroutine(PerformCoroutine());
        return;

        IEnumerator PerformCoroutine()
        {
            var elapsedTime = 0f;

            while (elapsedTime < delayBeforeMove)
            {
                _cameraTransform.position = _ballTransform.position + initialOffsetGlobal;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                var progress = elapsedTime / duration;
                var easeProgress = Mathf.Sin(progress * Mathf.PI / 2f);

                var offset = Vector3.Lerp(initialOffsetGlobal, finalOffsetGlobal, easeProgress);
                var rotation = Quaternion.Lerp(initialRotation, finalRotation, easeProgress);
                transform.position = _ballTransform.position + offset;
                transform.rotation = rotation;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = _ballTransform.position + finalOffsetGlobal;
            transform.rotation = finalRotation;

            _finalOffsetGlobal = finalOffsetGlobal;
            _isAfterMove = true;
        }
    }
}
