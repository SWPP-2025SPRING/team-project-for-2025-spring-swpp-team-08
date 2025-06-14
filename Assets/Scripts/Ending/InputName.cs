using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class InputName : MonoBehaviour
{
    public GameObject nameInputPanel;
    public TMP_InputField nameInputField;
    public Button submitButton;

    public GameObject endingCreditPanel;
    public EndingCreditScript creditRoll;

    public GameObject leaderboardPanel;
    public Button restartButton;

    private System.Action<string> onNameSubmitted;

    private void Awake()
    {
        nameInputPanel.SetActive(false);
        endingCreditPanel.SetActive(false);
        leaderboardPanel.SetActive(false);
        restartButton.gameObject.SetActive(false);

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

        StartCredits();
    }

    private void StartCredits()
    {
        Debug.Log("qqqqqq");
        endingCreditPanel.SetActive(true);
        creditRoll.onCreditEnd = OnCreditEnd;
        creditRoll.StartRoll();
    }

    private void OnCreditEnd()
    {
        endingCreditPanel.SetActive(false);
        leaderboardPanel.SetActive(true);
        restartButton.gameObject.SetActive(true);
    }
}
