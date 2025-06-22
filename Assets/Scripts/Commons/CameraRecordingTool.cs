using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraKeyframe
{
    public Vector3 position;
    public Quaternion rotation;
    public float fov = 60f;
    public float timestamp;
    
    public CameraKeyframe()
    {
        // Default constructor
    }
    
    public CameraKeyframe(Transform cameraTransform, Camera cam, float time)
    {
        position = cameraTransform.position;
        rotation = cameraTransform.rotation;
        fov = cam.fieldOfView;
        timestamp = time;
    }
}

[System.Serializable]
public class CameraShot
{
    public string shotName;
    public List<CameraKeyframe> keyframes = new List<CameraKeyframe>();
    public float duration;
    public Color gizmoColor = Color.white;
    
    public void CalculateDuration()
    {
        if (keyframes.Count >= 2)
        {
            duration = keyframes[keyframes.Count - 1].timestamp - keyframes[0].timestamp;
        }
    }
}

public class CameraRecordingTool : MonoBehaviour
{
    [Header("📹 Recording Controls")]
    [Tooltip("Press R to start/stop recording")]
    public bool isRecording = false;
    
    [Header("📝 Current Shot")]
    public string currentShotName = "Shot_01";
    public int shotCounter = 1;
    
    [Header("⚙️ Recording Settings")]
    
    public float recordingInterval = 0.1f;
    [Tooltip("Higher = smoother movement during playback")]
    
    public float playbackSpeed = 1f;
    
    [Header("🎬 All Shots")]
    public List<CameraShot> allShots = new List<CameraShot>();
    
    [Header("💾 Save/Load")]
    public CameraSequenceAsset sequenceToSave;
    [Tooltip("Create: Right-click in Project → Create → Camera → Camera Sequence")]
    public string newSequenceName = "Stage_01_Intro";
    
    [Header("📊 Debug Info")]
    public bool showDebugInfo = true;
    public bool showGizmos = true;
    
    // Private variables
    private Camera cam;
    private List<CameraKeyframe> currentRecording = new List<CameraKeyframe>();
    private float lastRecordTime;
    private float recordingStartTime;
    private bool isPlaying = false;
    
    // Free look camera variables
    [Header("🎮 Free Look Camera")]
    public bool freeLookEnabled = false;
    
    public float moveSpeed = 5f;
    
    public float fastMoveSpeed = 15f;
    
    public float mouseSensitivity = 2f;
    
