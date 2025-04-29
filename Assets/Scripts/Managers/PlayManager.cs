using UnityEngine;

public enum PlayStates
{
    Ready,
    Playing,
    Finished
}

public class PlayManager : MonoBehaviour
{
    public int stageNo;

    // TODO: Add UIManager reference

    private PlayStates _state;
    private float _playtimeSeconds;

    private void Awake()
    {
        GameManager.Instance.playManager = this;
    }

    private void Start()
    {
        _state = PlayStates.Ready;
        _playtimeSeconds = 0f;

        // TODO: Set initial UI values
    }

    private void Update()
    {
        if (_state == PlayStates.Playing)
        {
            _playtimeSeconds += Time.deltaTime;
            // TODO: Display playtime
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

    // TODO: Add UI related functions
}
