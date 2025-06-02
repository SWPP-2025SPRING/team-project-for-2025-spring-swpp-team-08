using UnityEngine;

public class Playermovement2 : MonoBehaviour
{
    [Header("Movement")]
    public float forwardTorquePower = 25f;
    public float sidewaysTorquePowerBase = 12f;
    public float maxAngularVelocity = 30f;
    public float linearDrag = 0.3f;
    public float angularDrag = 0.2f;

    [Header("Jumping")]
    public float jumpForce = 10f;
    public KeyCode jumpKey = KeyCode.Space;
    public float groundCheckDistance = 0.7f; // Distance from pivot to check for ground (adjust based on ball radius)
    public LayerMask groundLayer; // Set this to your ground layer in the Inspector

    [Header("Jitter Reduction")]
    public float jitterDampeningFactor = 0.5f; // How much to dampen small upward velocities when grounded
    public float maxJitterVelocity = 0.5f; // Upward velocities below this (when grounded and not jumping) are considered jitter

    [Header("Steering Boost at Speed")]
    public float highSpeedThresholdForSteeringBoost = 5f;
    public float steeringBoostFactor = 1.5f;

    [Header("Camera")]
    public Transform cameraTransform;

    [Header("Physics Material")]
    public PhysicMaterial highFrictionMaterial;

    [Header("Visuals")]
    public GameObject arrowPrefab;
    public bool hideBallMesh = true;
    public float arrowRotationSpeed = 20f;

    private Rigidbody rb;
    private Collider col;
    private MeshRenderer meshRenderer;
    private Vector3 lastPlayerInputDirection;
    private GameObject arrowInstance;
    private bool isGrounded;
    private bool tryingToJumpThisFrame; // Flag to ensure jitter reduction doesn't fight jump

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();

        // Null checks
        if (rb == null) { Debug.LogError("PlayerMovement2: No Rigidbody found!"); enabled = false; return; }
        if (col == null) { Debug.LogError("PlayerMovement2: No Collider found!"); enabled = false; return; }
        if (meshRenderer == null && hideBallMesh) { Debug.LogWarning("PlayerMovement2: No MeshRenderer found to hide!"); }
        if (highFrictionMaterial == null) { Debug.LogError("PlayerMovement2: High Friction Physic Material not assigned!"); enabled = false; return; }
        if (cameraTransform == null) { Debug.LogError("PlayerMovement2: Camera Transform not assigned!"); enabled = false; return; }
        if (groundLayer == 0) { Debug.LogWarning("PlayerMovement2: Ground Layer not assigned in the Inspector! Ground check and jitter reduction might not work correctly."); }


        rb.maxAngularVelocity = maxAngularVelocity;
        rb.drag = linearDrag;
        rb.angularDrag = angularDrag;

        col.material = highFrictionMaterial;
        lastPlayerInputDirection = GetInitialInputDirection();

        if (hideBallMesh && meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }

        if (arrowPrefab != null)
        {
            arrowInstance = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
            if (lastPlayerInputDirection != Vector3.zero)
            {
                arrowInstance.transform.rotation = Quaternion.LookRotation(lastPlayerInputDirection, Vector3.up);
            }
            arrowInstance.SetActive(true);
        }
        else
        {
            Debug.LogWarning("PlayerMovement2: Arrow Prefab not assigned.");
        }
    }

    Vector3 GetInitialInputDirection()
    {
        Vector3 initialDir = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        return (initialDir == Vector3.zero) ? Vector3.forward : initialDir;
    }

    void Update() // Using Update for KeyDown as it's more responsive for single press events
    {
        tryingToJumpThisFrame = false; // Reset jump attempt flag
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            tryingToJumpThisFrame = true; // Set flag before FixedUpdate physics
        }
    }

    void FixedUpdate()
    {
        PerformGroundCheck();

        if (tryingToJumpThisFrame)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            tryingToJumpThisFrame = false; // Consume jump action
        }

        // --- Movement Torque ---
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 camForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camRight = Vector3.Scale(cameraTransform.right, new Vector3(1, 0, 1)).normalized;
        Vector3 currentPlayerInputDirection = (camForward * v + camRight * h);

        if (currentPlayerInputDirection.magnitude >= 0.01f)
        {
            Vector3 normalizedCurrentPlayerInput = currentPlayerInputDirection.normalized;
            float actualSidewaysTorquePower = sidewaysTorquePowerBase;
            float currentSpeedAlongCamForward = Vector3.Dot(rb.velocity, camForward);

            if (Mathf.Abs(currentSpeedAlongCamForward) > highSpeedThresholdForSteeringBoost && Mathf.Abs(h) > 0.1f)
            {
                actualSidewaysTorquePower *= steeringBoostFactor;
            }

            Vector3 torqueFromV = new Vector3(camForward.z * v, 0, -camForward.x * v) * forwardTorquePower;
            Vector3 torqueFromH = new Vector3(camRight.z * h, 0, -camRight.x * h) * actualSidewaysTorquePower;
            rb.AddTorque(torqueFromV + torqueFromH);

            lastPlayerInputDirection = normalizedCurrentPlayerInput;
        }

        // --- Arrow Indicator Update ---
        if (arrowInstance != null)
        {
            arrowInstance.transform.position = transform.position;
            if (lastPlayerInputDirection != Vector3.zero) // Ensure lastPlayerInputDirection is not zero
            {
                 Quaternion targetArrowRotation = Quaternion.LookRotation(lastPlayerInputDirection, Vector3.up);
                 arrowInstance.transform.rotation = Quaternion.Slerp(arrowInstance.transform.rotation, targetArrowRotation, arrowRotationSpeed * Time.fixedDeltaTime);
            }
        }

        // --- Jitter Reduction ---
        // Apply only if grounded, not trying to jump, and has a small upward velocity (jitter)
        if (isGrounded && !tryingToJumpThisFrame && rb.velocity.y > 0 && rb.velocity.y < maxJitterVelocity)
        {
            // Dampen the upward velocity
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * (1 - jitterDampeningFactor), rb.velocity.z);
        }
    }

    void PerformGroundCheck()
    {
        // Raycast downwards to check for ground
        // It's good to start the raycast slightly above the pivot if the pivot isn't at the very bottom of the ball.
        // Or, ensure groundCheckDistance is appropriately set (e.g., ball radius + a small epsilon).
        // For simplicity, assuming pivot is at the center of the ball:
        Vector3 rayStart = transform.position; 
        isGrounded = Physics.Raycast(rayStart, Vector3.down, groundCheckDistance, groundLayer);
        
        // Optional: Visualize the ray in the editor
        #if UNITY_EDITOR
        Color rayColor = isGrounded ? Color.green : Color.red;
        Debug.DrawRay(rayStart, Vector3.down * groundCheckDistance, rayColor);
        #endif
    }


    void OnDisable()
    {
        if (arrowInstance != null)
        {
            arrowInstance.SetActive(false);
        }
    }

    void OnDestroy()
    {
        if (arrowInstance != null)
        {
            Destroy(arrowInstance);
        }
    }
}
