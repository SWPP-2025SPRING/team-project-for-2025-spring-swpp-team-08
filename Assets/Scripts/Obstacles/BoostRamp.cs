using UnityEngine;

public class BoostRamp : MonoBehaviour
{
    [Tooltip("Force applied to the player on entry.")]
    public float boostForce = 30f;
    [Tooltip("Direction of the boost in local space.")]
    public Vector3 localBoostDirection = new Vector3(0, 1, 1);
    [Tooltip("Cooldown time to prevent repeated boosting.")]
    public float cooldownTime = 2f;
    private readonly string _playerTag = "Player";
    

    private float lastBoostTime = -999f;

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time < lastBoostTime + cooldownTime) return;

        if (other.CompareTag(_playerTag))
        {
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null)
            {
                // Apply force in ramp's local direction
                Vector3 worldBoostDir = transform.TransformDirection(localBoostDirection.normalized);
                rb.AddForce(worldBoostDir * boostForce, ForceMode.Impulse);

                lastBoostTime = Time.time;
            }
        }
    }
}
