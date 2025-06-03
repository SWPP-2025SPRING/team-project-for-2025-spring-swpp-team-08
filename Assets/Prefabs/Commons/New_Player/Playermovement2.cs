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
    public LayerMask groundLayer; // Used for isGrounded check and jitter reduction

    [Header("Jitter Reduction")]
    public float jitterDampeningFactor = 0.5f;
    public float maxJitterVelocity = 0.5f;

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
    private bool tryingToJumpThisFrame; 
    private int groundContactCount = 0; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();

        if (rb == null) { enabled = false; return; }
        if (col == null) { enabled = false; return; }
        if (highFrictionMaterial == null) { enabled = false; return; }
        if (cameraTransform == null) { enabled = false; return; }
        
        rb.maxAngularVelocity = maxAngularVelocity;
        rb.drag = linearDrag;
        rb.angularDrag = angularDrag;

        if (col != null) 
        {
            col.material = highFrictionMaterial;
        }
        
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
        isGrounded = false;
    }

    Vector3 GetInitialInputDirection()
    {
        if (cameraTransform == null) return Vector3.forward; 
        Vector3 initialDir = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        return (initialDir == Vector3.zero) ? Vector3.forward : initialDir;
    }

    void Update()
    {
        // Check for jump input
        if (Input.GetKeyDown(jumpKey) && isGrounded) // MODIFIED: Added '&& isGrounded' back
        {
            tryingToJumpThisFrame = true; // Set the flag if jump key is pressed AND player is grounded
        }
    }

    void FixedUpdate()
    {
        if (tryingToJumpThisFrame)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            tryingToJumpThisFrame = false; // Consume the jump action; reset the flag HERE
        }

        // --- Movement Torque ---
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (cameraTransform != null) 
        {
            Vector3 camForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 camRight = Vector3.Scale(cameraTransform.right, new Vector3(1, 0, 1)).normalized;
            Vector3 currentPlayerInputDirection = (camForward * v + camRight * h);

            if (currentPlayerInputDirection.magnitude >= 0.01f)
            {
                Vector3 normalizedCurrentPlayerInput = currentPlayerInputDirection.normalized;
                float actualSidewaysTorquePower = sidewaysTorquePowerBase;
                if (rb != null) 
                {
                    float currentSpeedAlongCamForward = Vector3.Dot(rb.velocity, camForward);
                    if (Mathf.Abs(currentSpeedAlongCamForward) > highSpeedThresholdForSteeringBoost && Mathf.Abs(h) > 0.1f)
                    {
                        actualSidewaysTorquePower *= steeringBoostFactor;
                    }
                }

                Vector3 torqueFromV = new Vector3(camForward.z * v, 0, -camForward.x * v) * forwardTorquePower;
                Vector3 torqueFromH = new Vector3(camRight.z * h, 0, -camRight.x * h) * actualSidewaysTorquePower;
                if (rb != null) 
                {
                    rb.AddTorque(torqueFromV + torqueFromH);
                }
                lastPlayerInputDirection = normalizedCurrentPlayerInput;
            }
        }


        // --- Arrow Indicator Update ---
        if (arrowInstance != null)
        {
            arrowInstance.transform.position = transform.position;
            if (lastPlayerInputDirection != Vector3.zero)
            {
                 Quaternion targetArrowRotation = Quaternion.LookRotation(lastPlayerInputDirection, Vector3.up);
                 arrowInstance.transform.rotation = Quaternion.Slerp(arrowInstance.transform.rotation, targetArrowRotation, arrowRotationSpeed * Time.fixedDeltaTime);
            }
        }

        // --- Jitter Reduction ---
        if (isGrounded && !tryingToJumpThisFrame && rb != null && rb.velocity.y > 0 && rb.velocity.y < maxJitterVelocity)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * (1 - jitterDampeningFactor), rb.velocity.z);
        }
    }

    // Collision methods to manage isGrounded state
    void OnCollisionEnter(Collision collision)
    {
        if ((groundLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            groundContactCount++;
            isGrounded = (groundContactCount > 0);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if ((groundLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            if (!isGrounded) { // If somehow isGrounded is false but we are staying on ground
                isGrounded = true; 
            }
            if (groundContactCount == 0 && isGrounded) { // If isGrounded is true but count is 0 (e.g. missed Enter)
                groundContactCount = 1; 
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if ((groundLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            groundContactCount--;
            if (groundContactCount < 0) 
            {
                groundContactCount = 0;
            }
            isGrounded = (groundContactCount > 0);
        }
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
