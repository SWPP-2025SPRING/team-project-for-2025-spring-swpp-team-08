using UnityEngine;

// CHANGED: Removed [RequireComponent(typeof(Collider))] as it's less specific.
// The setup now assumes you've manually added the correct Mesh Collider.
public class SlowPad : MonoBehaviour
{
    public float slowMultiplier = 0.5f;
    public AudioClip slowSound;
    public float groundedThreshold = 0.1f; // vertical speed threshold for "on ground"

    private AudioSource audioSource;

    private void Start()
    {
        // This setup logic remains the same
        if (slowSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = slowSound;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }
    }

    // CHANGED: Replaced OnTriggerStay with OnCollisionStay.
    // This is called every physics frame as long as two colliders are touching.
    private void OnCollisionStay(Collision collision)
    {
        // We get the player object from the 'collision' data.
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // The core logic for checking if the player is grounded is the same.
                bool isGrounded = Mathf.Abs(rb.velocity.y) < groundedThreshold;

                if (isGrounded)
                {
                    // The slowing logic is also the same. We directly modify the velocity.
                    Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                    Vector3 slowedVelocity = horizontalVelocity * slowMultiplier;
                    rb.velocity = new Vector3(slowedVelocity.x, rb.velocity.y, slowedVelocity.z);

                    // Play sound if not already playing
                    if (audioSource != null && !audioSource.isPlaying)
                    {
                        audioSource.Play();
                    }
                }
                else
                {
                    // Stop the sound if player jumps while on the pad
                    if (audioSource != null && audioSource.isPlaying)
                    {
                        audioSource.Stop();
                    }
                }
            }
        }
    }

    // CHANGED: Replaced OnTriggerExit with OnCollisionExit.
    // This is called once when the colliders stop touching.
    private void OnCollisionExit(Collision collision)
    {
        // We check the tag to ensure it was the player that stopped touching the pad.
        if (collision.gameObject.CompareTag("Player") && audioSource != null && audioSource.isPlaying)
        {
            // Clean up and stop the sound.
            audioSource.Stop();
        }
    }
}