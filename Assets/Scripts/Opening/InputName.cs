using System.Collections;

using TMPro;
using UnityEngine;

public class InputName : MonoBehaviour
{
    public GameObject nameInputPanel;
    public TMP_InputField nameInputField;

    private void Start()
    {
        nameInputField.text = "";
        nameInputField.DeactivateInputField();
    }

    public void Show()
    {
        nameInputPanel.SetActive(true);
        nameInputField.interactable = true;
        nameInputField.text = "";
        StartCoroutine(ActivateInputFieldCoroutine());
        return;

        IEnumerator ActivateInputFieldCoroutine()
        {
            yield return null;   // Wait one frame to ensure the UI is ready
            nameInputField.Select();
            nameInputField.ActivateInputField();
        }
    }

    public void SubmitPlayerName()
    {
        string playerName = nameInputField.text;
        if (string.IsNullOrEmpty(playerName))
        {
            playerName = "Anonymous";
        }

        GameManager.Instance.SetCurrentPlayerName(playerName);

        nameInputPanel.SetActive(false);
    }
}
