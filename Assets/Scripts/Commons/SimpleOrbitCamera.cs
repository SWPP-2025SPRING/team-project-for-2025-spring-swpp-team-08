using UnityEngine;

/// <summary>
/// A simple camera script that follows a target and allows orbiting around it with mouse input.
/// Attach this script directly to the Main Camera.
/// </summary>
public class SimpleOrbitCamera : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("The Transform the camera will follow and orbit around.")]
    public Transform target;

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
        if (target == null)
        {
            Debug.LogError("SimpleOrbitCamera: Target is not assigned! Please assign a target in the Inspector.", this);
            enabled = false; // Disable script if target is missing
            return;
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
        if (target == null)
        {
            return; // Do nothing if target is not assigned
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
        // Start at the target's position, then apply the offset rotated by the desired camera rotation.
        // The 'offset' vector determines the camera's position relative to the target
        // (e.g., behind and slightly above).
        Vector3 desiredPosition = target.position + (desiredRotation * offset);

        // --- Apply Position and Rotation to Camera ---
        transform.rotation = desiredRotation;
        transform.position = desiredPosition;
    }

    // Optional: Gizmo to visualize the offset and target line in the editor
    void OnDrawGizmosSelected()
    {
        if (target != null)
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

            Vector3 gizmoPosition = target.position + (gizmoRotation * offset);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(gizmoPosition, target.position); // Line from camera to target

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(target.position + (gizmoRotation * offset), 0.1f); // Sphere at camera position
            
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(target.position, 0.15f); // Sphere at target position
        }
    }
}
