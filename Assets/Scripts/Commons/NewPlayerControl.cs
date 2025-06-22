using UnityEngine;

public class NewPlayerControl : MonoBehaviour
{
    [Header("Control")]
    public bool canControl;

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

    [Header("Audio Clips")]
    public AudioClip jumpAudio;
    public AudioClip landAudio;
    public AudioClip bumpAudio;
    public AudioClip cruiseAudio;

    [Header("Audio Volumes")]
    [Tooltip("Below this speed, cruise audio is completely silent.")]
    public float minSpeedForCruiseSound = 3f;
    [Range(0f, 1f)] public float jumpVolume = 0.7f;
    [Range(0f, 1f)] public float landVolume = 0.7f;
    [Range(0f, 1f)] public float bumpVolume = 1.0f;
    [Range(0f, 5f)] public float cruiseMaxVolume = 2.0f;
    [Min(0f)] public float cruiseVolumeMultiplier = 1.0f;

    [Header("Master Volume")]
    [Min(0f)] public float masterVolume = 1.0f;

    [Header("Audio Settings")]
    [Range(0f, 0.5f)] public float pitchRandomness = 0.1f;
    public float maxSpeedForCruiseAudio = 50f;
    public float minCruisePitch = 0.8f;
    public float maxCruisePitch = 1.5f;

    [Header("Landing Sound Settings")]
    public float minFallDistanceForSound = 0.5f;
    public float maxFallDistanceForSound = 15f;

    [Header("Bump Sound Settings")]
    public float minSpeedForBump = 10f;
    public float bumpSoundCooldown = 0.5f;
    [Range(0f, 1f)] public float wallAngleThreshold = 0.5f;

    [Header("Camera")]
    public Transform New_MainCamera;

    [Header("Physics Material")]
    public PhysicMaterial highFrictionMaterial;

    [Header("Visuals")]
    public bool hideBallMesh = false;

    public Vector3 LastPlayerInputDirection { get; private set; }

    protected Rigidbody rb;
    private Collider col;
    private MeshRenderer meshRenderer;
    private AudioSource sfxAudioSource;
    private AudioSource cruiseAudioSource;
    private AudioSource cruiseAudioSource2; // NEW
    private bool isGrounded;
    private bool wasGrounded;
    private bool tryingToJumpThisFrame;
    private bool enteredOutOfBoundsTrigger = false;
    private float coyoteTimeCounter;
    private float originalAngularDrag;
    private float originalLinearDrag;
    private float _fallTimer = 0f;
    private float originalPitch;
    private float lastBumpTime;
    private float peakFallHeight;

    protected void Start()
    {
        canControl = false;

        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();

        if (rb == null || col == null || highFrictionMaterial == null)
        {
            
            enabled = false;
            return;
        }

        if (New_MainCamera == null)
        {
            GameObject cam = GameObject.Find("Main Camera");
            if (cam != null) New_MainCamera = cam.transform;
            else { Debug.LogError("Main Camera not found."); enabled = false; return; }
        }

        rb.maxAngularVelocity = maxAngularVelocity;
        originalAngularDrag = angularDrag;
        originalLinearDrag = linearDrag;
        rb.angularDrag = originalAngularDrag;
        rb.drag = originalLinearDrag;
        col.material = highFrictionMaterial;

        LastPlayerInputDirection = GetInitialInputDirection();
        if (hideBallMesh && meshRenderer != null) meshRenderer.enabled = false;

        SetupAudioSources();
        lastBumpTime = -bumpSoundCooldown;
    }

    void SetupAudioSources()
    {
        sfxAudioSource = gameObject.AddComponent<AudioSource>();
        sfxAudioSource.playOnAwake = false;
        originalPitch = sfxAudioSource.pitch;

        cruiseAudioSource = gameObject.AddComponent<AudioSource>();
        cruiseAudioSource2 = gameObject.AddComponent<AudioSource>(); // NEW

        if (cruiseAudio != null)
        {
            cruiseAudioSource.clip = cruiseAudio;
            cruiseAudioSource.loop = true;
            cruiseAudioSource.Play();

            cruiseAudioSource2.clip = cruiseAudio; // NEW
            cruiseAudioSource2.loop = true;
            cruiseAudioSource2.Play(); // NEW
        }

        cruiseAudioSource.volume = 0;
        cruiseAudioSource2.volume = 0; // NEW
    }

    Vector3 GetInitialInputDirection()
    {
        if (New_MainCamera == null) return Vector3.forward;
        Vector3 initialDir = Vector3.Scale(New_MainCamera.forward, new Vector3(1, 0, 1)).normalized;
        return (initialDir == Vector3.zero) ? Vector3.forward : initialDir;
    }

    protected void Update()
    {

        if (canControl && Input.GetKeyDown(jumpKey) && coyoteTimeCounter > 0f)
        {
            tryingToJumpThisFrame = true;
        }

        if (!isGrounded && rb.velocity.y < 0f) _fallTimer += Time.deltaTime;
        else _fallTimer = 0f;

        if (IsFallen())
        {
            Vector3 currentCheckpoint = GameManager.Instance.playManager.GetCurrentCheckpoint();
            MoveTo(currentCheckpoint);
            if (GameManager.Instance.playManager.stageNo == 2)
            {
                Stage2.StackGround.ResetAllFootsteps();
            }
        }

        HandleCruiseAudio();
    }

