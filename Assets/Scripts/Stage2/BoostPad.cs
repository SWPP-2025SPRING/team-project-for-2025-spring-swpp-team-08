using UnityEngine;

// No changes needed to the class declaration
public class BoostPad : MonoBehaviour
{
    public float boostForce = 10f;
    public AudioClip boostSound;
    public float soundDelay = 0.5f;

    // CHANGED: We are now using OnCollisionEnter instead of OnTriggerEnter.
    // This function is called when another collider physically bumps into this one.
    private void OnCollisionEnter(Collision collision)
    {
        // The 'collision' variable contains information about the contact,
        // including which object we hit. We get it from 'collision.gameObject'.
        if (collision.gameObject.CompareTag("Player"))
        {
            // The rest of the logic is the same, we just get the components
            // from 'collision.gameObject' instead of 'other'.
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            NewPlayerControl playerControl = collision.gameObject.GetComponent<NewPlayerControl>();

            if (rb != null && playerControl != null)
            {
                // We keep the check to only boost from the ground.
                // You could remove this if you want aerial boosts to be possible.
                if (Mathf.Abs(rb.velocity.y) < 0.2f && rb.velocity.magnitude > 0.1f)
                {
                    // 1. Apply the directional boost based on player's incoming velocity.
                    Vector3 moveDirection = rb.velocity.normalized;
                    rb.AddForce(moveDirection * boostForce, ForceMode.VelocityChange);

                    // 2. Consume the player's jump ability to prevent the double jump.
                    playerControl.Jump(0);

                    Debug.Log("Player collided with BoostPad mesh and was boosted.");
                }
            }

            if (boostSound != null)
            {
                StartCoroutine(PlayDelayedSound());
            }
        }
    }

    private System.Collections.IEnumerator PlayDelayedSound()
    {
        yield return new WaitForSeconds(soundDelay);
        AudioSource.PlayClipAtPoint(boostSound, transform.position);
    }
}