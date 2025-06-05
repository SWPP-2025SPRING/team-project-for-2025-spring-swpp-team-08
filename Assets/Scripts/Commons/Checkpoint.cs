using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private static int INGAME_CHECKPOINT_NUM = 0;
    public int currentCheckpointNum;
    private bool _isTriggered = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_istriggered)
        {
            Vector3 checkpointPosition = transform.position;
            GameManager.Instance.playManager.UpdateCheckpoint(checkpointPosition);
            _istriggered=true;
            Debug.Log("Checkpoint saved at: " + checkpointPosition);
        }
    }
}
