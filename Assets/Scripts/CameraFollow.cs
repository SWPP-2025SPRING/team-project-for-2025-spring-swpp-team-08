using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform cameraPivot;
    public Vector3 pivotOffset;
    public Vector3 cameraOffset;
    public float followSpeed;

    private void LateUpdate()
    {
        var cameraTargetPos = cameraPivot.position + cameraPivot.rotation * cameraOffset;
        transform.position = Vector3.Lerp(transform.position, cameraTargetPos, followSpeed * Time.deltaTime);
        transform.LookAt(cameraPivot.position + pivotOffset);
    }
}
