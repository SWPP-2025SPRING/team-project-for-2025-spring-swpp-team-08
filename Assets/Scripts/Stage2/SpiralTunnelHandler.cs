using UnityEngine;

public class SpiralTunnelHandler : MonoBehaviour
{
    public float moveForce = 10f;
    public float stickForce = 30f;
    public float raycastDistance = 5f;
    public LayerMask tunnelLayer;
    public bool useConstantDirection = true;
    public Vector3 constantForwardDirection = Vector3.forward;

    private bool playerInside = false;
    private Rigidbody playerRb;
    private Transform player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.useGravity = false;
                playerInside = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerRb != null)
                playerRb.useGravity = true;

            playerInside = false;
            player = null;
            playerRb = null;
        }
    }

    private void FixedUpdate()
    {
        if (!playerInside || playerRb == null) return;

        // Step 1: Stick to the spiral mesh
        RaycastHit hit;
        if (Physics.Raycast(player.position, -player.up, out hit, raycastDistance, tunnelLayer))
        {
            Vector3 normal = hit.normal;
            Vector3 stickDirection = -normal;

            // Apply force to "stick" player to mesh
            playerRb.AddForce(stickDirection * stickForce, ForceMode.Acceleration);

            // Optional: Align player's 'up' direction with the surface normal
            Quaternion targetRotation = Quaternion.FromToRotation(player.up, normal) * player.rotation;
            player.rotation = Quaternion.Slerp(player.rotation, targetRotation, Time.fixedDeltaTime * 5f);
        }

        // Step 2: Move the player forward
        Vector3 moveDirection;
        if (useConstantDirection)
        {
            moveDirection = transform.TransformDirection(constantForwardDirection.normalized);
        }
        else
        {
            moveDirection = player.forward;
        }

        playerRb.AddForce(moveDirection * moveForce, ForceMode.Acceleration);
    }
}
