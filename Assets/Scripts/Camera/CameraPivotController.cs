using UnityEngine;

public class CameraPivotController : MonoBehaviour
{
    public Transform player;
    public float mouseSensitivity = 3f;

    private float yaw = 0f;
    private float pitch = 20f; // 기본 각도
    public float minPitch = -30f;
    public float maxPitch = 60f;

    void Start()
    {
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    void LateUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.position = player.position;
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
