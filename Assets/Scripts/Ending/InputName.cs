using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InputName : MonoBehaviour
{
    public GameObject nameInputPanel;
    public TMP_InputField nameInputField;
    public Button submitButton;

    public GameObject endingCreditPanel;
    public EndingCreditScript creditRoll;

    public Button scoreButton;
    public Button restartButton;

    private void Awake()
    {
        nameInputPanel.SetActive(false);
        endingCreditPanel.SetActive(false);
        scoreButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);

        submitButton.onClick.AddListener(OnSubmitClicked);
    }

    public void Show()
    {
        endingCreditPanel.SetActive(true);
        creditRoll.onCreditEnd = OnCreditEnd;
        creditRoll.StartRoll();
    }

    private void OnCreditEnd()
    {
        endingCreditPanel.SetActive(false);
        nameInputPanel.SetActive(true);
        nameInputField.text = "";
        nameInputField.ActivateInputField();
    }

    private void OnSubmitClicked()
    {
        string playerName = nameInputField.text;
        if (string.IsNullOrEmpty(playerName))
            playerName = "Anonymous";

        SaveScore(playerName);

        nameInputPanel.SetActive(false);

        SceneManager.LoadScene("LeaderboardScene");
    }

    private void SaveScore(string playerName)
    {
        //float finalScore = GameManager.Instance.totalPlayTime; :추후 수정 이걸로

        float finalScore = 1f; // 테스트용

        var scoreboard = FindObjectOfType<ScoreBoardManager>();
        if (scoreboard != null)
            scoreboard.SaveScore(playerName, finalScore);
    }
}
