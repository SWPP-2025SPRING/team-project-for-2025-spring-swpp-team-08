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
    private const float DelayBeforeStart = 5f;
    private const float DelayAfterFinish = 5f;

    /// <summary>
    /// Stage number of current scene.
    /// Must be assigned in Unity Inspector.
    /// </summary>
    public int stageNo;

    /// <summary>
    /// Scene name of next stage.
    /// Must be assigned in Unity Inspector.
    /// </summary>
    public string nextSceneName;

    // TODO: Add UIManager reference
    public UIManager uiManager;
    public string stageName;
    public AudioClip setCheckpoint;
    public AudioClip fallDown;
    public Vector3 spawnPoint;
    public float fallThresholdHeight = 0f;
    public float fallThresholdSecond = 5f;

    private PlayStates _state;
    private float _playTimeCurrent;
    private float _playTimeTotal;
    private Vector3 _checkpoint;
    private GameObject _player;

    private void Awake()
    {
        GameManager.Instance.playManager = this;

        _player = GameObject.FindWithTag("Player");
    }

    private void Start()
    {
        _state = PlayStates.Ready;
        _playTimeCurrent = 0f;
        _playTimeTotal = GameManager.Instance.totalPlayTime;
        _checkpoint = spawnPoint;

        uiManager.UpdatePlayTime(_playTimeTotal);
        uiManager.UpdateCurrentPlayTime(_playTimeCurrent);
        uiManager.UpdateStage(stageName);

        SetPlayerControllable(false);

        ReadyGame();
    }

    private void Update()
    {
        if (_state == PlayStates.Playing)
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

    public void ReadyGame()
    {
        Debug.Log("Ready");

        StartCoroutine(ReadyGameCoroutine());
        return;

        IEnumerator ReadyGameCoroutine()
        {
            yield return new WaitForSeconds(DelayBeforeStart);

            StartGame();
        }
    }

    public void StartGame()
    {
        if (_state != PlayStates.Ready) return;

        _state = PlayStates.Playing;
        SetPlayerControllable(true);
        Debug.Log("Playing");
        // TODO: Implement start logic
    }

    public void FinishGame()
    {
        if (_state != PlayStates.Playing) return;

        _state = PlayStates.Finished;
        SetPlayerControllable(false);
        Debug.Log("Finished");
        // TODO: Implement finish logic

        StartCoroutine(LoadNextStageCoroutine());
        return;

        IEnumerator LoadNextStageCoroutine()
        {
            yield return new WaitForSeconds(DelayAfterFinish);

            LoadNextStage();
        }
    }

    public void LoadNextStage()
    {
        GameManager.Instance.totalPlayTime += _playTimeCurrent;
        GameManager.LoadScene(nextSceneName);

    }

    private void SetPlayerControllable(bool value)
    {
        _player.GetComponent<PlayerController>().enabled = value;
        _player.GetComponentInChildren<CameraPivotController>().enabled = value;
    }
    // TODO: Add UI related functions
}
