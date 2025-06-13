using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class InputName : MonoBehaviour
{
    public GameObject nameInputPanel;     
    public TMP_InputField nameInputField; 
    public Button submitButton;     

    private System.Action<string> onNameSubmitted;

    private void Awake()
    {
        nameInputPanel.SetActive(false);
        submitButton.onClick.AddListener(OnSubmitClicked);
    }

    public void Show(System.Action<string> onSubmit)
    {
        onNameSubmitted = onSubmit;
        nameInputPanel.SetActive(true);
        nameInputField.text = "";
        nameInputField.ActivateInputField();
    }

    private void OnSubmitClicked()
    {
        string playerName = nameInputField.text;
        if (string.IsNullOrEmpty(playerName))
        {
            playerName = "Anonymous";
        }

        onNameSubmitted?.Invoke(playerName);
        nameInputPanel.SetActive(false);
    }

}
