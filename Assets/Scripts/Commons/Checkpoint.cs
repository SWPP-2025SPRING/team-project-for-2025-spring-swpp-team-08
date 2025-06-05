using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int currentCheckpointNum;
    public Material triggeredMaterial;
    private bool _isTriggered = false;

    private void Start()
    {
        CheckpointManager.Instance.RegisterCheckpoint(this); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_isTriggered)
        {
            Vector3 checkpointPosition = transform.position;
            GameManager.Instance.playManager.UpdateCheckpoint(checkpointPosition);
            Debug.Log("Checkpoint saved at: " + checkpointPosition);
            usedCheckpoint();
            CheckpointManager.Instance.DisablePreviousCheckpoints(currentCheckpointNum);
        }
    }

    private void usedCheckpoint()
    {
        Renderer renderer = GetComponent<Renderer>();
        _isTriggered = true;
        if (renderer != null && triggeredMaterial != null)
        {
            renderer.material = triggeredMaterial;
        }
    }

    public void DisableCheckpoint()
    {
        usedCheckpoint();
    }
}
