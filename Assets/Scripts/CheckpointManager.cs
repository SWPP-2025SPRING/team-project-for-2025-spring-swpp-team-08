using System.Collections.Generic;

using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    private List<Checkpoint> allCheckpoints = new List<Checkpoint>();

    public void RegisterCheckpoint(Checkpoint checkpoint)
    {
        allCheckpoints.Add(checkpoint);
    }

    public void DisablePreviousCheckpoints(int currentNum)
    {
        Debug.Log(currentNum);
        foreach (Checkpoint cp in allCheckpoints)
        {
            if (cp.currentCheckpointNum < currentNum)
            {
                cp.DisableCheckpoint();
            }
        }
    }
}
