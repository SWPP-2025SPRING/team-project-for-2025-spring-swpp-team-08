using UnityEngine;

public class SpiralMagnet : MonoBehaviour
{
    public float magnetForce = 20f;
    public float maxMagnetDistance = 3f;
    public LayerMask playerLayer;

    private void FixedUpdate()
    {
        Collider[] players = Physics.OverlapSphere(transform.position, 50f, playerLayer); // adjust radius as needed
        foreach (var player in players)
        {
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb == null) continue;

            Vector3 closestPoint = GetComponent<Collider>().ClosestPoint(player.transform.position);
            Vector3 pullDirection = (closestPoint - player.transform.position);

            if (pullDirection.magnitude < maxMagnetDistance)
            {
                rb.AddForce(pullDirection.normalized * magnetForce, ForceMode.Acceleration);
            }
        }
    }
}
