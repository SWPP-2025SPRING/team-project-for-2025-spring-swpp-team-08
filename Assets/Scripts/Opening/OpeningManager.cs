using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningManager : MonoBehaviour
{
    public OpeningSequence openingSequence;
    public InputName playerNameInput;

    private bool _isInputActive;

    public void Start()
    {
        StartCoroutine(PlayOpening());
    }

    private void Update()
    {
        if (_isInputActive && Input.GetKeyDown(KeyCode.Return))
        {
            _isInputActive = false;
            playerNameInput.SubmitPlayerName();
            StartTutorial();
        }
    }

    private IEnumerator PlayOpening()
    {
        yield return openingSequence.PlaySequence();
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
