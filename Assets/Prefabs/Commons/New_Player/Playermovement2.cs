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
    public float groundCheckDistance = 0.7f; // Distance from pivot to check for ground
    public LayerMask groundLayer; // Set this to your ground layer in the Inspector

    [Header("Braking")]
    public KeyCode brakeKey = KeyCode.LeftShift;
    public float brakingAngularDrag = 1.5f; 
    public float brakingLinearDrag = 0.8f;   

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

    // Removed Wall Interaction Header and its public variables

    private Rigidbody rb;
    private Collider col;
    private MeshRenderer meshRenderer;
    private Vector3 lastPlayerInputDirection;
    private GameObject arrowInstance;
    private bool isGrounded;
    private bool tryingToJumpThisFrame; 

    // Store original drag values
    private float originalAngularDrag;
    private float originalLinearDrag;

    // Removed private variables for Wall Interaction
    

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
        if (groundLayer == 0) { Debug.LogWarning("PlayerMovement2: Ground Layer not assigned in the Inspector! Ground check might not work correctly."); }

        rb.maxAngularVelocity = maxAngularVelocity;
        
        // Store original drag values and then set initial drag
        originalAngularDrag = angularDrag;
        originalLinearDrag = linearDrag;
        rb.angularDrag = originalAngularDrag;
        rb.drag = originalLinearDrag;


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
        else
        {
            Debug.LogWarning("PlayerMovement2: Arrow Prefab not assigned.");
        }
    }

    Vector3 GetInitialInputDirection()
    {
        if (cameraTransform == null) return Vector3.forward; 
        Vector3 initialDir = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        return (initialDir == Vector3.zero) ? Vector3.forward : initialDir;
    }

    void Update() 
    {
        // Jump input detection
        if (Input.GetKeyDown(jumpKey))
        {
            if (isGrounded) 
            {
                tryingToJumpThisFrame = true; 
            }
        }
    }

    void FixedUpdate()
    {
        PerformGroundCheck(); // Ground check using Raycast

        HandleBraking(); // Apply braking logic

        if (tryingToJumpThisFrame)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            tryingToJumpThisFrame = false; // Consume jump action
        }
        
        // --- Input and Direction Update (for Arrow) ---
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        
        if (cameraTransform != null) 
        {
            Vector3 camForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 camRight = Vector3.Scale(cameraTransform.right, new Vector3(1, 0, 1)).normalized;
            Vector3 currentPlayerInputDirection = (camForward * v + camRight * h);

            if (currentPlayerInputDirection.magnitude >= 0.01f)
            {
                lastPlayerInputDirection = currentPlayerInputDirection.normalized;
            }
            // If there's no significant new input, lastPlayerInputDirection retains its previous value for the arrow.
            // Or, you could decide to make it point forward if input is zero:
            // else { lastPlayerInputDirection = camForward != Vector3.zero ? camForward : transform.forward; }


            // --- Movement Torque Application (Conditional on Braking) ---
            if (!Input.GetKey(brakeKey) || rb.angularVelocity.magnitude < 0.5f) 
            {
                if (currentPlayerInputDirection.magnitude >= 0.01f)
                {
                    // Use base torque powers directly
                    float currentForwardTorquePower = forwardTorquePower;
                    float currentSidewaysTorquePower = sidewaysTorquePowerBase;
                    
                    // Apply steering boost directly to currentSidewaysTorquePower
                    if (rb != null) 
                    {
                        float currentSpeedAlongCamForward = Vector3.Dot(rb.velocity, camForward);
                        if (Mathf.Abs(currentSpeedAlongCamForward) > highSpeedThresholdForSteeringBoost && Mathf.Abs(h) > 0.1f)
                        {
                            currentSidewaysTorquePower *= steeringBoostFactor;
                        }
                    }

                    Vector3 torqueFromV = new Vector3(camForward.z * v, 0, -camForward.x * v) * currentForwardTorquePower;
                    Vector3 torqueFromH = new Vector3(camRight.z * h, 0, -camRight.x * h) * currentSidewaysTorquePower; 
                    
                    if (rb != null) 
                    {
                        rb.AddTorque(torqueFromV + torqueFromH);
                    }
                }
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
    }

    void HandleBraking()
    {
        if (rb == null) return;

        if (Input.GetKey(brakeKey))
        {
            rb.angularDrag = brakingAngularDrag;
            rb.drag = brakingLinearDrag;
        }
        else
        {
            rb.angularDrag = originalAngularDrag;
            rb.drag = originalLinearDrag;
        }
    }

    void PerformGroundCheck()
    {
        Vector3 rayStart = transform.position; 
        isGrounded = Physics.Raycast(rayStart, Vector3.down, groundCheckDistance, groundLayer);
        
        #if UNITY_EDITOR
        // Keep this debug ray as it's useful for basic ground check visualization
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
