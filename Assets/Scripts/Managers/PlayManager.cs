using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayStates
{
    Ready,
    Playing,
    Finished
}

[System.Serializable]
public class StageIntroSequence
{
    [Header("Sequence Settings")]
    public string sequenceName;
    public CameraSequenceAsset cameraSequence;
    
    [Header("Shot Selection")]
    [Tooltip("Which shots to play (leave empty to play all)")]
    public List<string> shotsToPlay = new List<string>();
    
    [Header("Timing")]
    [Range(0.5f, 3f)]
    public float playbackSpeed = 1f;
    [Range(0f, 2f)]
    public float pauseBetweenShots = 0.5f;
    [Range(0f, 3f)]
    public float pauseAfterSequence = 1f;
    
    [Header("Audio")]
    [Tooltip("Optional BGM to play during this sequence")]
    public AudioClip sequenceBgm;
    [Range(0f, 1f)]
    public float bgmVolume = 0.7f;
    
    public bool IsValid()
    {
        return cameraSequence != null && cameraSequence.shots.Count > 0;
    }
}

public class PlayManager : MonoBehaviour
{
    private const float DelayBeforeStart = 3f;

    [Header("Debug Settings")]
    public bool skipCountdown;
    public bool skipCameraIntro; // NEW: Skip camera sequences for testing

    [Header("Stage Settings")]
    public int stageNo;

    public SceneType sceneType;
    public string nextSceneName;
    public string stageName;
    public float fallThresholdHeight = 0f;
    public float fallThresholdSecond = 5f;

    [Header("Camera Intro Sequences")] // NEW SECTION
    [Tooltip("Drag your 3 stage sequence assets here")]
    public List<StageIntroSequence> stageSequences = new List<StageIntroSequence>();
    
    [Header("Sequence Selection")]
    [Tooltip("Which sequence to use for this stage (0 = first, 1 = second, 2 = third, -1 = none)")]
    public int selectedSequenceIndex = 0;
    
    [Header("Audio Clips")]
    public AudioClip bgmForOpening;
    public AudioClip bgmForEnding;
    public List<AudioClip> bgmsPerStage = new();  // Assign in Inspector by index (Stage 1 = index 0)

    public AudioClip setCheckpoint;
    public AudioClip fallDown;
    public AudioClip introBgm;         // For "준비"
    public AudioClip countdownSfx;     // For 3, 2, 1
    public AudioClip goSfx;            // For "GO!"
    public AudioClip finishJingleLoop;

    [Header("References")]
    public UIManager uiManager;
    public Vector3 spawnPoint;

    public PlayStates State { get; private set; }

    private float _playTimeCurrent;
    private float _playTimeTotal;
    private int _retryCount;
    private bool _canMoveToNextStage;
    private Vector3 _checkpoint;
    private NewPlayerControl _playerControl;
    private GameObject _cameraObject;
    private SimpleCameraPlayer _cameraPlayer; // NEW

    private void Awake()
    {
        GameManager.Instance.playManager = this;

        _playerControl = GameObject.FindWithTag("Player").GetComponentInChildren<NewPlayerControl>();
        _cameraObject = Camera.main?.gameObject;
        
        // Setup camera player
        if (_cameraObject != null)
        {
            _cameraPlayer = _cameraObject.GetComponent<SimpleCameraPlayer>();
            if (_cameraPlayer == null)
            {
                _cameraPlayer = _cameraObject.AddComponent<SimpleCameraPlayer>();
            }
        }
    }

    private void Start()
    {
        State = PlayStates.Ready;

        switch (sceneType)
        {
            case SceneType.OPENING:
                PlaySceneBgm(); // Play opening BGM immediately
                uiManager.HideAllUIs();
                break;

            case SceneType.STAGE:
                _playTimeCurrent = 0f;
                _playTimeTotal = GameManager.Instance.totalPlayTime;
                _checkpoint = spawnPoint;
                _canMoveToNextStage = false;

                uiManager.UpdatePlayTime(_playTimeTotal);
                uiManager.UpdateCurrentPlayTime(_playTimeCurrent);
                uiManager.UpdateStage(stageName);

                // DON'T play stage BGM here - let the camera sequence handle it
                StartCoroutine(ReadyGameCoroutine());
                break;

            case SceneType.ENDING:
                PlaySceneBgm(); // Play ending BGM immediately
                uiManager.HideAllUIs();
                StartGame();
                break;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance.playManager == this)
        {
            GameManager.Instance.playManager = null;
        }
    }

