using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputName : MonoBehaviour
{
    public GameObject nameInputPanel;
    public TMP_InputField nameInputField;

    private void Awake()
    {
        nameInputPanel.SetActive(false);
        nameInputField.text = "";
        nameInputField.DeactivateInputField();
    }

    public void Show()
    {
        nameInputPanel.SetActive(true);
        nameInputField.interactable = true;
        nameInputField.text = "";
        nameInputField.ActivateInputField();
    }

    public void OnSubmitClicked()
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
