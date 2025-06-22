using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Camera Sequence", menuName = "Camera/Camera Sequence")]
public class CameraSequenceAsset : ScriptableObject
{
    [System.Serializable]
    public class SavedKeyframe
    {
        public Vector3 position;
        public Quaternion rotation;
        public float fov = 60f;
        public float timestamp;
    }
    
    [System.Serializable]
    public class SavedShot
    {
        public string shotName;
        public List<SavedKeyframe> keyframes = new List<SavedKeyframe>();
        public float duration;
    }
    
    [Header("Sequence Info")]
    public string sequenceName;
    
    [Header("All Shots")]
    public List<SavedShot> shots = new List<SavedShot>();
    
    [Header("Playback Settings")]
    public float playbackSpeed = 1f;
}