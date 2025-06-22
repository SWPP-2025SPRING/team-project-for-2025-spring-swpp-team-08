using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayStates
{
    Ready,
    Playing,
    Finished
}

public class PlayManager : MonoBehaviour
{
    private const float DelayBeforeStart = 3f;

    [Header("Debug Settings")]
    public bool skipCountdown;
    public bool enableSkipStage;

    [Header("Stage Settings")]
    public int stageNo;

    public SceneType sceneType;
    public string nextSceneName;
    public string stageName;
    public float fallThresholdHeight = 0f;
    public float fallThresholdSecond = 5f;

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

    private void Awake()
    {
        GameManager.Instance.playManager = this;

        _playerControl = GameObject.FindWithTag("Player").GetComponentInChildren<NewPlayerControl>();
        _cameraObject = Camera.main?.gameObject;
    }

    private void Start()
    {
        State = PlayStates.Ready;

        PlaySceneBgm(); // Automatically play BGM based on scene type + stageNo

        switch (sceneType)
        {
            case SceneType.OPENING:
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

                StartCoroutine(ReadyGameCoroutine());
                break;

            case SceneType.ENDING:
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

            if (enableSkipStage && Input.GetKeyDown(KeyCode.Backslash))
            {
                FinishGame();
            }
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
        uiManager.UpdateStateSubtitle("체크포인트 설정됨", 3);
        _checkpoint = newCheckpoint;
        GameManager.Instance.PlaySfx(setCheckpoint);
    }

    // set player to chosen location
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
        yield return null; // WAIT FOR ONE FRAME. This is the crucial fix for the race condition.

        // Now that a frame has passed, it's safe to start the game.
        StartGame();

        // We must also start the BGM here, since 'yield break' will skip the logic later on.
        if (sceneType == SceneType.STAGE && stageNo > 0 && stageNo <= bgmsPerStage.Count)
        {
            var clip = bgmsPerStage[stageNo - 1];
            if (clip != null)
                GameManager.Instance.PlayBgm(clip);
        }

        yield break; // Exit the coroutine.
    }
#endif

    // 1. Play intro BGM at full volume
    if (introBgm != null)
        GameManager.Instance.PlayBgm(introBgm);

    // 2. Show "준비" and wait for 3.5 seconds
    uiManager.ShowCountdownText("준비", 3.5f);
    yield return new WaitForSeconds(3.5f);

    // 3. Stop intro BGM
    GameManager.Instance.StopBgm();

    // 4. Countdown numbers: "3", "2", "1"
    string[] countdownNumbers = { "3", "2", "1" };
    foreach (string number in countdownNumbers)
    {
        uiManager.ShowCountdownText(number, 0.5f);
        if (countdownSfx != null)
            GameManager.Instance.PlaySfx(countdownSfx, 0.12f);
        yield return new WaitForSeconds(1f);
    }

    // 5. "GO!"
    uiManager.ShowCountdownText("GO!", 1f);
    if (goSfx != null)
        GameManager.Instance.PlaySfx(goSfx, 0.12f);

    yield return new WaitForSeconds(0.25f);
    StartGame();

    // 6. Start actual stage BGM
    if (sceneType == SceneType.STAGE && stageNo > 0 && stageNo <= bgmsPerStage.Count)
    {
        var clip = bgmsPerStage[stageNo - 1];
        if (clip != null)
            GameManager.Instance.PlayBgm(clip);
    }

    yield return new WaitForSeconds(1f);
    uiManager.HideCountdownUI();
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
            GameManager.Instance.SetBgmLoop(true); // Set loop ON
        }

        yield return new WaitForSeconds(2.5f);
        _canMoveToNextStage = true;
    }


    public void LoadNextStage()
    {
        GameManager.Instance.SetBgmLoop(false); // Stop jingle from looping into next scene
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
