using UnityEngine;

public class BoostPad1 : MonoBehaviour
{
    public float boostForce = 10f;
    public AudioClip boostSound;
    public float soundDelay = 0.5f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            NewPlayerControl playerControl = collision.gameObject.GetComponent<NewPlayerControl>();

            if (rb != null && playerControl != null)
            {
                Vector3 moveDirection = rb.velocity.normalized;

                // Fallback: if velocity is too small, use forward direction
                if (moveDirection.magnitude < 0.1f)
                {
                    moveDirection = collision.transform.forward;
                }

                rb.velocity = Vector3.zero;
                rb.AddForce(moveDirection * boostForce, ForceMode.VelocityChange);
                playerControl.Jump(0);

                Debug.Log("Boosting in direction: " + moveDirection);
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
