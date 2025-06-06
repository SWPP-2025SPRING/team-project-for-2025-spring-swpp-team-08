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
    private GameObject _player;

    private void Awake()
    {
        GameManager.Instance.playManager = this;

        _player = GameObject.FindWithTag("Player");
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

        ReadyGame();
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
        if (State != PlayStates.Ready) return;

        State = PlayStates.Playing;
        Debug.Log("Playing");
        // TODO: Implement start logic
    }

    public void FinishGame()
    {
        if (State != PlayStates.Playing) return;

        State = PlayStates.Finished;
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

    // TODO: Add UI related functions
}