    private void PlaySceneBgm()
    {
        switch (sceneType)
        {
            case SceneType.OPENING:
                if (bgmForOpening != null)
                    GameManager.Instance.PlayBgm(bgmForOpening);
                break;

            case SceneType.STAGE:
                if (stageNo > 0 && stageNo <= bgmsPerStage.Count)
                {
                    var clip = bgmsPerStage[stageNo - 1];
                    if (clip != null)
                        GameManager.Instance.PlayBgm(clip);
                }
                break;

            case SceneType.ENDING:
                if (bgmForEnding != null)
                    GameManager.Instance.PlayBgm(bgmForEnding);
                break;
        }
    }

    private void Update()
    {
        if (State == PlayStates.Playing && sceneType == SceneType.STAGE)
        {
            _playTimeCurrent += Time.deltaTime;
            _playTimeTotal += Time.deltaTime;
            uiManager.UpdatePlayTime(_playTimeTotal);
            uiManager.UpdateCurrentPlayTime(_playTimeCurrent);
        }

        if (State == PlayStates.Finished && _canMoveToNextStage)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                _canMoveToNextStage = false;
                LoadNextStage();
            }
        }
    }

    public void UpdateCheckpoint(Vector3 newCheckpoint)
    {
        uiManager.UpdateStateSubtitle("Checkpoint set...", 3);
        _checkpoint = newCheckpoint;
        GameManager.Instance.PlaySfx(setCheckpoint);
    }

    public void DisplayCheckpointReturn()
    {
        uiManager.UpdateStateSubtitle("Moved to last checkpoint", 3);
        GameManager.Instance.PlaySfx(fallDown);
    }

    public Vector3 GetCurrentCheckpoint()
    {
        return _checkpoint;
    }

    public IEnumerator ReadyGameCoroutine()
    {
        DisablePlayerControl();
        uiManager.ShowPlayUI();
        Debug.Log("Ready");

        #if UNITY_EDITOR
        if (skipCountdown)
        {
            uiManager.HideCountdownUI();
            StartGame();
            // Still need to play stage BGM even when skipping
            PlayStageBgmAfterIntro();
            yield break;
        }
        #endif

        // NEW: Play camera intro sequence BEFORE countdown
        bool hadCameraSequence = false;
        if (!skipCameraIntro)
        {
            yield return StartCoroutine(PlayCameraIntroSequence());
            hadCameraSequence = true;
        }

        // 1. Play intro BGM at full volume (this will override any sequence BGM)
        if (introBgm != null)
        {
            GameManager.Instance.PlayBgm(introBgm);
            Debug.Log($"🎵 Playing intro BGM: {introBgm.name}");
        }

        // 2. Show "준비" and wait for 3.5 seconds
        uiManager.ShowCountdownText("준비", 3.5f);
        yield return new WaitForSeconds(3.5f);

        // 3. Stop intro BGM
        GameManager.Instance.StopBgm();
        Debug.Log("🎵 Stopped intro BGM");

        // 4. Countdown numbers: "3", "2", "1" — play countdownSfx at 60% volume
        string[] countdownNumbers = { "3", "2", "1" };
        foreach (string number in countdownNumbers)
        {
            uiManager.ShowCountdownText(number, 0.5f);
            if (countdownSfx != null)
                GameManager.Instance.PlaySfx(countdownSfx, 0.12f);
            yield return new WaitForSeconds(1f);
        }

        // 5. "GO!" and play goSfx at 60% volume
        uiManager.ShowCountdownText("GO!", 1f);
        if (goSfx != null)
            GameManager.Instance.PlaySfx(goSfx, 0.12f);

        yield return new WaitForSeconds(0.25f); // Allow GO! visual to show slightly
        StartGame();

        // 6. Start actual stage BGM at full volume
        PlayStageBgmAfterIntro();

        yield return new WaitForSeconds(1f);
        uiManager.HideCountdownUI();
    }

    // Helper method to play stage BGM
    private void PlayStageBgmAfterIntro()
    {
        if (sceneType == SceneType.STAGE && stageNo > 0 && stageNo <= bgmsPerStage.Count)
        {
            var clip = bgmsPerStage[stageNo - 1];
            if (clip != null)
            {
                GameManager.Instance.PlayBgm(clip);
                Debug.Log($"🎵 Playing stage BGM: {clip.name}");
            }
            else
            {
                Debug.LogWarning($"🎵 Stage BGM clip is null for stage {stageNo}");
            }
        }
        else
        {
            Debug.LogWarning($"🎵 Cannot play stage BGM - sceneType: {sceneType}, stageNo: {stageNo}, bgmsPerStage.Count: {bgmsPerStage.Count}");
        }
    }

    // NEW: Camera intro sequence system
    private IEnumerator PlayCameraIntroSequence()
    {
        // Check if we have a valid sequence selected
        if (selectedSequenceIndex < 0 || selectedSequenceIndex >= stageSequences.Count)
        {
            Debug.Log($"No camera sequence selected for this stage (index: {selectedSequenceIndex}, count: {stageSequences.Count})");
            yield break;
        }

        var sequence = stageSequences[selectedSequenceIndex];
        if (!sequence.IsValid())
        {
            Debug.LogWarning($"Invalid camera sequence at index {selectedSequenceIndex} - sequence: {sequence}, cameraSequence: {sequence?.cameraSequence}, shots: {sequence?.cameraSequence?.shots?.Count}");
            yield break;
        }

        Debug.Log($"🎬 Playing camera intro: {sequence.sequenceName}");
        Debug.Log($"🔍 Sequence BGM: {(sequence.sequenceBgm != null ? sequence.sequenceBgm.name : "NULL")}");
        Debug.Log($"🔍 Selected sequence index: {selectedSequenceIndex}");

        // Store original camera settings and disable orbit camera
        Vector3 originalPosition = _cameraObject.transform.position;
        Quaternion originalRotation = _cameraObject.transform.rotation;
        float originalFOV = Camera.main.fieldOfView;
        
        // Disable orbit camera and other camera controllers during sequence
        var orbitCamera = _cameraObject.GetComponent<SimpleOrbitCamera>();
        var resultPosition = _cameraObject.GetComponent<CameraResultPosition>();
        bool orbitWasEnabled = false;
        bool resultWasEnabled = false;
        
        if (orbitCamera != null)
        {
            orbitWasEnabled = orbitCamera.enabled;
            orbitCamera.enabled = false;
            Debug.Log("🎮 Disabled SimpleOrbitCamera for sequence");
        }
        
        if (resultPosition != null)
        {
            resultWasEnabled = resultPosition.enabled;
            resultPosition.enabled = false;
            Debug.Log("🎮 Disabled CameraResultPosition for sequence");
        }

        // Setup sequence BGM if provided
        if (sequence.sequenceBgm != null)
        {
            GameManager.Instance.PlayBgm(sequence.sequenceBgm);
            Debug.Log($"🎵 Playing sequence BGM: {sequence.sequenceBgm.name}");
        }
        else
        {
            // If no sequence BGM, play the regular stage BGM during camera sequence
            if (sceneType == SceneType.STAGE && stageNo > 0 && stageNo <= bgmsPerStage.Count)
            {
                var clip = bgmsPerStage[stageNo - 1];
                if (clip != null)
                {
                    GameManager.Instance.PlayBgm(clip);
                    Debug.Log($"🎵 Playing stage BGM during sequence: {clip.name}");
                }
            }
        }

        // Setup camera player
        _cameraPlayer.cameraSequence = sequence.cameraSequence;

        // Play specific shots or all shots
        if (sequence.shotsToPlay.Count > 0)
        {
            yield return StartCoroutine(PlaySelectedShots(sequence));
        }
        else
        {
            // Play all shots with custom settings
            yield return StartCoroutine(PlayAllShotsWithSettings(sequence));
        }

        // Wait after sequence
        yield return new WaitForSeconds(sequence.pauseAfterSequence);

        // Re-enable orbit camera and position it properly
        if (orbitCamera != null)
        {
            // Set camera to follow player position for smooth transition
            var playerTransform = GameObject.FindWithTag("Player")?.transform;
            if (playerTransform != null)
            {
                // Position camera behind player for smooth orbit camera takeover
                Vector3 behindPlayer = playerTransform.position + (-playerTransform.forward * 5f) + (Vector3.up * 2f);
                _cameraObject.transform.position = behindPlayer;
                _cameraObject.transform.LookAt(playerTransform.position + Vector3.up);
            }
            
            orbitCamera.enabled = orbitWasEnabled;
            Debug.Log("🎮 Re-enabled SimpleOrbitCamera");
        }
        
        if (resultPosition != null)
        {
            resultPosition.enabled = resultWasEnabled;
            Debug.Log("🎮 Re-enabled CameraResultPosition");
        }

        // After camera sequence, ensure the right BGM is playing
        if (sequence.sequenceBgm != null)
        {
            // If we had a special sequence BGM, now switch to stage BGM
            PlayStageBgmAfterIntro();
        }
        // If no sequence BGM was specified, stage BGM should already be playing

        Debug.Log("✅ Camera intro sequence finished, orbit camera restored");
    }

    private IEnumerator PlaySelectedShots(StageIntroSequence sequence)
    {
        foreach (string shotName in sequence.shotsToPlay)
        {
            var shot = sequence.cameraSequence.shots.Find(s => s.shotName == shotName);
            if (shot != null)
            {
                Debug.Log($"▶️ Playing selected shot: {shotName}");
                yield return StartCoroutine(PlaySingleShot(shot, sequence));
                yield return new WaitForSeconds(sequence.pauseBetweenShots);
            }
            else
            {
                Debug.LogWarning($"Shot '{shotName}' not found in sequence!");
            }
        }
    }

    private IEnumerator PlayAllShotsWithSettings(StageIntroSequence sequence)
    {
        for (int i = 0; i < sequence.cameraSequence.shots.Count; i++)
        {
            var shot = sequence.cameraSequence.shots[i];
            Debug.Log($"▶️ Playing shot: {shot.shotName}");
            yield return StartCoroutine(PlaySingleShot(shot, sequence));
            
            // Don't pause after last shot
            if (i < sequence.cameraSequence.shots.Count - 1)
            {
                yield return new WaitForSeconds(sequence.pauseBetweenShots);
            }
        }
    }

    private IEnumerator PlaySingleShot(CameraSequenceAsset.SavedShot shot, StageIntroSequence sequence)
    {
        if (shot.keyframes.Count < 2) yield break;

        float duration = shot.duration / sequence.playbackSpeed;
        float startTime = Time.time;
        
        Camera cam = Camera.main;

        while (Time.time - startTime < duration)
        {
            float progress = (Time.time - startTime) / duration;
            InterpolateCamera(shot.keyframes, progress, cam);
            yield return null;
        }

        // End at exact final position
        var lastFrame = shot.keyframes[shot.keyframes.Count - 1];
        _cameraObject.transform.position = lastFrame.position;
        _cameraObject.transform.rotation = lastFrame.rotation;
        cam.fieldOfView = lastFrame.fov;
    }

    private void InterpolateCamera(List<CameraSequenceAsset.SavedKeyframe> keyframes, float t, Camera cam)
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
        localT = Mathf.SmoothStep(0f, 1f, localT);

        _cameraObject.transform.position = Vector3.Lerp(current.position, next.position, localT);
        _cameraObject.transform.rotation = Quaternion.Lerp(current.rotation, next.rotation, localT);
        cam.fieldOfView = Mathf.Lerp(current.fov, next.fov, localT);
    }

    public void DisablePlayerControl()
    {
        _playerControl.canControl = false;
    }

    public void StartGame()
    {
        if (State != PlayStates.Ready) return;

        State = PlayStates.Playing;
        _playerControl.canControl = true;
        Debug.Log("Playing");
    }

    public void FinishGame()
    {
        if (State != PlayStates.Playing) return;

        State = PlayStates.Finished;
        _playerControl.canControl = false;
        Debug.Log("Finished");

        uiManager.HidePlayUI();
        uiManager.ShowResultUI(_playTimeCurrent, _playTimeTotal, _retryCount);
        _cameraObject.GetComponent<CameraResultPosition>().MoveCamera();

        StartCoroutine(FinishGameCoroutine());
    }

    private IEnumerator FinishGameCoroutine()
    {
        // Play looping jingle
        if (finishJingleLoop != null)
        {
            GameManager.Instance.PlayBgm(finishJingleLoop);
        }

        yield return new WaitForSeconds(2.5f);
        _canMoveToNextStage = true;
    }

    public void LoadNextStage()
    {
        GameManager.Instance.totalPlayTime += _playTimeCurrent;
        GameManager.Instance.SetScore(_playTimeCurrent, stageNo - 1);
        GameManager.Instance.LoadScene(nextSceneName);
    }

    public void UpdateStorySubtitle(string content, float durationSeconds = 2)
    {
        uiManager.UpdateStorySubtitle(content, durationSeconds);
    }

    public void UpdatePlayerLineSubtitle(string content, float durationSeconds = 2)
    {
        uiManager.UpdatePlayerLineSubtitle(content, durationSeconds);
    }
}