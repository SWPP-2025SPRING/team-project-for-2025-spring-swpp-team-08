using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningManager : MonoBehaviour
{
    public OpeningSequence openingSequence;
    public InputName playerNameInput;

    public void Start()
    {
        StartCoroutine(PlayOpening());
    }

    private IEnumerator PlayOpening()
    {
        yield return openingSequence.PlaySequence();
        playerNameInput.Show();
    }

    public void StartTutorial()
    {
        GetPlayManager().StartGame();
    }

    private PlayManager GetPlayManager()
    {
        return GameManager.Instance.playManager;
    }
}
