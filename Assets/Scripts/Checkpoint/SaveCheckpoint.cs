using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SaveCheckpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 checkpointPosition = transform.position;
            GameManager.Instance.playManager.UpdateCheckpoint(checkpointPosition);
            Debug.Log("Checkpoint saved at: " + checkpointPosition);
        }
    }
}
