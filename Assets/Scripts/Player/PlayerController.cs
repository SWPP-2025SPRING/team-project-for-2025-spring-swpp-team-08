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
        // 카메라 기준 방향 계산
        Vector3 camForward = cameraPivot.forward;
        Vector3 camRight = cameraPivot.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        // 이동 방향 계산
        Vector3 moveDir = (camForward * _inputDirection.z + camRight * _inputDirection.x).normalized;
        Vector3 currentVelocity = _rb.velocity;
        Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);
        Vector3 targetVelocity = moveDir * moveSpeed;

        // 가속 또는 감속 적용
        float accelerationRate = (_inputDirection.magnitude > 0) ? acceleration : deceleration;
        Vector3 newVelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity, accelerationRate * Time.fixedDeltaTime);

        // 최종 속도 설정
        _rb.velocity = new Vector3(newVelocity.x, currentVelocity.y, newVelocity.z);
    }
}
