// Follower.cs
using UnityEngine;

public class BallFollower : MonoBehaviour
{
    // These are now private as they will be found automatically by the script.
    private Transform targetToFollow;
    private NewPlayerControl playerMovementScript;

    [Header("Settings")]
    [Tooltip("How quickly the character turns to face the movement direction.")]
    public float rotationSpeed = 15f;

    [Tooltip("Fine-tune the character's position relative to the ball.")]
    public Vector3 positionOffset = Vector3.zero;

    [Tooltip("The name of the sibling object to follow.")]
    public string ballObjectName = "Ball";

    void Start()
    {
        // --- AUTOMATIC FIND LOGIC ---

        // First, check if this object even has a parent.
        if (transform.parent == null)
        {
            Debug.LogError("This script is on an object with no parent, so it cannot find a sibling Ball object!", this);
            enabled = false;
            return;
        }

        // 1. Get the parent's transform, and THEN find the child object named "Ball".
        targetToFollow = transform.parent.Find(ballObjectName); // <-- THE FIX IS HERE

        // 2. Check if the ball was found.
        if (targetToFollow == null)
        {
            Debug.LogError($"Follower script could not find a sibling object named '{ballObjectName}'! Make sure it's under the same parent.", this);
            enabled = false; // Disable this script to prevent errors.
            return; // Stop the Start() method here.
        }

        // 3. Get the movement script component from the ball we just found.
        playerMovementScript = targetToFollow.GetComponent<NewPlayerControl>();

        // 4. Check if the script was found on the ball.
        if (playerMovementScript == null)
        {
            Debug.LogError($"Follower script could not find the 'NewPlayerController' component on the '{ballObjectName}' object!", this);
            enabled = false; // Disable this script.
        }
    }

    // LateUpdate runs after all physics and Update calls, which is perfect for this.
    void LateUpdate()
    {
        // This check is important in case Start() disabled the script.
        if (targetToFollow == null) return;

        // --- POSITION LOGIC ---
        transform.position = targetToFollow.position + positionOffset;

        // --- ROTATION LOGIC ---
        Vector3 moveDirection = playerMovementScript.LastPlayerInputDirection;

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}