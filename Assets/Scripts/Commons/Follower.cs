// Follower.cs
using UnityEngine;

public class Follower : MonoBehaviour
{
    [Header("Target To Follow")]
    [Tooltip("The object whose position we should copy. This should be your 'new_player' ball.")]
    public Transform targetToFollow;

    [Header("Movement Controller")]
    [Tooltip("The script that provides the input direction. This should be the Playermovement2 script on your ball.")]
    public Newplayercontroller playerMovementScript;

    [Header("Settings")]
    [Tooltip("How quickly the character turns to face the movement direction.")]
    public float rotationSpeed = 15f;

    [Tooltip("Fine-tune the character's position relative to the ball.")]
    public Vector3 positionOffset = Vector3.zero;

    void Start()
    {
        // Error checking
        if (targetToFollow == null)
        {
            Debug.LogError("Follower script is missing a 'Target To Follow'!", this);
            enabled = false;
        }
        if (playerMovementScript == null)
        {
            Debug.LogError("Follower script is missing the 'Player Movement Script' reference!", this);
            enabled = false;
        }
    }

    // LateUpdate runs after all physics and Update calls, which is perfect for this.
    void LateUpdate()
    {
        // --- POSITION LOGIC ---
        // Force our position to match the target's position, plus any offset.
        transform.position = targetToFollow.position + positionOffset;


        // --- ROTATION LOGIC ---
        // This part is the same as before, but now it's guaranteed to work
        // because we are not a child of the rolling ball.

        // Get the intended movement direction from the player script.
        Vector3 moveDirection = playerMovementScript.LastPlayerInputDirection;

        // Ensure we don't try to look in a zero direction.
        if (moveDirection != Vector3.zero)
        {
            // Calculate the target rotation to look in the move direction, always staying upright.
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);

            // Smoothly rotate this object towards the target rotation.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}