using UnityEngine;

/// <summary>
/// A simple camera script that follows a Ball and allows orbiting around it with mouse input.
/// Attach this script directly to the Main Camera.
/// </summary>
public class SimpleOrbitCamera : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("The Transform the camera will follow and orbit around.")]
    public Transform Ball;

    [Header("Orbit & Offset Settings")]
    [Tooltip("The camera's position offset from the target. (X: sideways, Y: height, Z: distance). Z is typically negative.")]
    public Vector3 offset = new Vector3(0f, 1.5f, -5f);
    [Tooltip("How fast the camera rotates with mouse movement.")]
    public float mouseSensitivity = 3f;
    [Tooltip("Minimum vertical angle (pitch) the camera can look.")]
    public float minPitch = -30f;
    [Tooltip("Maximum vertical angle (pitch) the camera can look.")]
    public float maxPitch = 60f;

    private float _yaw = 0f;    // Rotation around the Y-axis (left/right)
    private float _pitch = 0f;  // Rotation around the X-axis (up/down)

    void Start()
    {
        if (Ball == null)
        {
            GameObject foundBall = GameObject.Find("Ball");
            if (foundBall != null)
            {
                Ball = foundBall.transform;
            }
            else
            {
                Debug.LogError("SimpleOrbitCamera: Could not find GameObject named 'Ball' and no target assigned in the Inspector.", this);
                enabled = false;
                return;
            }
        }


        // Initialize yaw and pitch based on the camera's initial rotation in the editor.
        // This allows you to set a starting camera view.
        Vector3 initialEulerAngles = transform.eulerAngles;
        _yaw = initialEulerAngles.y;
        _pitch = initialEulerAngles.x;

        // Optional: Lock and hide cursor for a cleaner experience
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (Ball == null)
        {
            return; // Do nothing if Ball is not assigned
        }

        // --- Handle Mouse Input for Rotation ---
        // Get mouse movement (delta)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Adjust yaw and pitch based on mouse input
        _yaw += mouseX;
        _pitch -= mouseY; // Subtract mouseY for natural up/down look

        // Clamp the pitch to prevent the camera from flipping over
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);

        // --- Calculate Camera Position and Rotation ---
        // Calculate the desired rotation of the camera
        Quaternion desiredRotation = Quaternion.Euler(_pitch, _yaw, 0f);

        // Calculate the desired position of the camera:
        // Start at the Ball's position, then apply the offset rotated by the desired camera rotation.
        // The 'offset' vector determines the camera's position relative to the Ball
        // (e.g., behind and slightly above).
        Vector3 desiredPosition = Ball.position + (desiredRotation * offset);

        // --- Apply Position and Rotation to Camera ---
        transform.rotation = desiredRotation;
        transform.position = desiredPosition;
    }

    // Optional: Gizmo to visualize the offset and Ball line in the editor
    void OnDrawGizmosSelected()
    {
        if (Ball != null)
        {
            // Calculate the rotation and position as it would be in LateUpdate
            Quaternion gizmoRotation = Quaternion.Euler(_pitch, _yaw, 0f);
            if (Application.isPlaying) // Use current _pitch and _yaw if playing
            {
                 gizmoRotation = Quaternion.Euler(_pitch, _yaw, 0f);
            }
            else // Use initial transform rotation if in editor and not playing
            {
                gizmoRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0f);
            }

            Vector3 gizmoPosition = Ball.position + (gizmoRotation * offset);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(gizmoPosition, Ball.position); // Line from camera to Ball

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(Ball.position + (gizmoRotation * offset), 0.1f); // Sphere at camera position

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(Ball.position, 0.15f); // Sphere at Ball position
        }
    }
    private Transform FindDeepChild(Transform parent, string name)
{
    foreach (Transform child in parent)
    {
        if (child.name == name)
            return child;

        Transform result = FindDeepChild(child, name);
        if (result != null)
            return result;
    }
    return null;
}
}
