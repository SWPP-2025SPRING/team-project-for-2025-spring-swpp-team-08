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
        StartCredits();
    }

    private void SaveScore(string playerName)
    {
        //float finalScore = GameManager.Instance.totalPlayTime; 나중에 이거로 바꾸기
        float finalScore = 1f; // UnityEngine.Random.Range(0f, 100f); 

        var scoreboard = FindObjectOfType<ScoreBoardManager>();
        if (scoreboard != null)
            scoreboard.SaveScore(playerName, finalScore);
    }


    private void StartCredits()
    {
        //Debug.Log("qqqqqq");
        endingCreditPanel.SetActive(true);
        creditRoll.onCreditEnd = OnCreditEnd;
        creditRoll.StartRoll();
    }

    private void OnCreditEnd()
    {
        Debug.Log("qqqqqq");
        endingCreditPanel.SetActive(false);
        Debug.Log("qqqqqq1");
        scoreButton.gameObject.SetActive(true);
        Debug.Log("qqqqqq2");
        restartButton.gameObject.SetActive(true);
        Debug.Log("qqqqqq3");
    }
}
