using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 30f;
    public float acceleration = 100f;
    public float deceleration = 100f;
    public float fallThreshold = 5f;
    public Transform cameraPivot;

    private Rigidbody _rb;
    private Vector3 _inputDirection;
    private float _fallTimer = 0f;
    private bool _onContact = false;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        _inputDirection = new Vector3(h, 0, v).normalized;
        if (!_onContact && _rb.velocity.y < 0f)
        {
            _fallTimer += Time.deltaTime;
        }
        else
        {
            _fallTimer = 0f;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        _onContact = true;
    }

    void OnCollisionExit(Collision collision)
    {
        _onContact = false;
    }

    private void FixedUpdate()
    {
        Vector3 camForward = cameraPivot.forward;
        Vector3 camRight = cameraPivot.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 inputDir = _inputDirection.normalized;
        Vector3 moveDir = (camForward * inputDir.z + camRight * inputDir.x).normalized;
        Vector3 currentVelocity = _rb.velocity;
        Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
        float horizontalSpeed = horizontalVelocity.magnitude;

        bool grounded = IsGrounded();
        bool isSliding = ApplySlopeSlide();

        RaycastHit hit;
        bool onSlope = false;
        Vector3 slopeNormal = Vector3.up;

        if (grounded && Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f))
        {
            slopeNormal = hit.normal;
            float slopeAngle = Vector3.Angle(slopeNormal, Vector3.up);
            if (slopeAngle > 5f)
            {
                onSlope = true;
                moveDir = Vector3.ProjectOnPlane(moveDir, slopeNormal).normalized;
            }
        }

        if (_inputDirection.magnitude > 0 && grounded)
        {
            float alignment = Vector3.Dot(horizontalVelocity.normalized, moveDir);
            float slopeFactor = onSlope ? 0.5f : 1f;

            // 일반적인 경우 빠르게 가속
            if (horizontalSpeed < moveSpeed - 0.5f)
            {
                Vector3 desiredVelocity = moveDir * moveSpeed;
                Vector3 velocityDelta = desiredVelocity - horizontalVelocity;
                Vector3 force = Vector3.ClampMagnitude(velocityDelta * acceleration * slopeFactor, acceleration);
                _rb.AddForce(force, ForceMode.Acceleration);
            }
            // 경사면 이동 등으로 인해 최대 속도를 초과한 상태에서 input이 있다면, 최대 속도에 천천히 수렴
            else if (alignment > 0.9f && horizontalSpeed > moveSpeed + 0.5f)
            {
                float excess = horizontalSpeed - moveSpeed;
                float dampingStrength = 0.003f;
                Vector3 decel = -horizontalVelocity.normalized * (excess * dampingStrength);
                _rb.AddForce(decel, ForceMode.Acceleration);
            }
            // 최대 속도를 초과한 상태에서 반대방향 키를 누르면 빠르게 멈춤
            else
            {
                Vector3 steer = (moveDir - horizontalVelocity.normalized) * acceleration * 0.5f * slopeFactor;
                _rb.AddForce(steer, ForceMode.Acceleration);
            }
        }

        // 평지에서 입력 없으면 감속
        if (!isSliding && grounded)
        {
            bool inputExists = _inputDirection.magnitude > 0;
            bool oppositeDir = inputExists && Vector3.Dot(horizontalVelocity, moveDir) < 0;

            if (!inputExists || oppositeDir)
            {
                if (horizontalSpeed > moveSpeed)
                {
                    // 평지에서 고속 + 무입력 → 빠르게 감속
                    Vector3 decel = -horizontalVelocity.normalized * deceleration * 0.5f;
                    _rb.AddForce(decel, ForceMode.Acceleration);
                }
                else if (horizontalSpeed > 0.1f)
                {
                    // 중속 → 일반 감속
                    Vector3 decel = -horizontalVelocity.normalized * deceleration;
                    _rb.AddForce(decel, ForceMode.Acceleration);
                }
                else
                {
                    // 저속 → 확실히 멈춤
                    Vector3 stop = -horizontalVelocity.normalized * deceleration * 2f;
                    _rb.AddForce(stop, ForceMode.Acceleration);
                }
            }
        }

        // 공중 -> 미세 조정만 가능
        if (_inputDirection.magnitude > 0 && !grounded)
        {
            Vector3 desiredVelocity = moveDir * moveSpeed;
            Vector3 velocityDelta = desiredVelocity - horizontalVelocity;
            Vector3 force = Vector3.ClampMagnitude(velocityDelta * 0.2f, 1f);
            _rb.AddForce(force, ForceMode.Acceleration);
        }

        // Debug.Log($"|v|: {_rb.velocity.magnitude:F2}, horizontal: {horizontalSpeed:F2}, onSlope: {onSlope}, grounded: {grounded}");
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, out _, 1.2f);
    }

    private bool ApplySlopeSlide()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.1f))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            if (angle > 10f && _inputDirection.magnitude < 0.1f)
            {
                Vector3 slideDir = Vector3.ProjectOnPlane(Vector3.down, hit.normal).normalized;
                _rb.AddForce(slideDir * 30f, ForceMode.Acceleration);
                return true;
            }
        }
        return false;
    }

    public bool IsFallen()
    {
        return _rb.transform.position.y <= 0f || isFallingTooLong();
    }

    private bool isFallingTooLong()
    {
        return _fallTimer >= fallThreshold;
    }

    public void MoveTo(Vector3 position)
    {
        _rb.velocity = Vector3.zero;        
        _rb.angularVelocity = Vector3.zero; 
        transform.position = position;
    }

}
