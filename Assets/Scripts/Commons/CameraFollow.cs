using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The object the camera will follow
    public Vector3 offset = new Vector3(0f, 5f, -10f); // Offset from the target

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogError("SimpleCameraFollow: Target is not assigned!", this);
            enabled = false; // Disable script if target is missing
            return;
        }

        // Calculate the desired camera position
        // This is simply the target's position plus the offset
        Vector3 desiredPosition = target.position + offset;

        // Set the camera's position directly
        transform.position = desiredPosition;

        // Make the camera look at the target
        transform.LookAt(target);
    }

    // Optional: Gizmo to visualize the offset in the editor
    void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, target.position);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(target.position, target.position + offset);
        }
    }
}
