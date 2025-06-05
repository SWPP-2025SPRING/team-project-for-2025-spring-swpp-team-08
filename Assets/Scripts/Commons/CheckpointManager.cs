using System.Collections.Generic;

using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    private List<Checkpoint> allCheckpoints = new List<Checkpoint>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void RegisterCheckpoint(Checkpoint checkpoint)
    {
        allCheckpoints.Add(checkpoint);
    }

    public void DisablePreviousCheckpoints(int currentNum)
    {
        foreach (Checkpoint cp in allCheckpoints)
        {
            if (cp.currentCheckpointNum < currentNum)
            {
                cp.DisableCheckpoint();
            }
        }
    }
}
