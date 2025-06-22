using System.Collections;
using UnityEngine;

public class OpeningManager : MonoBehaviour
{
    public OpeningSequence openingSequence;
    public InputName playerNameInput;
    public GameObject skipTextUi;

    private bool _isInputActive;
    private Coroutine _openingSequenceCoroutine;

    public void Start()
    {
        _openingSequenceCoroutine = StartCoroutine(PlayOpening());
    }

    private void Update()
    {
        if (_isInputActive && Input.GetKeyDown(KeyCode.Return))
        {
            _isInputActive = false;
            playerNameInput.SubmitPlayerName();
            StartTutorial();
        }

        if (_openingSequenceCoroutine != null && Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(EscapeOpening());
        }
    }

    private IEnumerator PlayOpening()
    {
        skipTextUi.SetActive(true);
        yield return openingSequence.PlaySequence();
        InitiatePlayerNameInput();
        _openingSequenceCoroutine = null;
        skipTextUi.SetActive(false);
    }

    private IEnumerator EscapeOpening()
    {
        StopCoroutine(_openingSequenceCoroutine);
        _openingSequenceCoroutine = null;
        GameManager.Instance.playManager.uiManager.HideAllUIs();
        yield return GameManager.Instance.InitiateTransition();
        openingSequence.FinishSequence();
        InitiatePlayerNameInput();
        skipTextUi.SetActive(false);
    }

    private void InitiatePlayerNameInput()
    {
        playerNameInput.gameObject.SetActive(true);
        playerNameInput.Show();
        GameManager.Instance.UnlockCursor();
        _isInputActive = true;
    }

    public void StartTutorial()
    {
        GameManager.Instance.LockCursor();
        GetPlayManager().StartGame();
    }

    private PlayManager GetPlayManager()
    {
        return GameManager.Instance.playManager;
    }
}
