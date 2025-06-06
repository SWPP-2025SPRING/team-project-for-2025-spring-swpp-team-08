using UnityEngine;

public class NewPlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float forwardTorquePower = 25f;
    public float sidewaysTorquePowerBase = 15f;
    public float maxAngularVelocity = 35f;
    public float linearDrag = 0.3f;
    public float angularDrag = 0.2f;

    [Header("Jumping")]
    public float jumpForce = 14f;
    public KeyCode jumpKey = KeyCode.Space;
    public float coyoteTime = 0.1f; // How long the player can jump after leaving the ground
    public float groundCheckDistance = 1.5f;
    public LayerMask groundLayer;

    [Header("Braking")]
    public KeyCode brakeKey = KeyCode.LeftShift;
    public float brakingAngularDrag = 1.5f;
    public float brakingLinearDrag = 0.8f;

    [Header("Steering Boost at Speed")]
    public float highSpeedThresholdForSteeringBoost = 5f;
    public float steeringBoostFactor = 1.5f;

    [Header("Camera")]
    public Transform New_MainCamera;

    [Header("Physics Material")]
    public PhysicMaterial highFrictionMaterial;

    [Header("Visuals")]
    public bool hideBallMesh = false; // Make sure this is false!

    public Vector3 LastPlayerInputDirection { get; private set; }

    private Rigidbody rb;
    private Collider col;
    private MeshRenderer meshRenderer;
    private bool isGrounded;
    private bool tryingToJumpThisFrame;
    private float coyoteTimeCounter; // NEW: Timer for coyote jump
    private float originalAngularDrag;
    private float originalLinearDrag;
    private float _fallTimer = 0f;
    public static bool RESPAWN_START = false;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();

        if (rb == null) { Debug.LogError("PlayerMovement2: No Rigidbody found!"); enabled = false; return; }
        if (col == null) { Debug.LogError("PlayerMovement2: No Collider found!"); enabled = false; return; }
        if (meshRenderer == null) { Debug.LogWarning("PlayerMovement2: No MeshRenderer found for the ball!"); }
        if (highFrictionMaterial == null) { Debug.LogError("PlayerMovement2: High Friction Physic Material not assigned!"); enabled = false; return; }
        if (New_MainCamera == null)
{
    GameObject cam = GameObject.Find("Main Camera");
    if (cam != null)
    {
        New_MainCamera = cam.transform;
    }
    else
    {
        Debug.LogError("PlayerMovement2: 'Main Camera' not found in scene, and no camera assigned.", this);
        enabled = false;
        return;
    }
}

        if (groundLayer == 0) { Debug.LogWarning("PlayerMovement2: Ground Layer not assigned!"); }

        rb.maxAngularVelocity = maxAngularVelocity;
        originalAngularDrag = angularDrag;
        originalLinearDrag = linearDrag;
        rb.angularDrag = originalAngularDrag;
        rb.drag = originalLinearDrag;

        if (col != null) { col.material = highFrictionMaterial; }
        LastPlayerInputDirection = GetInitialInputDirection();

        if (hideBallMesh && meshRenderer != null) { meshRenderer.enabled = false; }
    }

    Vector3 GetInitialInputDirection()
    {
        if (New_MainCamera == null) return Vector3.forward;
        Vector3 initialDir = Vector3.Scale(New_MainCamera.forward, new Vector3(1, 0, 1)).normalized;
        return (initialDir == Vector3.zero) ? Vector3.forward : initialDir;
    }

    void Update()
{
    // Handle jump input
    if (Input.GetKeyDown(jumpKey) && coyoteTimeCounter > 0f)
    {
        tryingToJumpThisFrame = true;
    }

    // --- Respawn Check Logic ---
    if (!isGrounded && rb.velocity.y < 0f)
    {
        _fallTimer += Time.deltaTime;
    }
    else
    {
        _fallTimer = 0f;
    }

    if (IsFallen())
    {
        Vector3 currentCheckpoint = GameManager.Instance.playManager.GetCurrentCheckpoint();
        MoveTo(currentCheckpoint);
        
        if (GameManager.Instance.playManager.stageNo == 2)
        {
            Stage2.StackGround.ResetAllFootsteps();
        }
    }
}

    void FixedUpdate()
    {
        PerformGroundCheck();

        // --- NEW: Coyote Time Logic ---
        // If we are on the ground, reset the coyote time counter.
        // If we are not, start counting it down.
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.fixedDeltaTime;
        }

        // Perform the jump if requested from Update()
        if (tryingToJumpThisFrame)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            tryingToJumpThisFrame = false;
            
            // NEW: Immediately set coyote time to 0 to prevent double jumps after leaving a ledge.
            coyoteTimeCounter = 0f;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 currentPlayerInputDirection = Vector3.zero;
        Vector3 camForward = Vector3.zero;
        Vector3 camRight = Vector3.zero;

        if (New_MainCamera != null)
        {
            camForward = Vector3.Scale(New_MainCamera.forward, new Vector3(1, 0, 1)).normalized;
            camRight = Vector3.Scale(New_MainCamera.right, new Vector3(1, 0, 1)).normalized;
            currentPlayerInputDirection = (camForward * v + camRight * h);

            if (currentPlayerInputDirection.magnitude >= 0.01f)
            {
                LastPlayerInputDirection = currentPlayerInputDirection.normalized;
            }
        }

        bool isBraking = Input.GetKey(brakeKey);

        if (currentPlayerInputDirection.magnitude >= 0.01f)
        {
            float currentSidewaysTorquePower = sidewaysTorquePowerBase;
            float currentSpeedAlongCamForward = Vector3.Dot(rb.velocity, camForward);

            if (!isBraking && Mathf.Abs(currentSpeedAlongCamForward) > highSpeedThresholdForSteeringBoost && Mathf.Abs(h) > 0.1f)
            {
                currentSidewaysTorquePower *= steeringBoostFactor;
            }

            Vector3 torqueFromV = new Vector3(camForward.z * v, 0, -camForward.x * v) * forwardTorquePower;
            Vector3 torqueFromH = new Vector3(camRight.z * h, 0, -camRight.x * h) * currentSidewaysTorquePower;
            rb.AddTorque(torqueFromV + torqueFromH);
        }

        if (isBraking)
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
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    public void MoveTo(Vector3 position)
{
    rb.velocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;
    transform.position = position;
}

private bool IsFallen()
{
    float fallUnder = GameManager.Instance.playManager.fallThresholdHeight;
    return transform.position.y <= fallUnder || IsFallingTooLong();
}

private bool IsFallingTooLong()
{
    float fallSecond = GameManager.Instance.playManager.fallThresholdSecond;
    return _fallTimer >= fallSecond;
}

}