using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SpiralBoost : MonoBehaviour
{
    [Header("Force Components")]
    public float xComponentForce = 5f;  // Negative X: inward tangential
    public float yComponentForce = 15f; // Positive Y: forward along spiral

    [Header("Audio")]
    public AudioClip boostSound;
    public float soundDelay = 0.5f;

    private LineRenderer lr;
    private Vector3 lastBoostVector;
    private bool showArrow = false;
    private float arrowDuration = 1.5f;
    private float arrowTimer = 0f;

    private void Start()
    {
        // Setup line renderer
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.yellow;
        lr.endColor = Color.red;
        lr.startWidth = 0.15f;
        lr.endWidth = 0.05f;
        lr.useWorldSpace = true;
        lr.enabled = false;
    }

    private void Update()
    {
        if (showArrow)
        {
            arrowTimer -= Time.deltaTime;
            if (arrowTimer <= 0f)
            {
                lr.enabled = false;
                showArrow = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        NewPlayerControl playerControl = collision.gameObject.GetComponent<NewPlayerControl>();

        if (rb != null && playerControl != null)
        {
            // Direct unnormalized force vector
            Vector3 boostForceVec = (-transform.right * xComponentForce) + (transform.up * yComponentForce);

            rb.AddForce(boostForceVec, ForceMode.VelocityChange);
            playerControl.Jump(0);

            lastBoostVector = boostForceVec;
            ShowArrow();

            Debug.Log($"🚀 Boosted player: {boostForceVec} | Magnitude: {boostForceVec.magnitude}");
        }

        if (boostSound != null)
        {
            StartCoroutine(PlayDelayedSound());
        }
    }

    private void ShowArrow()
    {
        if (lr == null) return;

        Vector3 origin = transform.position;
        Vector3 end = origin + lastBoostVector.normalized * 3f;

        lr.SetPosition(0, origin);
        lr.SetPosition(1, end);
        lr.enabled = true;

        arrowTimer = arrowDuration;
        showArrow = true;
    }

    private System.Collections.IEnumerator PlayDelayedSound()
    {
        yield return new WaitForSeconds(soundDelay);
        AudioSource.PlayClipAtPoint(boostSound, transform.position);
    }
}
