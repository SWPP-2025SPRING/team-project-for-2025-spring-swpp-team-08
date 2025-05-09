using System;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float velocityMultiplier = 1.2f;
    public float maxJumpForce = 500f;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            float computedJump = Math.Min(maxJumpForce, Math.Abs(other.impulse.y) * velocityMultiplier);
            // Debug.Log($"impulse: {other.impulse.y} computed: {computedJump}");
            playerController.Jump(computedJump);
        }
    }
}
