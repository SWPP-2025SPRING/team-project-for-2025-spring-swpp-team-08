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

    // rigidbody of player
    public Rigidbody playerRigidbody;  

    /// <summary>
    /// Scene name of next stage.
    /// Must be assigned in Unity Inspector.
    /// </summary>
    public string nextSceneName;

    // TODO: Add UIManager reference
    public UIManager uimanager;
    public string stageName;

    private PlayStates _state;
    private float _playtimeCurrent;
    private float _playtimeTotal;
    
    private Vector3 _checkpoint = new Vector3(0, 0, 0);

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
        uimanager.UpdateStage(stageName);
    }

    private void Update()
    {
        if (_state == PlayStates.Playing)
        {
            _playtimeCurrent += Time.deltaTime;
            _playtimeTotal += Time.deltaTime;
            uimanager.UpdatePlayTime(_playtimeTotal);
            uimanager.UpdateCurrentPlayTime(_playtimeCurrent);
            /*if player is fallen, call the following at player:
                uimanager.UpdateSubtitle(_deathSubtitle, 5);
                moveToLastCheckpoint();
            }
            */
        }
    }
    //update checkpoint
    public void UpdateCheckpoint(Vector3 newCheckpoint)
    {
        uimanager.UpdateSubtitle("Checkpoint set...", 3);
        _checkpoint = newCheckpoint;
    }

    //set player to chosen location
    public void moveToLastCheckpoint()
    {
        //TBD
    }

    //check if player has fallen down 
    //can be changed later
    public bool IsPlayerFallen()
    {
        return playerRigidbody.position.y <= -10f;
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
