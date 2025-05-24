using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 30f;
    public float acceleration = 100f;
    public float deceleration = 100f;
    public Transform cameraPivot;

    private Rigidbody _rb;
    private Vector3 _inputDirection;
    private float _fallTimer = 0f;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.centerOfMass = new Vector3(0, -0.5f, 0);
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        _inputDirection = new Vector3(h, 0, v).normalized;
        if (!IsGrounded() && _rb.velocity.y < 0f)
        {
            _fallTimer += Time.deltaTime;
        }
        else
        {
            _fallTimer = 0f;
        }
        if (IsFallen())
        {
            Vector3 currentCheckPoint = GameManager.Instance.playManager.GetCurrentCheckpoint();
            MoveTo(currentCheckPoint);
        }
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
        Vector3 desiredMoveDir = (camForward * inputDir.z + camRight * inputDir.x).normalized;
        Vector3 moveDir = desiredMoveDir;

        Vector3 currentVelocity = _rb.velocity;
        Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
        float horizontalSpeed = horizontalVelocity.magnitude;

        bool grounded = IsGrounded();
        bool isSliding = ApplySlopeSlide();

        RaycastHit hit;
        bool onSlope = false;
        Vector3 slopeNormal = Vector3.up;
        float slopeAngle = 0f;

        if (grounded && Physics.Raycast(transform.position + Vector3.up * 0.1f, - transform.up, out hit, 1.2f))
        {
            slopeNormal = hit.normal;
            slopeAngle = Vector3.Angle(slopeNormal, Vector3.up);
            if (slopeAngle > 5f)
            {
                onSlope = true;
                Vector3 slopeMoveDir = Vector3.ProjectOnPlane(desiredMoveDir, slopeNormal).normalized;

                // 방향 보간 (급격한 전환 방지)
                float angleBetween = Vector3.Angle(desiredMoveDir, slopeMoveDir);
                float t = Mathf.Clamp01(angleBetween / 45f);
                moveDir = Vector3.Slerp(desiredMoveDir, slopeMoveDir, t);
            }
        }

        float slopeAccelFactor = onSlope ? Mathf.Cos(slopeAngle * Mathf.Deg2Rad) : 1f;
        float maxSpeed = moveSpeed * slopeAccelFactor;
        float effectiveAccel = acceleration * slopeAccelFactor;
        float moveDirSpeed = Vector3.Dot(currentVelocity, moveDir);

        if (_inputDirection.magnitude > 0 && grounded)
        {
            float alignment = Vector3.Dot(horizontalVelocity.normalized, moveDir);

            // 일반적인 경우 빠르게 가속
            if (moveDirSpeed < maxSpeed - 0.1f)
            {
                Vector3 desiredVelocity = moveDir * maxSpeed;
                Vector3 velocityDelta = desiredVelocity - currentVelocity;
                Vector3 force = Vector3.ClampMagnitude(velocityDelta * effectiveAccel, acceleration);
                _rb.AddForce(force, ForceMode.Acceleration);
            }
            // 경사면 이동 등으로 인해 최대 속도를 초과한 상태에서 input이 있다면, 최대 속도에 천천히 수렴
            else if (alignment > 0.9f && moveDirSpeed > maxSpeed + 0.5f && slopeAngle < 3f)
            {
                float excess = moveDirSpeed - maxSpeed;
                float dampingStrength = 0.003f;
                Vector3 decel = -moveDir * (excess * dampingStrength);
                _rb.AddForce(decel, ForceMode.Acceleration);
            }
            // 최대 속도를 초과한 상태에서 반대방향 키를 누르면 빠르게 멈춤
            else if (alignment < -0.1f)
            {
                Vector3 steer = (moveDir - horizontalVelocity.normalized) * effectiveAccel * 0.5f;
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
                // 평지에서 고속 + 무입력 → 빠르게 감속
                if (horizontalSpeed > moveSpeed)
                {
                    Vector3 decel = -horizontalVelocity.normalized * deceleration * 0.5f;
                    _rb.AddForce(decel, ForceMode.Acceleration);
                }
                // 중속 → 일반 감속
                else if (horizontalSpeed > 1f)
                {
                    Vector3 decel = -horizontalVelocity.normalized * deceleration;
                    _rb.AddForce(decel, ForceMode.Acceleration);
                }
                // 저속 → 확실히 멈춤
                else
                {
                    Vector3 velocity = _rb.velocity;
                    velocity.x = 0f;
                    velocity.z = 0f;
                    _rb.velocity = velocity;
                }
            }
        }

        // 공중 -> 미세 조정만 가능
        if (_inputDirection.magnitude > 0 && !grounded)
        {
            Vector3 desiredVelocity = moveDir * moveSpeed;
            Vector3 velocityDelta = desiredVelocity - horizontalVelocity;
            Vector3 force = Vector3.ClampMagnitude(velocityDelta * 0.05f, 0.3f);
            _rb.AddForce(force, ForceMode.Acceleration);
        }

        // 경사면에서 뜨지 않게 보정
        if (onSlope && grounded)
        {
            if (_rb.velocity.y > 0f)
            {
                _rb.AddForce(-slopeNormal * 50f, ForceMode.Acceleration);
            }

            Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hitSlope, 1.5f))
            {
                float distanceToGround = hitSlope.distance;
                if (distanceToGround > 0.01f && distanceToGround < 0.2f && _rb.velocity.y <= 0f)
                {
                    _rb.MovePosition(transform.position - slopeNormal * (distanceToGround - 0.01f));
                }
            }
        }

        if (IsGrounded())
        {
            rotateByGroundAndCamera();
        }

        // Debug.DrawRay(transform.position, Vector3.down * 0.9f, Color.red, 1f);

        // Debug.Log($"onslope: {onSlope}, grounded: {grounded}");
        // Debug.Log($"|v|: {_rb.velocity.magnitude:F2}, moveDirSpeed: {moveDirSpeed:F2}, slopeAngle: {slopeAngle:F1}");
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
        float fallUnder = GameManager.Instance.playManager.fallThresholdHeight;
        return _rb.transform.position.y <= fallUnder || IsFallingTooLong();
    }

    private bool IsFallingTooLong()
    {
        float fallSecond = GameManager.Instance.playManager.fallThresholdSecond;
        return _fallTimer >= fallSecond;
    }

    public void MoveTo(Vector3 position)
    {
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        transform.position = position;
    }

    private void rotateByGroundAndCamera()
    {
        // 1. 현재 플레이어가 서 있는 지면 노멀 계산
        Vector3 surfaceNormal = Vector3.up;  // 기본값
        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit2, 2f))
        {
            surfaceNormal = hit2.normal;
        }

        // 2. 카메라 방향 기반으로 y축 회전만 추출
        Vector3 cameraForward = cameraPivot.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();
        Quaternion yRotation = Quaternion.LookRotation(cameraForward, Vector3.up);

        // 3. 최종 회전 계산: 지면 노멀에 align + y축은 카메라 기준
        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, surfaceNormal) * yRotation;

        // 4. 부드럽게 회전 (옵션)
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
    }

    public void Jump(float magnitude = 1.0f)
    {
        _rb.AddForce(Vector3.up * magnitude, ForceMode.Impulse);
    }
}