    private float rotationX = 0f;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;
    
    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("CameraRecordingTool must be attached to a Camera!");
            enabled = false;
            return;
        }
        
        originalCameraPosition = transform.position;
        originalCameraRotation = transform.rotation;
        
        Debug.Log("📹 Camera Recording Tool Ready!");
        Debug.Log("Controls:");
        Debug.Log("F - Toggle free look mode");
        Debug.Log("R - Start/Stop recording");
        Debug.Log("S - Save current recording as shot");
        Debug.Log("P - Play all shots");
        Debug.Log("O - Play current shot only");
        Debug.Log("C - Clear current recording");
        Debug.Log("N - Next shot number");
        Debug.Log("X - Delete last shot");
        Debug.Log("Z - Save all shots to ScriptableObject asset");
        Debug.Log("L - Load shots from ScriptableObject asset");
    }
    
    void Update()
    {
        HandleInput();
        
        if (isRecording && Time.time - lastRecordTime >= recordingInterval)
        {
            RecordKeyframe();
        }
        
        if (freeLookEnabled && !isPlaying)
        {
            HandleFreeLookMovement();
            HandleMouseLook();
        }
    }
    
    void HandleInput()
    {
        // Toggle free look mode
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleFreeLook();
        }
        
        // Don't allow recording controls during playback
        if (isPlaying) return;
        
        // Recording controls
        if (Input.GetKeyDown(KeyCode.R))
        {
            ToggleRecording();
        }
        
        if (Input.GetKeyDown(KeyCode.S) && !isRecording)
        {
            SaveCurrentShot();
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(PlayAllShots());
        }
        
        if (Input.GetKeyDown(KeyCode.O))
        {
            StartCoroutine(PlayCurrentRecording());
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearCurrentRecording();
        }
        
        if (Input.GetKeyDown(KeyCode.N))
        {
            NextShotNumber();
        }
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            DeleteLastShot();
        }
        
        // Save to ScriptableObject
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SaveToScriptableObject();
        }
        
        // Load from ScriptableObject
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadFromScriptableObject();
        }
    }
    
    void ToggleFreeLook()
    {
        if (isRecording)
        {
            Debug.LogWarning("Can't toggle free look while recording!");
            return;
        }
        
        freeLookEnabled = !freeLookEnabled;
        
        if (freeLookEnabled)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Debug.Log("🎮 Free Look Mode: ON (WASD + Mouse + Space/Ctrl)");
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Log("🎮 Free Look Mode: OFF");
        }
    }
    
    void HandleFreeLookMovement()
    {
        float speed = Input.GetKey(KeyCode.LeftShift) ? fastMoveSpeed : moveSpeed;
        
        Vector3 move = Vector3.zero;
        
        // WASD movement
        if (Input.GetKey(KeyCode.W)) move += transform.forward;
        if (Input.GetKey(KeyCode.S)) move -= transform.forward;
        if (Input.GetKey(KeyCode.A)) move -= transform.right;
        if (Input.GetKey(KeyCode.D)) move += transform.right;
        
        // Up/Down movement
        if (Input.GetKey(KeyCode.Space)) move += Vector3.up;
        if (Input.GetKey(KeyCode.LeftControl)) move -= Vector3.up;
        
        transform.position += move.normalized * speed * Time.deltaTime;
    }
    
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        
        transform.localRotation = Quaternion.Euler(rotationX, transform.localEulerAngles.y + mouseX, 0);
    }
    
    void ToggleRecording()
    {
        if (isPlaying)
        {
            Debug.LogWarning("Can't record during playback!");
            return;
        }
        
        isRecording = !isRecording;
        
        if (isRecording)
        {
            StartRecording();
        }
        else
        {
            StopRecording();
        }
    }
    
    void StartRecording()
    {
        currentRecording.Clear();
        recordingStartTime = Time.time;
        lastRecordTime = Time.time;
        
        // Record first keyframe immediately
        RecordKeyframe();
        
        Debug.Log($"🔴 RECORDING: {currentShotName}");
    }
    
    void StopRecording()
    {
        if (currentRecording.Count > 0)
        {
            Debug.Log($"⏹️ STOPPED: {currentShotName} ({currentRecording.Count} keyframes, {GetCurrentRecordingDuration():F1}s)");
        }
        else
        {
            Debug.Log("⏹️ STOPPED: No keyframes recorded");
        }
    }
    
    void RecordKeyframe()
    {
        var keyframe = new CameraKeyframe(transform, cam, Time.time - recordingStartTime);
        currentRecording.Add(keyframe);
        lastRecordTime = Time.time;
    }
    
    void SaveCurrentShot()
    {
        if (currentRecording.Count < 2)
        {
            Debug.LogWarning("Need at least 2 keyframes to save a shot!");
            return;
        }
        
        var newShot = new CameraShot
        {
            shotName = currentShotName,
            keyframes = new List<CameraKeyframe>(currentRecording),
            gizmoColor = Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.8f, 1f)
        };
        
        newShot.CalculateDuration();
        
        // Check if shot already exists
        var existingShot = allShots.Find(s => s.shotName == currentShotName);
        if (existingShot != null)
        {
            existingShot.keyframes = newShot.keyframes;
            existingShot.duration = newShot.duration;
            Debug.Log($"💾 UPDATED: {currentShotName} ({newShot.duration:F1}s)");
        }
        else
        {
            allShots.Add(newShot);
            Debug.Log($"💾 SAVED: {currentShotName} ({newShot.duration:F1}s)");
        }
        
        ClearCurrentRecording();
    }
    
    void ClearCurrentRecording()
    {
        currentRecording.Clear();
        Debug.Log($"🗑️ CLEARED: Current recording");
    }
    
    void NextShotNumber()
    {
        if (isRecording)
        {
            Debug.LogWarning("Stop recording before changing shot number!");
            return;
        }
        
        shotCounter++;
        currentShotName = $"Shot_{shotCounter:00}";
        Debug.Log($"📝 Next shot: {currentShotName}");
    }
    
    void DeleteLastShot()
    {
        if (allShots.Count > 0)
        {
            var lastShot = allShots[allShots.Count - 1];
            allShots.RemoveAt(allShots.Count - 1);
            Debug.Log($"🗑️ DELETED: {lastShot.shotName}");
        }
        else
        {
            Debug.Log("No shots to delete!");
        }
    }
    
    void SaveToScriptableObject()
    {
        if (allShots.Count == 0)
        {
            Debug.LogWarning("No shots to save!");
            return;
        }
        
        if (sequenceToSave == null)
        {
            Debug.LogError("No CameraSequenceAsset assigned! Create one: Right-click → Create → Camera → Camera Sequence");
            return;
        }
        
        // Convert our shots to the ScriptableObject format
        sequenceToSave.shots.Clear();
        sequenceToSave.sequenceName = newSequenceName;
        
        foreach (var shot in allShots)
        {
            var savedShot = new CameraSequenceAsset.SavedShot
            {
                shotName = shot.shotName,
                duration = shot.duration,
                keyframes = new List<CameraSequenceAsset.SavedKeyframe>()
            };
            
            foreach (var keyframe in shot.keyframes)
            {
                savedShot.keyframes.Add(new CameraSequenceAsset.SavedKeyframe
                {
                    position = keyframe.position,
                    rotation = keyframe.rotation,
                    fov = keyframe.fov,
                    timestamp = keyframe.timestamp
                });
            }
            
            sequenceToSave.shots.Add(savedShot);
        }
        
        // Mark dirty so Unity saves it
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(sequenceToSave);
        UnityEditor.AssetDatabase.SaveAssets();
        #endif
        
        Debug.Log($"💾 SAVED TO ASSET: {sequenceToSave.name} ({allShots.Count} shots)");
    }
    
    void LoadFromScriptableObject()
    {
        if (sequenceToSave == null)
        {
            Debug.LogError("No CameraSequenceAsset assigned!");
            return;
        }
        
        if (sequenceToSave.shots.Count == 0)
        {
            Debug.LogWarning("ScriptableObject has no shots to load!");
            return;
        }
        
        // Convert from ScriptableObject format to our format
        allShots.Clear();
        
        foreach (var savedShot in sequenceToSave.shots)
        {
            var shot = new CameraShot
            {
                shotName = savedShot.shotName,
                duration = savedShot.duration,
                gizmoColor = Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.8f, 1f),
                keyframes = new List<CameraKeyframe>()
            };
            
            foreach (var savedKeyframe in savedShot.keyframes)
            {
                var newKeyframe = new CameraKeyframe
                {
                    position = savedKeyframe.position,
                    rotation = savedKeyframe.rotation,
                    fov = savedKeyframe.fov,
                    timestamp = savedKeyframe.timestamp
                };
                shot.keyframes.Add(newKeyframe);
            }
            
            allShots.Add(shot);
        }
        
        Debug.Log($"📂 LOADED FROM ASSET: {sequenceToSave.name} ({allShots.Count} shots)");
    }
    
    public IEnumerator PlayAllShots()
    {
        if (allShots.Count == 0)
        {
            Debug.LogWarning("No shots to play!");
            yield break;
        }
        
        isPlaying = true;
        Debug.Log($"▶️ PLAYING ALL SHOTS ({allShots.Count} shots)");
        
        for (int i = 0; i < allShots.Count; i++)
        {
            Debug.Log($"▶️ Playing: {allShots[i].shotName}");
            yield return StartCoroutine(PlayShot(allShots[i]));
            
            // Brief pause between shots (except after last shot)
            if (i < allShots.Count - 1)
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
        
        isPlaying = false;
        Debug.Log("✅ FINISHED: All shots played");
    }
    
    public IEnumerator PlayCurrentRecording()
    {
        if (currentRecording.Count < 2)
        {
            Debug.LogWarning("Need at least 2 keyframes to play!");
            yield break;
        }
        
        isPlaying = true;
        Debug.Log($"▶️ PLAYING: Current recording ({GetCurrentRecordingDuration():F1}s)");
        
        yield return StartCoroutine(PlayKeyframes(currentRecording));
        
        isPlaying = false;
        Debug.Log("✅ FINISHED: Current recording played");
    }
    
    IEnumerator PlayShot(CameraShot shot)
    {
        if (shot.keyframes.Count < 2) yield break;
        yield return StartCoroutine(PlayKeyframes(shot.keyframes));
    }
    
    IEnumerator PlayKeyframes(List<CameraKeyframe> keyframes)
    {
        if (keyframes.Count < 2) yield break;
        
        float duration = keyframes[keyframes.Count - 1].timestamp - keyframes[0].timestamp;
        float startTime = Time.time;
        
        while (Time.time - startTime < duration / playbackSpeed)
        {
            float progress = (Time.time - startTime) * playbackSpeed / duration;
            InterpolateCamera(keyframes, progress);
            yield return null;
        }
        
        // Ensure we end at the exact final position
        var lastFrame = keyframes[keyframes.Count - 1];
        transform.position = lastFrame.position;
        transform.rotation = lastFrame.rotation;
        cam.fieldOfView = lastFrame.fov;
    }
    
    void InterpolateCamera(List<CameraKeyframe> keyframes, float t)
    {
        if (keyframes.Count < 2) return;
        
        float targetTime = Mathf.Lerp(keyframes[0].timestamp, keyframes[keyframes.Count - 1].timestamp, t);
        
        // Find the two keyframes to interpolate between
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
        
        // Smooth interpolation
        localT = Mathf.SmoothStep(0f, 1f, localT);
        
        transform.position = Vector3.Lerp(current.position, next.position, localT);
        transform.rotation = Quaternion.Lerp(current.rotation, next.rotation, localT);
        cam.fieldOfView = Mathf.Lerp(current.fov, next.fov, localT);
    }
    
    float GetCurrentRecordingDuration()
    {
        if (currentRecording.Count < 2) return 0f;
        return currentRecording[currentRecording.Count - 1].timestamp - currentRecording[0].timestamp;
    }
    
    void OnGUI()
    {
        if (!showDebugInfo) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 300, 400));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("📹 Camera Recording Tool", GUI.skin.box);
        
        // Status
        GUILayout.Label($"Status: {(isRecording ? "🔴 RECORDING" : isPlaying ? "▶️ PLAYING" : "⏸️ READY")}");
        GUILayout.Label($"Free Look: {(freeLookEnabled ? "🎮 ON" : "OFF")}");
        GUILayout.Label($"Current Shot: {currentShotName}");
        
        GUILayout.Space(5);
        
        // Current recording info
        if (currentRecording.Count > 0)
        {
            GUILayout.Label($"Current Recording: {currentRecording.Count} keyframes");
            if (currentRecording.Count >= 2)
            {
                GUILayout.Label($"Duration: {GetCurrentRecordingDuration():F1}s");
            }
        }
        
        GUILayout.Space(5);
        
        // All shots info
        GUILayout.Label($"Total Shots: {allShots.Count}");
        foreach (var shot in allShots)
        {
            GUILayout.Label($"• {shot.shotName}: {shot.duration:F1}s ({shot.keyframes.Count} frames)");
        }
        
        GUILayout.Space(10);
        
        // Controls reminder
        GUILayout.Label("Controls:", GUI.skin.box);
        GUILayout.Label("F - Free Look | R - Record");
        GUILayout.Label("S - Save | P - Play All | O - Play Current");
        GUILayout.Label("C - Clear | N - Next Shot | X - Delete Last");
        GUILayout.Label("Z - Save to Asset | L - Load from Asset");
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
    
    void OnDrawGizmos()
    {
        if (!showGizmos) return;
        
        // Draw all saved shots
        foreach (var shot in allShots)
        {
            DrawShotGizmos(shot);
        }
        
        // Draw current recording in bright color
        if (currentRecording.Count > 1)
        {
            Gizmos.color = isRecording ? Color.red : Color.yellow;
            for (int i = 0; i < currentRecording.Count - 1; i++)
            {
                Gizmos.DrawLine(currentRecording[i].position, currentRecording[i + 1].position);
                Gizmos.DrawWireSphere(currentRecording[i].position, 0.2f);
            }
            // Last point
            Gizmos.DrawWireSphere(currentRecording[currentRecording.Count - 1].position, 0.2f);
        }
    }
    
    void DrawShotGizmos(CameraShot shot)
    {
        if (shot.keyframes.Count < 2) return;
        
        Gizmos.color = shot.gizmoColor;
        
        for (int i = 0; i < shot.keyframes.Count - 1; i++)
        {
            Gizmos.DrawLine(shot.keyframes[i].position, shot.keyframes[i + 1].position);
            Gizmos.DrawWireSphere(shot.keyframes[i].position, 0.1f);
        }
        
        // Draw camera direction at start and end
        var firstFrame = shot.keyframes[0];
        var lastFrame = shot.keyframes[shot.keyframes.Count - 1];
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(firstFrame.position, 0.3f); // Start
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(lastFrame.position, 0.3f); // End
    }
}