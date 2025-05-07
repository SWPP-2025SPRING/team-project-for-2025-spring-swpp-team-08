using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 30f;
    public float acceleration = 100f;
    public float deceleration = 100f;
    public Transform cameraPivot;

    private Rigidbody _rb;
    private Vector3 _inputDirection;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        _inputDirection = new Vector3(h, 0, v).normalized;
    }

    void FixedUpdate()
    {
        Vector3 camForward = cameraPivot.forward;
        Vector3 camRight = cameraPivot.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * _inputDirection.z + camRight * _inputDirection.x).normalized;
        Vector3 currentVelocity = _rb.velocity;
        Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
        float horizontalSpeed = horizontalVelocity.magnitude;

        bool isSliding = ApplySlopeSlide();
        bool grounded = IsGrounded();

        if (_inputDirection.magnitude > 0)
        {
            Vector3 desiredVelocity = moveDir * moveSpeed;
            Vector3 velocityDelta = desiredVelocity - horizontalVelocity;

            if (grounded)
            {
                // 지상: 가속 정상 처리
                if (horizontalSpeed > moveSpeed)
                {
                    float damping = 1f - (0.1f * Time.fixedDeltaTime);
                    Vector3 dampedVelocity = horizontalVelocity * damping;
                    _rb.velocity = new Vector3(dampedVelocity.x, currentVelocity.y, dampedVelocity.z);
                }
                else
                {
                    Vector3 force = Vector3.ClampMagnitude(velocityDelta * acceleration, acceleration);
                    _rb.AddForce(force, ForceMode.Acceleration);
                }
            }
            else
            {
                // 공중: 최소한의 방향 보정만 허용 (아주 작은 힘만 가함)
                Vector3 force = Vector3.ClampMagnitude(velocityDelta * 0.2f, 1f); // 계수 0.2f, 최대 힘 1
                _rb.AddForce(force, ForceMode.Acceleration);
            }
        }
        else if (!isSliding && grounded)
        {
            // 감속은 지상에서만 적용
            Vector3 decel = -horizontalVelocity.normalized * deceleration;
            _rb.AddForce(decel, ForceMode.Acceleration);
        }

        Debug.Log($"velocity: {_rb.velocity.magnitude:F2}");
    }


    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, out _, 1.1f);
    }








    bool ApplySlopeSlide()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.1f))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            if (angle > 10f && _inputDirection.magnitude < 0.1f)
            {
                Vector3 slideDirection = Vector3.ProjectOnPlane(Vector3.down, hit.normal).normalized;
                _rb.AddForce(slideDirection * 30f, ForceMode.Acceleration); // slideForce는 원하는 대로 조절
                return true;
            }
        }
        return false;
    }

}