    protected void FixedUpdate()
    {
        PerformGroundCheck();

        if (isGrounded) coyoteTimeCounter = coyoteTime;
        else coyoteTimeCounter -= Time.fixedDeltaTime;

        if (!isGrounded && wasGrounded) peakFallHeight = transform.position.y;
        if (!isGrounded && transform.position.y > peakFallHeight) peakFallHeight = transform.position.y;

        if (tryingToJumpThisFrame) Jump(jumpForce);

        float h = 0f, v = 0f;
        if (canControl)
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
        }

        Vector3 camForward = Vector3.Scale(New_MainCamera.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camRight = Vector3.Scale(New_MainCamera.right, new Vector3(1, 0, 1)).normalized;
        Vector3 inputDir = (camForward * v + camRight * h);

        if (inputDir.magnitude >= 0.01f)
            LastPlayerInputDirection = inputDir.normalized;

        bool isBraking = Input.GetKey(brakeKey);

        if (inputDir.magnitude >= 0.01f)
        {
            float currentSidewaysTorque = sidewaysTorquePowerBase;
            float currentSpeed = Vector3.Dot(rb.velocity, camForward);

            if (!isBraking && Mathf.Abs(currentSpeed) > highSpeedThresholdForSteeringBoost && Mathf.Abs(h) > 0.1f)
            {
                currentSidewaysTorque *= steeringBoostFactor;
            }

            Vector3 torqueV = new Vector3(camForward.z * v, 0, -camForward.x * v) * forwardTorquePower;
            Vector3 torqueH = new Vector3(camRight.z * h, 0, -camRight.x * h) * currentSidewaysTorque;
            rb.AddTorque(torqueV + torqueH);
        }

        rb.angularDrag = isBraking ? brakingAngularDrag : originalAngularDrag;
        rb.drag = isBraking ? brakingLinearDrag : originalLinearDrag;

        wasGrounded = isGrounded;
    }

    public void Jump(float magnitude)
    {
        rb.AddForce(Vector3.up * magnitude, ForceMode.Impulse);
        PlayJumpSound();
        tryingToJumpThisFrame = false;
        coyoteTimeCounter = 0f;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (Time.time < lastBumpTime + bumpSoundCooldown) return;

        float speed = collision.relativeVelocity.magnitude;
        if (speed < minSpeedForBump) return;

        float angle = Vector3.Dot(collision.contacts[0].normal, Vector3.up);
        if (Mathf.Abs(angle) > wallAngleThreshold) return;

        PlayBumpSound();
        lastBumpTime = Time.time;
    }

    void PerformGroundCheck()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        if (isGrounded && !wasGrounded)
        {
            float fallDistance = peakFallHeight - transform.position.y;
            if (fallDistance > minFallDistanceForSound)
            {
                float ratio = Mathf.InverseLerp(minFallDistanceForSound, maxFallDistanceForSound, fallDistance);
                float dynamicVolume = Mathf.Lerp(0.1f, landVolume, ratio);
                PlayLandSound(dynamicVolume);
            }
        }
    }

    void PlayJumpSound()
    {
        if (jumpAudio != null && sfxAudioSource != null)
        {
            sfxAudioSource.pitch = originalPitch + Random.Range(-pitchRandomness, pitchRandomness);
            sfxAudioSource.PlayOneShot(jumpAudio, jumpVolume * masterVolume);
        }
    }

    void PlayLandSound(float volume)
    {
        if (landAudio != null && sfxAudioSource != null)
        {
            sfxAudioSource.pitch = originalPitch + Random.Range(-pitchRandomness, pitchRandomness);
            sfxAudioSource.PlayOneShot(landAudio, volume * masterVolume);
        }
    }

    void PlayBumpSound()
    {
        if (bumpAudio != null && sfxAudioSource != null)
        {
            sfxAudioSource.pitch = originalPitch + Random.Range(-pitchRandomness, pitchRandomness);
            sfxAudioSource.PlayOneShot(bumpAudio, bumpVolume * masterVolume);
        }
    }

    void HandleCruiseAudio()
    {
        if (cruiseAudioSource == null || cruiseAudioSource2 == null) return;

        float speed = rb.velocity.magnitude;

        if (isGrounded && speed >= minSpeedForCruiseSound)
        {
            float speedRatio = Mathf.InverseLerp(minSpeedForCruiseSound, maxSpeedForCruiseAudio, speed);
            float volume = Mathf.Clamp01(speedRatio * cruiseMaxVolume * cruiseVolumeMultiplier * masterVolume);

            cruiseAudioSource.volume = volume;
            cruiseAudioSource2.volume = volume;
            cruiseAudioSource.pitch = Mathf.Lerp(minCruisePitch, maxCruisePitch, speedRatio);
            cruiseAudioSource2.pitch = cruiseAudioSource.pitch;
        }
        else
        {
            cruiseAudioSource.volume = Mathf.Lerp(cruiseAudioSource.volume, 0f, Time.deltaTime * 10f);
            cruiseAudioSource2.volume = Mathf.Lerp(cruiseAudioSource2.volume, 0f, Time.deltaTime * 10f);
        }
    }

    public void MoveTo(Vector3 position)
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = position;
        enteredOutOfBoundsTrigger = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("OutOfBounds"))
        {
            enteredOutOfBoundsTrigger = true;
        }
    }

    protected bool IsFallen()
    {
        float fallUnder = GameManager.Instance.playManager.fallThresholdHeight;
        return transform.position.y <= fallUnder || IsFallingTooLong() || enteredOutOfBoundsTrigger;
    }

    private bool IsFallingTooLong()
    {
        float fallSecond = GameManager.Instance.playManager.fallThresholdSecond;
        return _fallTimer >= fallSecond;
    }
}
