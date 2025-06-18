using System;

using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float velocityMultiplier = 2.0f;
    public float maxJumpForce = 500f;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            NewPlayerControl newPlayerControl = other.gameObject.GetComponent<NewPlayerControl>();
            float computedJump = Math.Min(maxJumpForce, Math.Abs(other.impulse.y) * velocityMultiplier);
            // Debug.Log($"impulse: {other.impulse.y} computed: {computedJump}");
            newPlayerControl.Jump(computedJump);
        }
    }
}
