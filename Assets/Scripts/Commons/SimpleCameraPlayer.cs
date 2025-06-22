using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraPlayer : MonoBehaviour
{
    public CameraSequenceAsset cameraSequence;
    
    private Camera cam;
    
    void Start()
    {
        cam = GetComponent<Camera>();
    }
    
    public IEnumerator PlaySequence()
    {
        if (cameraSequence == null || cameraSequence.shots.Count == 0)
        {
            Debug.LogWarning("No camera sequence to play!");
            yield break;
        }
        
        Debug.Log($"🎬 Playing camera sequence: {cameraSequence.sequenceName}");
        
        foreach (var shot in cameraSequence.shots)
        {
            Debug.Log($"▶️ Playing shot: {shot.shotName}");
            yield return StartCoroutine(PlayShot(shot));
            yield return new WaitForSeconds(0.5f); // Brief pause between shots
        }
        
        Debug.Log("✅ Camera sequence finished");
    }
    
    IEnumerator PlayShot(CameraSequenceAsset.SavedShot shot)
    {
        if (shot.keyframes.Count < 2) yield break;
        
        float duration = shot.duration / cameraSequence.playbackSpeed;
        float startTime = Time.time;
        
        while (Time.time - startTime < duration)
        {
            float progress = (Time.time - startTime) / duration;
            InterpolateCamera(shot.keyframes, progress);
            yield return null;
        }
        
        // End at exact final position
        var lastFrame = shot.keyframes[shot.keyframes.Count - 1];
        transform.position = lastFrame.position;
        transform.rotation = lastFrame.rotation;
        cam.fieldOfView = lastFrame.fov;
    }
    
    void InterpolateCamera(List<CameraSequenceAsset.SavedKeyframe> keyframes, float t)
    {
        if (keyframes.Count < 2) return;
        
        float targetTime = Mathf.Lerp(keyframes[0].timestamp, keyframes[keyframes.Count - 1].timestamp, t);
        
        // Find keyframes to interpolate between
        int index = 0;
        for (int i = 0; i < keyframes.Count - 1; i++)
        {
            if (keyframes[i + 1].timestamp >= targetTime)
            {
                index = i;
                break;
            }
        }
        
        if (index >= keyframes.Count - 1) return;
        
        var current = keyframes[index];
        var next = keyframes[index + 1];
        
        float localT = (targetTime - current.timestamp) / (next.timestamp - current.timestamp);
        localT = Mathf.SmoothStep(0f, 1f, localT); // Smooth interpolation
        
        transform.position = Vector3.Lerp(current.position, next.position, localT);
        transform.rotation = Quaternion.Lerp(current.rotation, next.rotation, localT);
        cam.fieldOfView = Mathf.Lerp(current.fov, next.fov, localT);
    }
}