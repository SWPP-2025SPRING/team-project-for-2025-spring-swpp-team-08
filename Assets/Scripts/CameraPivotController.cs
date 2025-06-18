using UnityEngine;

public class CameraPivotController : MonoBehaviour
{
    public Transform player;
    public float mouseSensitivity = 3f;
    public float minPitch = -30f;
    public float maxPitch = 60f;

    private float _yaw = 0f;
    private float _pitch = 20f; // 기본 각도

    private void Start()
    {
        _yaw = transform.eulerAngles.y;
        _pitch = transform.eulerAngles.x;
    }

    private void LateUpdate()
    {
        if (GameManager.Instance.playManager.State == PlayStates.Playing)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            _yaw += mouseX;
            _pitch -= mouseY;
            _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
        }

        transform.position = player.position;
        transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);

    }
}
