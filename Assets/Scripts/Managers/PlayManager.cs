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
    public UIManager uimanager;

    private PlayStates _state;
    private float _playtimeCurrent;
    private float _playtimeTotal;
    public string _stageName; //set in each playmanager

    private void Awake()
    {
        GameManager.Instance.playManager = this;
    }

    private void Start()
    {
        _state = PlayStates.Ready;
        _playtimeCurrent = 0f;
        _playtimeTotal = GameManager.Instance.totalPlayTime;

        // TODO: Set initial UI values
        uimanager.UpdatePlayTime(_playtimeTotal);
        uimanager.UpdateCurrentPlayTime(_playtimeCurrent);
        uimanager.UpdateStage(_stageName);
    }

    private void Update()
    {
        if (_state == PlayStates.Playing)
        {
            _playtimeCurrent += Time.deltaTime;
            _playtimeTotal += Time.deltaTime;
            // TODO: Display playtime
            uimanager.UpdatePlayTime(_playtimeTotal);
            uimanager.UpdateCurrentPlayTime(_playtimeCurrent);
        }
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
        GameManager.Instance.totalPlayTime += _playtimeCurrent;
        GameManager.LoadScene(nextSceneName);

    }
    // TODO: Add UI related functions
}
