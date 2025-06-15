using UnityEngine;

public class HihiController : MonoBehaviour
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
    public float coyoteTime = 0.1f;
    public float groundCheckDistance = 1.5f;
    public LayerMask groundLayer;

    [Header("Braking")]
    public KeyCode brakeKey = KeyCode.LeftShift;
    public float brakingAngularDrag = 1.5f;
    public float brakingLinearDrag = 0.8f;

    [Header("Steering Boost at Speed")]
    public float highSpeedThresholdForSteeringBoost = 5f;
    public float steeringBoostFactor = 1.5f;

    // --- AUDIO ---
    [Header("Audio Clips")]
    public AudioClip jumpAudio;
    public AudioClip landAudio;
    public AudioClip bumpAudio;
    public AudioClip cruiseAudio;

    [Header("Audio Volumes")]
    [Range(0f, 1f)] public float jumpVolume = 0.7f; // CHANGED: Default is now 70%
    [Range(0f, 1f)] public float landVolume = 0.7f; // CHANGED: Default is now 70% (This is now the MAX land volume)
    [Range(0f, 1f)] public float bumpVolume = 1.0f;
    [Range(0f, 1f)] public float cruiseMaxVolume = 1.0f;

    [Header("Audio Settings")]
    [Range(0f, 0.5f)] public float pitchRandomness = 0.1f;
    public float maxSpeedForCruiseAudio = 50f;
    public float minCruisePitch = 0.8f;
    public float maxCruisePitch = 1.5f;

    [Header("Landing Sound Settings")] // NEW: Settings for dynamic landing
    [Tooltip("The minimum distance the ball must fall to make any landing sound.")]
    public float minFallDistanceForSound = 0.5f;
    [Tooltip("The fall distance at which the landing sound will be at its maximum volume.")]
    public float maxFallDistanceForSound = 15f;

    [Header("Bump Sound Settings")]
    public float minSpeedForBump = 10f;
    public float bumpSoundCooldown = 0.5f;
    [Tooltip("An angle of 0 means a flat wall, 1 is flat ground. This prevents bumps on gentle slopes.")]
    [Range(0f, 1f)] public float wallAngleThreshold = 0.5f;

    [Header("Camera")]
    public Transform New_MainCamera;

    [Header("Physics Material")]
    public PhysicMaterial highFrictionMaterial;

    [Header("Visuals")]
    public bool hideBallMesh = false;

    public Vector3 LastPlayerInputDirection { get; private set; }

    // --- Private Variables ---
    private Rigidbody rb;
    private Collider col;
    private MeshRenderer meshRenderer;
    private AudioSource sfxAudioSource;
    private AudioSource cruiseAudioSource;
    private bool isGrounded;
    private bool wasGrounded;
    private bool tryingToJumpThisFrame;
    private float coyoteTimeCounter;
    private float originalAngularDrag;
    private float originalLinearDrag;
    private float _fallTimer = 0f;
    private float originalPitch;
    private float lastBumpTime;
    private float peakFallHeight; // NEW: To track the highest point during a fall/jump

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

        SetupAudioSources();

        lastBumpTime = -bumpSoundCooldown;
    }

    void SetupAudioSources()
    {
        sfxAudioSource = gameObject.AddComponent<AudioSource>();
        sfxAudioSource.playOnAwake = false;
        originalPitch = sfxAudioSource.pitch;

        cruiseAudioSource = gameObject.AddComponent<AudioSource>();
        if (cruiseAudio != null)
        {
            cruiseAudioSource.clip = cruiseAudio;
            cruiseAudioSource.loop = true;
            cruiseAudioSource.Play();
        }
        cruiseAudioSource.volume = 0;
    }

    Vector3 GetInitialInputDirection()
    {
        if (New_MainCamera == null) return Vector3.forward;
        Vector3 initialDir = Vector3.Scale(New_MainCamera.forward, new Vector3(1, 0, 1)).normalized;
        return (initialDir == Vector3.zero) ? Vector3.forward : initialDir;
    }

    void Update()
    {
        if (Input.GetKeyDown(jumpKey) && coyoteTimeCounter > 0f)
        {
            tryingToJumpThisFrame = true;
        }

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
            Vector3 currentCheckpoint = FakeGameManager.Instance.playManager.GetCurrentCheckpoint();
            MoveTo(currentCheckpoint);
            if (FakeGameManager.Instance.playManager.stageNo == 2)
            {
                FakeGameManager.Stage2.StackGround.ResetAllFootsteps();
            }
        }

        HandleCruiseAudio();
    }

    void FixedUpdate()
    {
        PerformGroundCheck();

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.fixedDeltaTime;
        }

        // NEW: Logic to track peak height for dynamic landing sound
        if (!isGrounded && wasGrounded)
        {
            // We just left the ground, record our starting height.
            peakFallHeight = transform.position.y;
        }
        if (!isGrounded)
        {
            // While in the air, if we are still going up, update the peak height.
            if (transform.position.y > peakFallHeight)
            {
                peakFallHeight = transform.position.y;
            }
        }

        if (tryingToJumpThisFrame)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            PlayJumpSound();
            tryingToJumpThisFrame = false;
            coyoteTimeCounter = 0f;
        }

        // --- Movement and Braking Logic (Unchanged) ---
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
        // --- End of Movement Logic ---

        wasGrounded = isGrounded;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (Time.time < lastBumpTime + bumpSoundCooldown)
        {
            return;
        }

        float collisionSpeed = collision.relativeVelocity.magnitude;
        if (collisionSpeed < minSpeedForBump)
        {
            return;
        }

        float collisionAngle = Vector3.Dot(collision.contacts[0].normal, Vector3.up);
        if (Mathf.Abs(collisionAngle) > wallAngleThreshold)
        {
            return;
        }

        PlayBumpSound();
        lastBumpTime = Time.time;
    }

    void PerformGroundCheck()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        // CHANGED: Landing logic is now much more robust
        if (isGrounded && !wasGrounded)
        {
            // We just landed. Calculate how far we fell.
            float fallDistance = peakFallHeight - transform.position.y;

            // Only play a sound if we fell more than the minimum distance.
            // This prevents the "bump-hop-land" issue.
            if (fallDistance > minFallDistanceForSound)
            {
                // Calculate volume based on fall distance.
                // A value from 0 (min fall) to 1 (max fall or more).
                float fallRatio = Mathf.InverseLerp(minFallDistanceForSound, maxFallDistanceForSound, fallDistance);

                // We use a low minimum volume so even small valid falls are audible.
                float dynamicVolume = Mathf.Lerp(0.1f, landVolume, fallRatio);

                PlayLandSound(dynamicVolume);
            }
        }
    }

    // --- Audio Handling Methods ---

    void PlayJumpSound()
    {
        if (jumpAudio != null && sfxAudioSource != null)
        {
            sfxAudioSource.pitch = originalPitch + Random.Range(-pitchRandomness, pitchRandomness);
            sfxAudioSource.PlayOneShot(jumpAudio, jumpVolume);
        }
    }

    // CHANGED: Now accepts a volume parameter for dynamic sound.
    void PlayLandSound(float volume)
    {
        if (landAudio != null && sfxAudioSource != null)
        {
            sfxAudioSource.pitch = originalPitch + Random.Range(-pitchRandomness, pitchRandomness);
            sfxAudioSource.PlayOneShot(landAudio, volume);
        }
    }

    void PlayBumpSound()
    {
        if (bumpAudio != null && sfxAudioSource != null)
        {
            sfxAudioSource.pitch = originalPitch + Random.Range(-pitchRandomness, pitchRandomness);
            sfxAudioSource.PlayOneShot(bumpAudio, bumpVolume);
        }
    }

    void HandleCruiseAudio()
    {
        if (cruiseAudioSource == null) return;

        if (isGrounded)
        {
            float speed = rb.velocity.magnitude;
            float speedRatio = Mathf.Clamp01(speed / maxSpeedForCruiseAudio);

            cruiseAudioSource.volume = speedRatio * cruiseMaxVolume;
            cruiseAudioSource.pitch = Mathf.Lerp(minCruisePitch, maxCruisePitch, speedRatio);
        }
        else
        {
            cruiseAudioSource.volume = Mathf.Lerp(cruiseAudioSource.volume, 0f, Time.deltaTime * 10f);
        }
    }

    // --- Utility Methods ---

    public void MoveTo(Vector3 position)
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = position;
    }

    private bool IsFallen()
    {
        float fallUnder = FakeGameManager.Instance.playManager.fallThresholdHeight;
        return transform.position.y <= fallUnder || IsFallingTooLong();
    }

    private bool IsFallingTooLong()
    {
        float fallSecond = FakeGameManager.Instance.playManager.fallThresholdSecond;
        return _fallTimer >= fallSecond;
    }

    // --- Placeholder GameManager ---
    private static class FakeGameManager
    {
        public static class Instance
        {
            public static class playManager
            {
                public static float fallThresholdHeight = -50f;
                public static float fallThresholdSecond = 8f;
                public static int stageNo = 1;
                public static Vector3 GetCurrentCheckpoint() { return new Vector3(0, 5, 0); } // Start a little higher up
            }
        }
        public static class Stage2 {
            public static class StackGround {
                public static void ResetAllFootsteps() { /* Debug.Log("Resetting footsteps"); */ }
            }
        }
    }
}
