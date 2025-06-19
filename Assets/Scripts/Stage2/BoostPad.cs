using UnityEngine;


public class BoostPad : MonoBehaviour
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

                if (Mathf.Abs(rb.velocity.y) < 0.2f && rb.velocity.magnitude > 0.1f)
                {
                    
                    Vector3 moveDirection = rb.velocity.normalized;
                    rb.AddForce(moveDirection * boostForce, ForceMode.VelocityChange);

            
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