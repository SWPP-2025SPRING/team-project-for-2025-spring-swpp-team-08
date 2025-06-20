using System.Collections;

using UnityEngine;

public enum PlayStates
{
    Ready,
    Playing,
    Finished
}

public class PlayManager : MonoBehaviour
{
    private const float DelayBeforeStart = 2f;

    [Header("Stage Settings")]
    public int stageNo;
    public string nextSceneName;
    public string stageName;
    public float fallThresholdHeight = 0f;
    public float fallThresholdSecond = 5f;

    [Header("References")]
    public UIManager uiManager;
    public AudioClip setCheckpoint;
    public AudioClip fallDown;

    [Header("Runtime Values")]
    public Vector3 spawnPoint;

    public PlayStates State { get; private set; }

    private float _playTimeCurrent;
    private float _playTimeTotal;
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
        _playTimeCurrent = 0f;
        _playTimeTotal = GameManager.Instance.totalPlayTime;
        _checkpoint = spawnPoint;

        uiManager.UpdatePlayTime(_playTimeTotal);
        uiManager.UpdateCurrentPlayTime(_playTimeCurrent);
        uiManager.UpdateStage(stageName);

        StartCoroutine(ReadyGame());
    }

    private void Update()
    {
        if (State == PlayStates.Playing)
        {
            _playTimeCurrent += Time.deltaTime;
            _playTimeTotal += Time.deltaTime;
            uiManager.UpdatePlayTime(_playTimeTotal);
            uiManager.UpdateCurrentPlayTime(_playTimeCurrent);
        }
    }

    public void UpdateCheckpoint(Vector3 newCheckpoint)
    {
        uiManager.UpdateSubtitle("Checkpoint set...", 3);
        _checkpoint = newCheckpoint;
        GameManager.Instance.PlaySfx(setCheckpoint);
    }

    //set player to chosen location
    public void DisplayCheckpointReturn()
    {
        uiManager.UpdateSubtitle("Moved to last checkpoint", 3);
        GameManager.Instance.PlaySfx(fallDown);
    }

    public Vector3 GetCurrentCheckpoint()
    {
        return _checkpoint;
    }

    public IEnumerator ReadyGame()
    {
        _playerControl.canControl = false;
        uiManager.ShowPlayUI();
        Debug.Log("Ready");

        yield return new WaitForSeconds(DelayBeforeStart);

        StartGame();
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
        uiManager.ShowResultUI();
        _cameraObject.GetComponent<CameraResultPosition>().MoveCamera();
    }

    public void LoadNextStage()
    {
        GameManager.Instance.totalPlayTime += _playTimeCurrent;
        GameManager.Instance.LoadScene(nextSceneName);
    }

    // TODO: Add UI related functions
}
