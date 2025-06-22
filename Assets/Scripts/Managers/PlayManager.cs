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

        uiManager.ShowCountdownText("준비", DelayBeforeStart);
        yield return new WaitForSeconds(DelayBeforeStart + 0.25f);

        uiManager.ShowCountdownText("3", 0.5f);
        yield return new WaitForSeconds(1f);

        uiManager.ShowCountdownText("2", 0.5f);
        yield return new WaitForSeconds(1f);

        uiManager.ShowCountdownText("1", 0.5f);
        yield return new WaitForSeconds(1f);

        uiManager.ShowCountdownText("GO!", 1f);
        yield return new WaitForSeconds(0.25f);
        StartGame();
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
