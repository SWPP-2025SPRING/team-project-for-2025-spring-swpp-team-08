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
    private const float DelayBeforeStart = 1f;
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

    public PlayStates State { get; private set; }

    private float _playTimeCurrent;
    private float _playTimeTotal;
    private Vector3 _checkpoint;
    private NewPlayerControl _playerControl;

    private void Awake()
    {
        GameManager.Instance.playManager = this;

        _playerControl = GameObject.FindWithTag("Player").GetComponentInChildren<NewPlayerControl>();
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
        // TODO: Implement start logic
    }

    public void FinishGame()
    {
        if (State != PlayStates.Playing) return;

        State = PlayStates.Finished;
        _playerControl.canControl = false;
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
        if (stageNo == 1)
        {
            GameManager.Instance.SetScore1(_playTimeCurrent);
        }
        else if (stageNo == 2)
        {
            GameManager.Instance.SetScore2(_playTimeCurrent);
        }
        else if (stageNo == 4)
        {
            GameManager.Instance.SetScore3(_playTimeCurrent);
        }
        GameManager.LoadScene(nextSceneName);

    }

    // TODO: Add UI related functions
}
