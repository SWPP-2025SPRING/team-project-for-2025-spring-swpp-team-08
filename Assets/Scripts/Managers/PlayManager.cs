using UnityEngine;

public enum PlayStates
{
    Ready,
    Playing,
    Finished
}

public class PlayManager : MonoBehaviour
{
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

    private PlayStates _state;
    private float _playTimeCurrent;
    private float _playTimeTotal;
    
    private Vector3 _checkpoint = new Vector3(0, 0, 0);

    private void Awake()
    {
        GameManager.Instance.playManager = this;
    }

    private void Start()
    {
        _state = PlayStates.Ready;
        _playTimeCurrent = 0f;
        _playTimeTotal = GameManager.Instance.totalPlayTime;

        uiManager.UpdatePlayTime(_playTimeTotal);
        uiManager.UpdateCurrentPlayTime(_playTimeCurrent);
        uiManager.UpdateStage(stageName);
    }

    private void Update()
    {
        if (_state == PlayStates.Playing)
        {
            _playTimeCurrent += Time.deltaTime;
            _playTimeTotal += Time.deltaTime;
            uiManager.UpdatePlayTime(_playTimeTotal);
            uiManager.UpdateCurrentPlayTime(_playTimeCurrent);
            /*if player is fallen, call the following at player:
             *uimanager.UpdateSubtitle(_deathSubtitle, 5);
             *moveToLastCheckpoint();
             * }
             */
        }
    }

    public void UpdateCheckpoint(Vector3 newCheckpoint)
    {
        uiManager.UpdateSubtitle("Checkpoint set...", 3);
        _checkpoint = newCheckpoint;
    }

    //set player to chosen location
    public void moveToLastCheckpoint()
    {
        //TBD
    }

    public void StartGame()
    {
        if (_state != PlayStates.Ready) return;

        _state = PlayStates.Playing;
        // TODO: Implement start logic
    }

    public void FinishGame()
    {
        if (_state != PlayStates.Playing) return;

        _state = PlayStates.Finished;
        // TODO: Implement finish logic
    }

    public void LoadNextStage()
    {
        GameManager.Instance.totalPlayTime += _playTimeCurrent;
        GameManager.LoadScene(nextSceneName);

    }
    // TODO: Add UI related functions
}
