using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private static int INGAME_CHECKPOINT_NUM = 0;
    public int currentCheckpointNum;
    public Material triggeredMaterial;
    private bool _isTriggered = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_isTriggered)
        {
            Vector3 checkpointPosition = transform.position;
            GameManager.Instance.playManager.UpdateCheckpoint(checkpointPosition);
            _isTriggered=true;
            INGAME_CHECKPOINT_NUM = currentCheckpointNum; // start from 1
            Debug.Log("Checkpoint saved at: " + checkpointPosition);
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null && triggeredMaterial != null)
            {
                renderer.material = triggeredMaterial;
            }
        }
    }
}
