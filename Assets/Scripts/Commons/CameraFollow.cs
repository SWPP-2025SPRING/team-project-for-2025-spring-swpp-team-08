using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform cameraPivot;
    public Vector3 offset = new Vector3(0f,5f, -20f);
    public float followSpeed = 10f;

    private void LateUpdate()
    {
        Vector3 targetPos = cameraPivot.position + cameraPivot.rotation * offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
        transform.LookAt(cameraPivot);
    }
}
